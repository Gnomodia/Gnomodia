/*
 *  Gnomodia UI
 *
 *  Copyright © 2013 Faark (http://faark.de/)
 *  Copyright © 2013 Alexander Krivács Schrøder (https://alexanderschroeder.net/)
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
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Game;
using GameLibrary;
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
        internal delegate void PregameInitialize(object sender, PregameInitializeEventArgs args);

        [ImportMany(typeof(IMod), RequiredCreationPolicy = CreationPolicy.Shared), UsedImplicitly]
        IEnumerable<IMod> _mods;

        public IMod[] Mods
        {
            get
            {
                return _mods/*.Select(m => m.Value)*/.ToArray();
            }
        }

        public void OnImportsSatisfied()
        {
            InitializeMods();
        }

        private void InitializeMods()
        {
            foreach (var mod in Mods)
            {
                SetInstances(mod);
                HookUpEvents(mod);
            }
        }

        private void HookUpEvents(IMod mod)
        {
            // Get all event listeners from mod
            var methodEvents = from method in mod.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                               where method.GetCustomAttributes(typeof(EventListenerAttribute), false).Any()
                               select method;

            // Pre-game Initialize events
            foreach (var pregameInitializeDelegate in methodEvents.Select(methodEvent => Delegate.CreateDelegate(typeof(PregameInitialize), mod, methodEvent, false)).OfType<PregameInitialize>())
                PregameInitializeEvent += pregameInitializeDelegate;

        }

        private event PregameInitialize PregameInitializeEvent;
        public void OnPregameInitializeEvent(PregameInitializeEventArgs args)
        {
            PregameInitialize handler = PregameInitializeEvent;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void SetInstances(IMod mod)
        {
            var fieldInstances =
                from field in
                    mod.GetType()
                        .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                where field.GetCustomAttributes(typeof(InstanceAttribute), false).Any()
                select field;
            foreach (var instanceMember in fieldInstances)
            {
                Type instanceType = instanceMember.FieldType;
                IMod instanceMod = Mods.SingleOrDefault(m => m.GetType() == instanceType);
                if (instanceMod != null)
                    instanceMember.SetValue(mod, instanceMod);
            }
            var propertyInstances =
                from property in
                    mod.GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                where property.GetCustomAttributes(typeof(InstanceAttribute), false).Any()
                select property;
            foreach (var instanceMember in propertyInstances)
            {
                Type instanceType = instanceMember.PropertyType;
                IMod instanceMod = Mods.SingleOrDefault(m => m.GetType() == instanceType);
                if (instanceMod != null)
                instanceMember.SetValue(mod, mod, null);
            }
        }
    }
}
