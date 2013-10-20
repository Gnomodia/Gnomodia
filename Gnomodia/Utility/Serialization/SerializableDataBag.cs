/*
 *  Gnomodia
 *
 *  Copyright © 2013 Faark (http://faark.de/)
 *  Copyright © 2013 Alexander Krivács Schrøder (https://alexanderschroeder.net/)
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU Lesser General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU Lesser General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gnomodia.Utility.Serialization
{
    public static class SerializableDataBag
    {
        public static string ToJson<T>(IEnumerable<Tuple<string, T>> data)
        {
            return data.ToBag().ToJson();
        }

        public static string ToJson<T>(IEnumerable<KeyValuePair<string, T>> data)
        {
            return data.ToBag().ToJson();
        }

        public static SerializableDataBag<T> ToBag<T>(this IEnumerable<Tuple<string, T>> data)
        {
            return new SerializableDataBag<T>(data);
        }

        public static SerializableDataBag<T> ToBag<T>(this IEnumerable<KeyValuePair<string, T>> data)
        {
            return new SerializableDataBag<T>(data);
        }

        public static string ToBagJson<T>(this IEnumerable<Tuple<string, T>> data)
        {
            return ToJson(data);
        }

        public static string ToBagJson<T>(this IEnumerable<KeyValuePair<string, T>> data)
        {
            return ToJson(data);
        }

        public static SerializableDataBag<T> FromJson<T>(string data)
        {
            return SerializableDataBag<T>.FromJson(data);
        }
    }

    [Serializable]
    public class SerializableDataBag<T> : ISerializable, IEnumerable<KeyValuePair<string, T>>
    {
        private readonly Dictionary<string, T> _data = new Dictionary<string, T>();

        public SerializableDataBag(IEnumerable<KeyValuePair<string, T>> initialData)
        {
            if (initialData == null)
                throw new ArgumentNullException("initialData");

            foreach (var element in initialData)
            {
                if (element.Key == null)
                {
                    throw new ArgumentException("Cannot have an empty key!");
                }
                _data.Add(element.Key, element.Value);
            }
        }

        public SerializableDataBag(IEnumerable<Tuple<string, T>> initialData)
        {
            if (initialData == null)
                throw new ArgumentNullException("initialData");

            foreach (var element in initialData)
            {
                if (element.Item1 == null)
                {
                    throw new ArgumentException("Cannot have an empty key!");
                }
                _data.Add(element.Item1, element.Item2);
            }
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (KeyValuePair<string, T> kvp in _data)
            {
                info.AddValue(kvp.Key, kvp.Value);
            }
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected SerializableDataBag(SerializationInfo info, StreamingContext context)
        {
            // TODO: validate inputs before deserializing. See http://msdn.microsoft.com/en-us/library/ty01x675(VS.80).aspx
            foreach (SerializationEntry entry in info)
            {
                _data.Add(entry.Name, (T)entry.Value);
            }
        }

        public string ToJson()
        {
            return Json.ToJson(this, typeof(T));
        }

        public static SerializableDataBag<T> FromJson(string json)
        {
            return Json.FromJson<SerializableDataBag<T>>(json, typeof(T));
        }

        public Dictionary<string, T> ToDictionary()
        {
            return new Dictionary<string,T>(_data);
        }

        public Dictionary<string, TOut> ToDictionary<TOut>(Func<T, TOut> converter)
        {
            var dict = new Dictionary<string, TOut>();
            foreach (var element in _data)
            {
                dict.Add(element.Key, converter(element.Value));
            }
            return dict;
        }

        public Dictionary<string, TOut> ToDictionary<TOut>(Func<string, T, TOut> converter)
        {
            var dict = new Dictionary<string, TOut>();
            foreach (var element in _data)
            {
                dict.Add(element.Key, converter(element.Key, element.Value));
            }
            return dict;
        }
    }
}