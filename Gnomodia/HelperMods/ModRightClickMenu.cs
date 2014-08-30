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
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using Game.GUI;
using Game.GUI.Controls;
using Gnomodia.Annotations;
using Gnomodia.Attributes;
using Gnomodia.Events;

namespace Gnomodia.HelperMods
{
    [Export(typeof(IMod))]
    public class ModRightClickMenu : SupportMod
    {
        #region public stuff
        private readonly Dictionary<String, ModMenuItemClickedCallback> _modMenuItems = new Dictionary<string, ModMenuItemClickedCallback>();

        [Instance]
        private static ModRightClickMenu Instance { get; [UsedImplicitly] set; }

        public delegate void ModMenuItemClickedCallback();
        #endregion
        #region Setup stuff
        public override string Author
        {
            get
            {
                return "Faark";
            }
        }
        public override string Description
        {
            get
            {
                return "Helper object that makes it easy for other mods to place items in the right click menu under a \"Mods\" menu";
            }
        }
        public override Version Version
        {
            get { return typeof(ModRightClickMenu).Assembly.GetName().Version; }
        }
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

        public override IEnumerable<IModification> Modifications
        {
            get
            {
                yield return new MethodHook(
                    typeof(RightClickMenu).GetConstructor(new Type[] { }),
                    Method.Of<RightClickMenu>(OnRightClickMenuCreated)
                );
            }
        }
        private static FieldInfo s_RightClickMenuContextMenu;

        [EventListener]
        public void InitializeRightClickMenu(object sender, PreGameInitializeEventArgs eventArgs)
        {
            s_RightClickMenuContextMenu = typeof(RightClickMenu)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(field => field.FieldType == typeof(ContextMenu));
        }

        public static void OnRightClickMenuCreated(RightClickMenu rightClickMenu)
        {
            var contextMenu = (ContextMenu)(s_RightClickMenuContextMenu.GetValue(rightClickMenu));
            var modsGroup = new MenuItem("Mods");
            foreach (var modMenuItem in Instance._modMenuItems)
            {
                AddMenuItem(modMenuItem, modsGroup);
            }
            if (!Instance._modMenuItems.Any())
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
