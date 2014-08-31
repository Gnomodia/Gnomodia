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
using Gnomodia;
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

            foreach (var modification in ModManager.Mods.SelectMany(mod => mod.Modifications))
            {
                if (gameInjector.AssemblyContainsType(modification.TargetType))
                {
                    gameInjector.InjectModification(modification);
                }
                else if (libInjector.AssemblyContainsType(modification.TargetType))
                {
                    libInjector.InjectModification(modification);
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Cannot change behavior of type {0}!", modification.TargetType));
                }
            }

            gameInjector.Write(moddedExe);
            libInjector.Write(moddedLib);
        }
    }
}