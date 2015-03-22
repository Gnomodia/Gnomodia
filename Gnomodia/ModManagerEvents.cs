/*
 *  Gnomodia
 *
 *  Copyright © 2015 Alexander Krivács Schrøder (https://alexanderschroeder.net/)
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

 /*
  * This file is auto-generated based on the non-abstract classes defined
  * under the "Events" folder. Do not edit directly. If you have added a
  * new event, and want to re-generate this file, right-click on the file
  * named ModManagerEvents.tt and select "Run Custom Tool".
  */

namespace Gnomodia
{
   internal partial interface IModManager
   {
        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        void OnPostGameInitializeEvent(Events.PostGameInitializeEventArgs args);

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        void OnPostGenerateMapEvent(Events.PostGenerateMapEventArgs args);

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        void OnPostLoadGameEvent(Events.PostLoadGameEventArgs args);

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        void OnPostSaveGameEvent(Events.PostSaveGameEventArgs args);

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        void OnPreGameInitializeEvent(Events.PreGameInitializeEventArgs args);

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        void OnPreGenerateMapEvent(Events.PreGenerateMapEventArgs args);

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        void OnPreLoadGameEvent(Events.PreLoadGameEventArgs args);

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        void OnPreSaveGameEvent(Events.PreSaveGameEventArgs args);

   }

   internal partial class ModManager
   {
        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        internal delegate void PostGameInitialize(object sender, Events.PostGameInitializeEventArgs args);

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        internal delegate void PostGenerateMap(object sender, Events.PostGenerateMapEventArgs args);

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        internal delegate void PostLoadGame(object sender, Events.PostLoadGameEventArgs args);

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        internal delegate void PostSaveGame(object sender, Events.PostSaveGameEventArgs args);

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        internal delegate void PreGameInitialize(object sender, Events.PreGameInitializeEventArgs args);

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        internal delegate void PreGenerateMap(object sender, Events.PreGenerateMapEventArgs args);

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        internal delegate void PreLoadGame(object sender, Events.PreLoadGameEventArgs args);

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        internal delegate void PreSaveGame(object sender, Events.PreSaveGameEventArgs args);

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        private event PostGameInitialize PostGameInitializeEvent;
        
        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        public void OnPostGameInitializeEvent(Events.PostGameInitializeEventArgs args)
        {
            PostGameInitialize handler = PostGameInitializeEvent;
            if (handler != null)
                handler(this, args);
        }

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        private event PostGenerateMap PostGenerateMapEvent;
        
        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        public void OnPostGenerateMapEvent(Events.PostGenerateMapEventArgs args)
        {
            PostGenerateMap handler = PostGenerateMapEvent;
            if (handler != null)
                handler(this, args);
        }

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        private event PostLoadGame PostLoadGameEvent;
        
        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        public void OnPostLoadGameEvent(Events.PostLoadGameEventArgs args)
        {
            PostLoadGame handler = PostLoadGameEvent;
            if (handler != null)
                handler(this, args);
        }

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        private event PostSaveGame PostSaveGameEvent;
        
        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        public void OnPostSaveGameEvent(Events.PostSaveGameEventArgs args)
        {
            PostSaveGame handler = PostSaveGameEvent;
            if (handler != null)
                handler(this, args);
        }

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        private event PreGameInitialize PreGameInitializeEvent;
        
        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        public void OnPreGameInitializeEvent(Events.PreGameInitializeEventArgs args)
        {
            PreGameInitialize handler = PreGameInitializeEvent;
            if (handler != null)
                handler(this, args);
        }

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        private event PreGenerateMap PreGenerateMapEvent;
        
        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        public void OnPreGenerateMapEvent(Events.PreGenerateMapEventArgs args)
        {
            PreGenerateMap handler = PreGenerateMapEvent;
            if (handler != null)
                handler(this, args);
        }

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        private event PreLoadGame PreLoadGameEvent;
        
        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        public void OnPreLoadGameEvent(Events.PreLoadGameEventArgs args)
        {
            PreLoadGame handler = PreLoadGameEvent;
            if (handler != null)
                handler(this, args);
        }

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        private event PreSaveGame PreSaveGameEvent;
        
        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        public void OnPreSaveGameEvent(Events.PreSaveGameEventArgs args)
        {
            PreSaveGame handler = PreSaveGameEvent;
            if (handler != null)
                handler(this, args);
        }

        [System.CodeDom.Compiler.GeneratedCode("ModManagerEvents", "1.1")]
        private void HookUpEvents(IMod mod, System.Collections.Generic.List<System.Reflection.MethodInfo> eventMethods)
        {
            foreach (PostGameInitialize postGameInitializeDelegate in GetEventDelegates<Events.PostGameInitializeEventArgs, PostGameInitialize>(eventMethods, mod))
                PostGameInitializeEvent += postGameInitializeDelegate;

            foreach (PostGenerateMap postGenerateMapDelegate in GetEventDelegates<Events.PostGenerateMapEventArgs, PostGenerateMap>(eventMethods, mod))
                PostGenerateMapEvent += postGenerateMapDelegate;

            foreach (PostLoadGame postLoadGameDelegate in GetEventDelegates<Events.PostLoadGameEventArgs, PostLoadGame>(eventMethods, mod))
                PostLoadGameEvent += postLoadGameDelegate;

            foreach (PostSaveGame postSaveGameDelegate in GetEventDelegates<Events.PostSaveGameEventArgs, PostSaveGame>(eventMethods, mod))
                PostSaveGameEvent += postSaveGameDelegate;

            foreach (PreGameInitialize preGameInitializeDelegate in GetEventDelegates<Events.PreGameInitializeEventArgs, PreGameInitialize>(eventMethods, mod))
                PreGameInitializeEvent += preGameInitializeDelegate;

            foreach (PreGenerateMap preGenerateMapDelegate in GetEventDelegates<Events.PreGenerateMapEventArgs, PreGenerateMap>(eventMethods, mod))
                PreGenerateMapEvent += preGenerateMapDelegate;

            foreach (PreLoadGame preLoadGameDelegate in GetEventDelegates<Events.PreLoadGameEventArgs, PreLoadGame>(eventMethods, mod))
                PreLoadGameEvent += preLoadGameDelegate;

            foreach (PreSaveGame preSaveGameDelegate in GetEventDelegates<Events.PreSaveGameEventArgs, PreSaveGame>(eventMethods, mod))
                PreSaveGameEvent += preSaveGameDelegate;

         
        }
    }
}

