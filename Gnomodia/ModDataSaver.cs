using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Gnomodia.Attributes;

namespace Gnomodia
{
    internal class ModDataSaver
    {
        [Serializable]
        private class NullSaveValue
        {
            public static NullSaveValue Instance { get; private set; }

            static NullSaveValue()
            {
                Instance = new NullSaveValue();
            }
        }

        private class ModDataSaveHeader
        {
            private readonly Stream _stream;
            private readonly Dictionary<IMod, long?> _modSavePositions = new Dictionary<IMod, long?>();

            public ModDataSaveHeader(Stream stream, IEnumerable<IMod> mods)
            {
                _stream = stream;

                foreach (var mod in mods)
                {
                    _modSavePositions.Add(mod, null);
                }
            }

            public void WriteHeader(bool restorePosition = false)
            {
                long currentPosition = _stream.Position;
                _stream.Seek(0, SeekOrigin.Begin);

                BinaryWriter writer = new BinaryWriter(_stream, Encoding.UTF8, true);
                writer.Write(_modSavePositions.Count);
                foreach (var modSaveLocation in _modSavePositions)
                {
                    writer.Write(modSaveLocation.Key.Id);
                    writer.Write(modSaveLocation.Value ?? 0);
                }

                if (restorePosition)
                    _stream.Seek(currentPosition, SeekOrigin.Begin);
            }

            public void ReadHeader(bool restorePosition = false)
            {
                long currentPosition = _stream.Position;
                _stream.Seek(0, SeekOrigin.Begin);

                BinaryReader reader = new BinaryReader(_stream, Encoding.UTF8, true);
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    string modId = reader.ReadString();
                    long saveLocation = reader.ReadInt64();

                    IMod key = _modSavePositions.Keys.SingleOrDefault(m => m.Id == modId);
                    if (key == null)
                        continue;

                    if (saveLocation != 0)
                        _modSavePositions[key] = saveLocation;
                    else
                        _modSavePositions[key] = null;
                }

                if (restorePosition)
                    _stream.Seek(currentPosition, SeekOrigin.Begin);
            }

            public void SetSavePosition(IMod saveableMod, long position)
            {
                _modSavePositions[saveableMod] = position;
            }

            public long? GetSavePosition(IMod saveableMod)
            {
                return _modSavePositions.ContainsKey(saveableMod) ? _modSavePositions[saveableMod] : null;
            }
        }

        private readonly Stream _stream;
        private readonly Dictionary<IMod, MemberInfo> _saveableMods;

        public ModDataSaver(Stream stream, IModManager modManager)
        {
            _stream = stream;
            _saveableMods = modManager.Mods
                .Where(mod => mod.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Union<MemberInfo>(mod.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    .Count(mi => mi.GetCustomAttributes(typeof(SaveObjectAttribute), false).Any()) == 1)
                .ToDictionary(mod => mod, mod => mod.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Union<MemberInfo>(mod.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    .Single(mi => mi.GetCustomAttributes(typeof(SaveObjectAttribute), false).Any()));
        }

        public void Save()
        {
            ModDataSaveHeader header = new ModDataSaveHeader(_stream, _saveableMods.Keys);
            header.WriteHeader();

            BinaryFormatter formatter = new BinaryFormatter();
            foreach (var saveableObjectMod in _saveableMods)
            {
                header.SetSavePosition(saveableObjectMod.Key, _stream.Position);

                var fieldInfo = saveableObjectMod.Value as FieldInfo;
                if (fieldInfo != null)
                {
                    object saveObject = fieldInfo.GetValue(saveableObjectMod.Key);
                    formatter.Serialize(_stream, saveObject ?? NullSaveValue.Instance);
                }
                else
                {
                    var propertyInfo = saveableObjectMod.Value as PropertyInfo;
                    if (propertyInfo != null)
                    {
                        object saveObject = propertyInfo.GetMethod.Invoke(saveableObjectMod.Key, null);
                        formatter.Serialize(_stream, saveObject ?? NullSaveValue.Instance);
                    }
                }
            }
            header.WriteHeader();
        }

        public void Load()
        {
            ModDataSaveHeader header = new ModDataSaveHeader(_stream, _saveableMods.Keys);
            header.ReadHeader();

            BinaryFormatter formatter = new BinaryFormatter();
            foreach (var saveableObjectMod in _saveableMods)
            {
                long? position = header.GetSavePosition(saveableObjectMod.Key);
                if (!position.HasValue)
                    continue;

                _stream.Seek(position.Value, SeekOrigin.Begin);

                var fieldInfo = saveableObjectMod.Value as FieldInfo;
                if (fieldInfo != null)
                {
                    object saveObject = formatter.Deserialize(_stream);
                    if (saveObject is NullSaveValue)
                    {
                        fieldInfo.SetValue(saveableObjectMod.Key, null);
                    }
                    else
                    {
                        fieldInfo.SetValue(saveableObjectMod.Key, saveObject);
                    }
                }
                else
                {
                    var propertyInfo = saveableObjectMod.Value as PropertyInfo;
                    if (propertyInfo != null)
                    {
                        object saveObject = formatter.Deserialize(_stream);
                        if (saveObject is NullSaveValue)
                        {
                            propertyInfo.SetMethod.Invoke(saveableObjectMod.Key, new object[] { null });
                        }
                        else
                        {
                            propertyInfo.SetMethod.Invoke(saveableObjectMod.Key, new[] { saveObject });
                        }
                    }
                }
            }
        }
    }
}
