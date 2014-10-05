/*
 *  Gnomodia UI
 *
 *  Copyright © 2013 Faark (http://faark.de/)
 *  Copyright © 2013, 2014 Alexander Krivács Schrøder (https://alexanderschroeder.net/)
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
using System.Linq;
using System.Reflection;
using Gnomodia;
using System.ComponentModel.Composition;
using Gnomodia.Annotations;
using Gnomodia.Attributes;
using Gnomodia.Events;

namespace GnomodiaUI
{
    [Export(typeof(IModManager))]
    internal class ModManager : IModManager, IPartImportsSatisfiedNotification
    {
        internal delegate void PreGameInitialize(object sender, PreGameInitializeEventArgs args);
        internal delegate void PostGameInitialize(object sender, PostGameInitializeEventArgs args);
        internal delegate void PreSaveGame(object sender, PreSaveGameEventArgs args);
        internal delegate void PostSaveGame(object sender, PostSaveGameEventArgs args);
        internal delegate void PreLoadGame(object sender, PreLoadGameEventArgs args);
        internal delegate void PostLoadGame(object sender, PostLoadGameEventArgs args);

        [ImportMany(typeof(IMod), RequiredCreationPolicy = CreationPolicy.Shared), UsedImplicitly]
        IEnumerable<Lazy<IMod, IModMetadata>> _mods;

        public IModMetadata[] ModMetadata
        {
            get { return _mods.Select(m => m.Metadata).ToArray(); }
        }

        public IModMetadata GetModMetadata(IMod mod)
        {
            return _mods.Where(m => m.Value == mod).Select(m => m.Metadata).SingleOrDefault();
        }

        public IMod CreateOrGet(IModMetadata metadata)
        {
            return metadata == null
                ? null
                : _mods.Where(m => m.Metadata == metadata).Select(m => m.Value).SingleOrDefault();
        }

        public IMod CreateOrGet(string modId)
        {
            return CreateOrGet(_mods.Where(m => m.Metadata.Id == modId).Select(m => m.Metadata).SingleOrDefault());
        }

        public IMod[] CreateOrGetAllMods()
        {
            return _mods.Select(m => m.Value).ToArray();
        }

        public void OnImportsSatisfied()
        {
            InitializeMods();
        }

        private void InitializeMods()
        {
            foreach (var mod in CreateOrGetAllMods())
            {
                SetInstances(mod);
                HookUpEvents(mod);
            }
        }

        private static IEnumerable<TDelegate> GetEventDelegates<TEventArgs, TDelegate>(IEnumerable<MethodInfo> eventMethods, IMod mod)
        {
            return (from eventMethod in eventMethods
                    let parameters = eventMethod.GetParameters()
                    where parameters.Length == 2 && parameters[1].ParameterType == typeof(TEventArgs)
                    select Delegate.CreateDelegate(typeof(TDelegate), mod, eventMethod, false)).OfType<TDelegate>();
        }

        private void HookUpEvents(IMod mod)
        {
            // Get all event listeners from mod
            var eventMethods = (from method in mod.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                where method.GetCustomAttributes(typeof(EventListenerAttribute), false).Any()
                                select method).ToList();

            // Pre-game Initialize events
            foreach (var preGameInitializeDelegate in GetEventDelegates<PreGameInitializeEventArgs, PreGameInitialize>(eventMethods, mod))
                PreGameInitializeEvent += preGameInitializeDelegate;

            // Post-game Initialize events
            foreach (var postGameInitializeDelegate in GetEventDelegates<PostGameInitializeEventArgs, PostGameInitialize>(eventMethods, mod))
                PostGameInitializeEvent += postGameInitializeDelegate;

            // Pre-save game events
            foreach (var preSaveGameDelegate in GetEventDelegates<PreSaveGameEventArgs, PreSaveGame>(eventMethods, mod))
                PreSaveGameEvent += preSaveGameDelegate;

            // Post-save game events
            foreach (var postSaveGameDelegate in GetEventDelegates<PostSaveGameEventArgs, PostSaveGame>(eventMethods, mod))
                PostSaveGameEvent += postSaveGameDelegate;

            // Pre-load game events
            foreach (var preLoadGameDelegate in GetEventDelegates<PreLoadGameEventArgs, PreLoadGame>(eventMethods, mod))
                PreLoadGameEvent += preLoadGameDelegate;

            // Post-loadgame events
            foreach (var postLoadGameDelegate in GetEventDelegates<PostLoadGameEventArgs, PostLoadGame>(eventMethods, mod))
                PostLoadGameEvent += postLoadGameDelegate;
        }

        private event PreGameInitialize PreGameInitializeEvent;

        public void OnPreGameInitializeEvent(PreGameInitializeEventArgs args)
        {
            var handler = PreGameInitializeEvent;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private event PostGameInitialize PostGameInitializeEvent;
        public void OnPostGameInitializeEvent(PostGameInitializeEventArgs args)
        {
            var handler = PostGameInitializeEvent;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private event PreSaveGame PreSaveGameEvent;
        public void OnPreSaveGameEvent(PreSaveGameEventArgs args)
        {
            var handler = PreSaveGameEvent;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private event PostSaveGame PostSaveGameEvent;
        public void OnPostSaveGameEvent(PostSaveGameEventArgs args)
        {
            var handler = PostSaveGameEvent;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private event PreLoadGame PreLoadGameEvent;
        public void OnPreLoadGameEvent(PreLoadGameEventArgs args)
        {
            var handler = PreLoadGameEvent;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private event PostLoadGame PostLoadGameEvent;
        public void OnPostLoadGameEvent(PostLoadGameEventArgs args)
        {
            var handler = PostLoadGameEvent;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void SetInstances(IMod mod)
        {
            var fieldInstances =
                from field in mod.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                where field.GetCustomAttributes(typeof(InstanceAttribute), false).Any()
                select field;
            foreach (var instanceMember in fieldInstances)
            {
                Type instanceType = instanceMember.FieldType;
                IMod instanceMod = CreateOrGetAllMods().SingleOrDefault(m => m.GetType() == instanceType);
                if (instanceMod != null)
                    instanceMember.SetValue(mod, instanceMod);
            }
            var propertyInstances =
                from property in mod.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                where property.GetCustomAttributes(typeof(InstanceAttribute), false).Any()
                select property;
            foreach (var instanceMember in propertyInstances)
            {
                Type instanceType = instanceMember.PropertyType;
                IMod instanceMod = CreateOrGetAllMods().SingleOrDefault(m => m.GetType() == instanceType);
                if (instanceMod != null)
                    instanceMember.SetValue(mod, mod, null);
            }
        }
    }
}
