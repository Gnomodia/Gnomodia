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
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Gnomodia.Utility.Serialization
{
    public static class Json
    {
        public static void ToJson<T>(T value, Stream targetStream, params Type[] knownTypes)
        {
            if (targetStream == null)
            {
                throw new ArgumentNullException("targetStream");
            }
            var jsonSerializer = new DataContractJsonSerializer(typeof(T), knownTypes);
            jsonSerializer.WriteObject(targetStream, value);
        }

        public static string ToJson<T>(T objectToSerialize, params Type[] knownTypes)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                ToJson(objectToSerialize, stream, knownTypes);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public static T FromJson<T>(Stream sourceStream, params Type[] knownTypes)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(T), knownTypes);
            return (T)jsonSerializer.ReadObject(sourceStream);
        }

        public static T FromJson<T>(string jsonText, params Type[] knownTypes)
        {
            return FromJson<T>(new MemoryStream(Encoding.UTF8.GetBytes(jsonText)), knownTypes);
        }
    }
}