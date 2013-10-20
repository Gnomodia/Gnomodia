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
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Gnomodia.Utility;

namespace Gnomodia
{
    [DataContract]
    public class ModType
    {

        [DataMember(Name = "ModType")]
        private String m_modTypeName;
        private Type m_modType;


        public Type TryGetType()
        {
            if (m_modType != null)
                return m_modType;
            if (m_modTypeName != null)
            {
                return m_modType = Type.GetType(
                    m_modTypeName,
                    an => AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(la => la.FullName == an.FullName),
                    (a, tn, casesense) => a.GetType(tn, false, true),
                    false
                    );
            }
            return null;
        }

        public Type Type
        {
            get
            {
                if (m_modType != null)
                    return m_modType;
                if (m_modTypeName != null)
                {
                    return m_modType = Type.GetType(
                        m_modTypeName,
                        an => AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(la => la.FullName == an.FullName),
                        (a, tn, casesense) => a.GetType(tn, false, true),
                        true
                        );
                }
                return null;
            }
        }
        public String TypeName
        {
            get
            {
                var tryType = TryGetType();
                return tryType != null ? tryType.AssemblyQualifiedName : m_modTypeName;
            }
        }

        public IMod GetInstance()
        {
            return ModEnvironment.Mods[this];
        }

        public ModType(IMod mod)
        {
            m_modType = mod.GetType();
            m_modTypeName = TypeName;
        }
        public ModType(string typeName)
        {
            m_modTypeName = typeName;
        }
        public ModType(Type sysType)
        {
            m_modType = sysType;
            m_modTypeName = TypeName;
        }
        protected ModType() { }

        public static implicit operator ModType(Mod toCreateFrom)
        {
            return new ModType(toCreateFrom);
        }
    }

    [DataContract]
    public class ModReference : ModType
    {
        [DataMember(Name = "Hash")]
        private String m_dllHash;
        [DataMember(Name = "AssemblyFileName")]
        private String m_assemblyFileName;
        [DataMember(Name = "SetupData", EmitDefaultValue = false)]
        private String m_setupData = null;
        public String Hash
        {
            get { return m_dllHash; }
        }
        public String AssemblyFileName
        {
            get { return m_assemblyFileName; }
        }
        public System.IO.FileInfo AssemblyFile
        {
            get
            {
                return new System.IO.FileInfo(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), m_assemblyFileName));
            }
        }
        public String SetupData
        {
            get
            {
                return m_setupData;
            }
        }

        protected ModReference() { }
        public ModReference(IMod mod)
            : base(mod)
        {
            m_setupData = mod.SetupData;
            m_dllHash = new System.IO.FileInfo(new System.Uri(Type.Assembly.CodeBase).LocalPath).GenerateMd5Hash();
            string refPath;
            if (Utility.FileExtensions.GetRelativePathTo(System.IO.Directory.GetCurrentDirectory(), Type.Assembly.CodeBase, out refPath))
            {
                m_assemblyFileName = refPath;
            }
            else
            {
                m_assemblyFileName = Type.Assembly.CodeBase;
            }
        }

        
        public static implicit operator ModReference(Mod toCreateFrom)
        {
            return new ModReference(toCreateFrom);
        }
        
    }
}
