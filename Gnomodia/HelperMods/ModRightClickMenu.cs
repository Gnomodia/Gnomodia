/*
 *  Gnomodia
 *
 *  Copyright © 2013 Faark (http://faark.de/)
 *  Copyright © 2013, 2014 Alexander Krivács Schrøder (https://alexanderschroeder.net/)
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
using System.Reflection;
using Game.GUI;
using Game.GUI.Controls;
using Gnomodia.Attributes;
using Gnomodia.Events;

namespace Gnomodia.HelperMods
{
    [GnomodiaMod(
        Id = "ModRightClickMenu",
        Name = "ModRightClickMenu",
        Author = "Faark",
        Description = "Helper object that makes it easy for other mods to place items in the right click menu under a \"Mods\" menu",
        Version = AssemblyResources.AssemblyBaseVersion + AssemblyResources.AssemblyPreReleaseVersion + "+" + AssemblyResources.GnomoriaTargetVersion)]
    public class ModRightClickMenu : IMod
    {
        #region public stuff
        private readonly Dictionary<String, ModMenuItemClickedCallback> _modMenuItems = new Dictionary<string, ModMenuItemClickedCallback>();

        public delegate void ModMenuItemClickedCallback();
        #endregion

        public void AddButton(string text, ModMenuItemClickedCallback callback)
        {
            if (_modMenuItems == null)
            {
                throw new InvalidOperationException("Looks like you are too late, menu already created...");
            }

            if (!(callback.Method.IsStatic && callback.Method.IsPublic))
                throw new ArgumentException("Click callback has to be public & static! " + callback.Method.Name + " is not.");
            if (UnmutableMethodModification.IsCompilerGenerated(callback.Method))
                throw new ArgumentException("Click callback cannot be compiler generated");
            UnmutableMethodModification.VerifyNestedPublicMethod(callback.Method);

            _modMenuItems.Add(text, callback);
        }

        private FieldInfo _rightClickMenuContextMenu;

        [EventListener]
        public void InitializeRightClickMenu(object sender, PreGameInitializeEventArgs eventArgs)
        {
            _rightClickMenuContextMenu = typeof(RightClickMenu)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(field => field.FieldType == typeof(ContextMenu));
        }

        [InterceptConstructor(typeof(RightClickMenu))]
        public void OnRightClickMenuCreated(RightClickMenu rightClickMenu)
        {
            var contextMenu = (ContextMenu)(_rightClickMenuContextMenu.GetValue(rightClickMenu));
            var modsGroup = new MenuItem("Mods");
            foreach (var modMenuItem in _modMenuItems)
            {
                AddMenuItem(modMenuItem, modsGroup);
            }
            if (!_modMenuItems.Any())
            {
                modsGroup.Enabled = false;
            }
            contextMenu.Items.Insert(contextMenu.Items.Count - 1, modsGroup);
        }

        private static void AddMenuItem(KeyValuePair<string, ModMenuItemClickedCallback> modMenuItem, MenuItem modsGroup)
        {
            var item = new MenuItem(modMenuItem.Key);
            item.Click += (sender, args) => modMenuItem.Value();
            modsGroup.Items.Add(item);
        }

    }
}
