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
    public abstract class Mod : IMod
    {
        public abstract string Id { get; }
        public virtual string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }
        public virtual string Description
        {
            get
            {
                return "v" + this.Version.ToString() + "; " + this.GetType().Namespace + "; " + this.GetType().Assembly.ManifestModule.ScopeName + " v" + this.GetType().Assembly.GetName().Version;
            }
        }
        public virtual string Author
        {
            get
            {
                return "N/A";
            }
        }
        public virtual Version Version
        {
            get
            {
                return new Version();
            }
        }

        public abstract IEnumerable<IModification> Modifications { get; }

        /*public virtual IEnumerable<ModDependency> Dependencies
        {
            get
            {
                return Enumerable.Empty<ModDependency>();
                //yield break;
                //return new IModDependency[0];
            }
        }
        public virtual IEnumerable<ModType> InitAfter
        {
            get
            {
                return Enumerable.Empty<ModType>();
                //yield break;
                //return new string[0];
            }
        }
        public virtual IEnumerable<ModType> InitBefore
        {
            get
            {
                return Enumerable.Empty<ModType>();
                //yield break;
                //return new string[0];
            }
        }
        public virtual String SetupData { get; set; }

        public virtual void Initialize_PreGame() { }
        public virtual void Initialize_ModDiscovery() { }
        public virtual void Initialize_PreGeneration() { }
        public virtual void PreWorldCreation(ModSaveData data, Game.Map map, Game.CreateWorldOptions options) { }
        public virtual void PostWorldCreation(ModSaveData data, Game.Map map, Game.CreateWorldOptions options) { }
        public virtual void PreGameLoaded(ModSaveData data) { }
        public virtual void AfterGameLoaded(ModSaveData data) { }
        public virtual void PreGameSaved(ModSaveData data) { }
        public virtual void AfterGameSaved(ModSaveData data) { }*/
    }

    public abstract class SupportMod : Mod { }
}
