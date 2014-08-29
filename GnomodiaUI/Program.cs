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
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Gnomodia;
using Gnomodia.Properties;
using Gnomodia.Utility;
using GnomoriaModUI;
using Microsoft.Win32;

namespace GnomodiaUI
{
    static class Program
    {
        internal static CompositionContainer Container;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if DEBUG
            // This helps us re-attach the debugger to the application on a restart
            if (!Debugger.IsAttached)
            {
                Thread.Sleep(500);
                Debugger.Launch();
            }
#endif

            // Make sure we know where Gnomoria is
            if (!VerifyOrConfigureGnomoriaInstallationPath())
                return;

            // Custom assembly resolving
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                var trgName = new AssemblyName(e.Name);
                if (trgName.Name == "Gnomoria")
                    return Assembly.Load(File.ReadAllBytes(Reference.GameDirectory.GetFile(Reference.OriginalExecutable).FullName));
                if (trgName.Name == "gnomorialib")
                    return Assembly.Load(File.ReadAllBytes(Reference.GameDirectory.GetFile(Reference.OriginalLibrary).FullName));
                return null;
            };

            // Set up composition
            InitializeCompositionContainer();

            // Mod game
            GameModder gameModder = new GameModder();
            try
            {
                Container.ComposeParts(gameModder);
            }
            catch (ReflectionTypeLoadException e)
            {
                var exes = e.LoaderExceptions;
                foreach (var ex in exes)
                {
                    Trace.WriteLine(ex);
                }
            }
            gameModder.ModifyGnomoria();

            // Launch game
            GameLauncher.Run();
        }

        private static void InitializeCompositionContainer()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(IMod).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog("Mods"));

            Container = new CompositionContainer(catalog);
        }

        private static bool VerifyOrConfigureGnomoriaInstallationPath()
        {
            string installationPath = Settings.Default.GnomoriaInstallationPath;
            string gnomoriaPath = Path.Combine(installationPath, Reference.OriginalExecutable);
            if (File.Exists(gnomoriaPath))
                return true;

            if (GetSteamInstallationPath(out installationPath) && !string.IsNullOrEmpty(installationPath))
            {
                gnomoriaPath = Path.Combine(installationPath, Reference.OriginalExecutable);
                if (File.Exists(gnomoriaPath))
                {
                    Settings.Default.GnomoriaInstallationPath = installationPath;
                    Settings.Default.Save();
                    return true;
                }
            }

            // TODO: Automatically get installation paths from other sources, if possible (Desura?)

            if (GetManualInstallationPath(out installationPath))
            {
                gnomoriaPath = Path.Combine(installationPath, Reference.OriginalExecutable);
                if (File.Exists(gnomoriaPath))
                {
                    Settings.Default.GnomoriaInstallationPath = installationPath;
                    Settings.Default.Save();
                    return true;
                }
            }

            return false;
        }

        private static bool GetManualInstallationPath(out string installationPath)
        {
            string gnomoriaPath = "";
            installationPath = null;
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
            }
            while (!File.Exists(gnomoriaPath))
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog
                {
                    Description = "Gnomoria installation folder (containing Gnomoria.exe)",
                    ShowNewFolderButton = false
                })
                {
                    if (fbd.ShowDialog() != DialogResult.OK)
                    {
                        return false;
                    }

                    installationPath = Settings.Default.GnomoriaInstallationPath = fbd.SelectedPath;
                    gnomoriaPath = Path.Combine(installationPath, Reference.OriginalExecutable);
                }
                Settings.Default.Save();
            }
            return true;
        }

        private static bool GetSteamInstallationPath(out string installationPath)
        {
            var installationKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 224500");
            if (installationKey == null)
            {
                installationPath = null;
                return false;
            }

            installationPath = (string)installationKey.GetValue("InstallLocation");
            return true;
        }
    }
}
