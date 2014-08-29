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
            //gameInjector.Inject_SaveLoadCalls();
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