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
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using Gnomodia;
using Gnomodia.Attributes;
using Gnomodia.Utility;
using GnomodiaUI.Annotations;

namespace GnomodiaUI
{
    internal class GameModder
    {
        [Import]
        private IModManager ModManager { get; [UsedImplicitly] set; }

        public void ModifyGnomoria()
        {
            Reference.GnomodiaDirectory.GetFile(RuntimeModController.Log.LogfileName).Delete();

            var sourceExe = Reference.GameDirectory.GetFile(Reference.OriginalExecutable);
            var moddedExe = Reference.GnomodiaDirectory.GetFile(Reference.ModdedExecutable);
            var sourceLib = Reference.GameDirectory.GetFile(Reference.OriginalLibrary);
            var moddedLib = Reference.GnomodiaDirectory.GetFile(Reference.ModdedLibrary);

            var gameInjector = new GnomoriaExeInjector(sourceExe);
            var libInjector = new Injector(sourceLib);

            gameInjector.InitializeGnomodia(Reference.GnomodiaDirectory.GetFile(Reference.GnomodiaLibrary));
            gameInjector.InjectMapGenerationCalls();
            gameInjector.InjectSaveLoadCalls();
            //gameInjector.Debug_ManipulateStuff();

            foreach (var mod in ModManager.CreateOrGetAllMods())
            {
                var interceptedMethods =
                    from method in mod.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                    where method.GetCustomAttributes(typeof(InterceptMethodAttribute), false).Any()
                    select new { Method = method, Attribute = method.GetCustomAttributes(typeof(InterceptMethodAttribute), false).Cast<InterceptMethodAttribute>().Single() };

                foreach (var interceptedMethod in interceptedMethods)
                {
                    var attribute = interceptedMethod.Attribute;
                    IModification mh = new MethodHook(attribute.InterceptedMethod, interceptedMethod.Method, attribute.HookType, attribute.HookFlags);
                    if (gameInjector.AssemblyContainsType(mh.TargetType))
                    {
                        gameInjector.InjectModification(mh);
                    }
                    else if (libInjector.AssemblyContainsType(mh.TargetType))
                    {
                        libInjector.InjectModification(mh);
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("Cannot change behavior of type {0}!", mh.TargetType));
                    }
                }
            }

            gameInjector.Write(moddedExe);
            libInjector.Write(moddedLib);
        }
    }
}