/*
 *  Gnomodia
 *
 *  Copyright © 2013-2015 Alexander Krivács Schrøder (https://alexanderschroeder.net/)
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

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
            private readonly IModManager _modManager;
            private readonly Dictionary<IMod, long?> _modSavePositions = new Dictionary<IMod, long?>();

            public ModDataSaveHeader(Stream stream, IEnumerable<IMod> mods, IModManager modManager)
            {
                _stream = stream;
                _modManager = modManager;

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
                    writer.Write(_modManager.GetModMetadata(modSaveLocation.Key).Id);
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

                    IMod key = _modSavePositions.Keys.SingleOrDefault(m => _modManager.GetModMetadata(m).Id == modId);
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
        private readonly IModManager _modManager;
        private readonly Dictionary<IMod, MemberInfo> _saveableMods;

        public ModDataSaver(Stream stream, IModManager modManager)
        {
            _stream = stream;
            _modManager = modManager;
            _saveableMods = modManager.CreateOrGetAllMods()
                .Where(mod => mod.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Union<MemberInfo>(mod.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    .Count(mi => mi.GetCustomAttributes(typeof(SaveObjectAttribute), false).Any()) == 1)
                .ToDictionary(mod => mod, mod => mod.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Union<MemberInfo>(mod.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    .Single(mi => mi.GetCustomAttributes(typeof(SaveObjectAttribute), false).Any()));
        }

        public void Save()
        {
            ModDataSaveHeader header = new ModDataSaveHeader(_stream, _saveableMods.Keys,_modManager);
            
            // Allocate the required header space
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

            // Re-write the header, now with the mods' save positions
            header.WriteHeader();
        }

        public void Load()
        {
            ModDataSaveHeader header = new ModDataSaveHeader(_stream, _saveableMods.Keys,_modManager);
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
