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
using System.Linq;
using System.Windows.Forms;
using Gnomodia;
using Gnomodia.HelperMods;
using Gnomodia.Utility;

namespace GnomodiaUI
{
    //[Serializable]
    public class ModdingEnvironmentWriter
    {
        private ModdingEnvironmentConfiguration config;
        private GnomoriaExeInjector game_injector;
        private Injector lib_injector;
        private IMod[] all_mods_to_process = null;
        private IMod[] all_possible_dependencies = null;
        private List<IMod> processedMods = new List<IMod>();//order does matter.
        //private HashSet<IMod> processedMods = new HashSet<IMod>();
        private HashSet<IMod> currentlyProcessing = new HashSet<IMod>();
        private void ProcessMod(IMod mod)
        {
            if (processedMods.Contains(mod))
            {
                return;
            }
            if (currentlyProcessing.Contains(mod))
            {
                throw new InvalidOperationException("Can't process mod [" + mod.Name + "], already processing it. Circular dependency?");
            }
            currentlyProcessing.Add(mod);
            mod.Initialize_PreGeneration();
            if (mod.InitBefore.Count() > 0)
            {
                throw new NotImplementedException("Mod.InitAfter and Mod.InitBefore are not yet supported. Sorry.");
            }
            foreach (var dependency in mod.Dependencies)
            {
                ProcessMod(all_mods_to_process.Union(all_possible_dependencies).Single(depModInstance => depModInstance.GetType() == dependency.Type));
            }
            foreach (var change in mod.Modifications)
            {
                if (game_injector.AssemblyContainsType(change.TargetType))
                {
                    game_injector.Inject_Modification(change);
                }
                else if (lib_injector.AssemblyContainsType(change.TargetType))
                {
                    lib_injector.Inject_Modification(change);
                }
                else
                {
                    throw new InvalidOperationException("Cannot change behavoir of type [" + change.TargetType + "]!");
                }
            }
            processedMods.Add(mod);
            currentlyProcessing.Remove(mod);
        }
        internal ModdingEnvironmentWriter(IMod[] mods_to_use, IMod[] dependencies_to_use, bool useHiDefProfile)
        {

            config = ModdingEnvironmentConfiguration.Create();

            ModEnvironment.RequestSetupDataReset();

            all_mods_to_process = mods_to_use;
            all_possible_dependencies = dependencies_to_use;

            // Force-inject ModDialog
            if (all_mods_to_process.All(m => m.GetType() != typeof(ModDialog)) && all_possible_dependencies.All(m => m.GetType() != typeof(ModDialog)))
            {
                all_mods_to_process = all_mods_to_process.Union(new ModDialog()).ToArray();
            }

            var source_exe = Reference.GameDirectory.GetFile(Reference.OriginalExecutable);
                //new System.IO.FileInfo(System.IO.Path.Combine(base_directoy.FullName, source_exe_name));
            var modded_exe = Reference.GnomodiaDirectory.GetFile(Reference.ModdedExecutable);
                //new System.IO.FileInfo(System.IO.Path.Combine(base_directoy.FullName, modded_exe_name));
            var source_lib = Reference.GameDirectory.GetFile(Reference.OriginalLibrary);
            var modded_lib = Reference.GnomodiaDirectory.GetFile(Reference.ModdedLibrary);



            
            game_injector = new GnomoriaExeInjector(source_exe);
            lib_injector = new Injector(source_lib);
            config.Hashes.SourceExecutable = source_exe.GenerateMd5Hash();
            config.Hashes.SourceLibrary = source_lib.GenerateMd5Hash();

            // may switch those 2 later to have it outside...
            game_injector.Inject_SetContentRootDirectoryToCurrentDir_InsertAtStartOfMain();
            game_injector.Inject_CallTo_ModRuntimeController_Initialize_AtStartOfMain(Reference.GnomodiaDirectory.GetFile(Reference.GnomodiaLibrary));
            //game_injector.Inject_TryCatchWrapperAroundEverthingInMain_WriteCrashLog();
            //game_injector.Inject_CurrentAppDomain_AddResolveEventAtStartOfMain();
            game_injector.Inject_SaveLoadCalls();
            //game_injector.Inject_TryCatchWrapperAroundGnomanEmpire_LoadGame();
            game_injector.Debug_ManipulateStuff();
            if (useHiDefProfile)
            {
                game_injector.Inject_AddHighDefXnaProfile();
            }


            foreach (var mod in all_mods_to_process)
            {
                ProcessMod(mod);
            }

            var allLoadedStuff = processedMods.Select(mod => Tuple.Create(mod, mod.Dependencies.Union(mod.InitAfter.Where(befor => processedMods.Contains(befor.GetInstance()))).Select(type => type.GetInstance())));
            var processedMods_sortedByDependencyAndInitAfter = DependencySorter.Sort(allLoadedStuff);

            config.SetModReferences(processedMods_sortedByDependencyAndInitAfter.Select(mod => new ModReference(mod)).ToArray());

            //Mono.Cecil.WriterParameters
            game_injector.Write(modded_exe);
            lib_injector.Write(modded_lib);
            config.Hashes.ModdedExecutable = modded_exe.GenerateMd5Hash();
            config.Hashes.ModdedLibrary = modded_lib.GenerateMd5Hash();
        }
        public void SaveEnvironmentConfiguration(System.IO.FileInfo xmlConfigFile)
        {
            config.Save(xmlConfigFile);
        }
    }
    internal class ModdingEnvironmentConfiguration : ModEnvironmentConfiguration
    {
        public static ModdingEnvironmentConfiguration Create()
        {
            return new ModdingEnvironmentConfiguration();
        }
        public void SetModReferences(ModReference[] refs)
        {
            ModReferences = refs;
        }
        public void Save(System.IO.FileInfo xmlConfigFile)
        {
            ModEnvironmentConfiguration.Save(new ModEnvironmentConfiguration(this), xmlConfigFile);
        }
        public static ModEnvironmentConfiguration LoadOrCreate(System.IO.FileInfo fileToLoad)
        {
            if (fileToLoad.Exists)
            {
                return Load(fileToLoad);
            }
            else
            {
                return Create();
            }
        }
    }
}
