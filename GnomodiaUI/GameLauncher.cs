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
using System.IO;
using Gnomodia;
using Gnomodia.Properties;
using GnomodiaUI.Properties;

namespace GnomoriaModUI
{
    class GameLauncher : MarshalByRefObject
    {
        // Warning: Make sure there are no dependency-related things in here that could trigger an auto load

        public void RunGame()
        {
            //throw new Exception("LOGGING IS OFF!");
            //base_dir = Settings.Default.GnomoriaInstallationPath;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            Reference.Settings = (ISettings)AppDomain.CurrentDomain.GetData("SettingsProxy");


            var os = Environment.OSVersion;
            if (os.Version.Major > 5)
            {
                AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            }
            try
            {
                var ass = System.Reflection.Assembly.Load(File.ReadAllBytes(Path.Combine(Reference.GnomodiaDirectory.FullName, Reference.ModdedExecutable)));
                var ep = ass.EntryPoint;
                //var inst = ass.GetType("Game.GnomanEmpire").GetProperty("Instance").GetGetMethod().Invoke(null, new object[] { });
                //var obj = ass.CreateInstance(ep.Name);
                ep.Invoke(null, new object[] { new[] { "-noassemblyresolve", "-noassemblyloading" } });
            }
            catch (Exception err)
            {
                CustomErrorHandler(err);
            }
        }

        private Exception _lastFirstChanceException;
        void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            handleStuff_Enter();
            if (!(e.Exception is System.Threading.ThreadAbortException))
            {
                RuntimeModController.Log.Write("FirstChanceException", e.Exception);
                _lastFirstChanceException = e.Exception;
            }
            handleStuff_Leave();
        }
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            handleStuff_Enter();
            if (e.ExceptionObject == _lastFirstChanceException)
            {
                RuntimeModController.Log.Write("First Chance Exception is not handled" + (e.IsTerminating ? ", terminating." : "."));
            }
            else
            {
                RuntimeModController.Log.Write(e.IsTerminating ? "UnhandledException (t)" : "UnhandledException", e.ExceptionObject as Exception);
            }
            handleStuff_Leave();
        }
        private bool _isCurrentlyHandlingSth;
        private void handleStuff_Enter()
        {
            if (_isCurrentlyHandlingSth)
            {
                AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
                AppDomain.CurrentDomain.FirstChanceException -= CurrentDomain_FirstChanceException;
                RuntimeModController.Log.Write("Tried to handle an error while already doing this.");
                throw new Exception("Tried to handle an error while already doing it...");
            }
            _isCurrentlyHandlingSth = true;
        }
        private void handleStuff_Leave()
        {
            _isCurrentlyHandlingSth = false;
        }

        static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var asses = AppDomain.CurrentDomain.GetAssemblies();
            var trgName = new System.Reflection.AssemblyName(args.Name);
            foreach (var ass in asses)
            {
                var aN = new System.Reflection.AssemblyName(ass.FullName);
                if (aN.Name == trgName.Name)
                {
                    return ass;
                }
            }
            if (trgName.Name == "gnomorialib")
            {
                return System.Reflection.Assembly.Load(File.ReadAllBytes(Path.Combine(Reference.GnomodiaDirectory.FullName, trgName.Name + "Modded.dll")));
            }
            if (trgName.Name == "Gnomodia")
            {
                string filePath = Path.Combine(Reference.GnomodiaDirectory.FullName, trgName.Name + ".dll");
                byte[] eh = File.ReadAllBytes(filePath);
                return System.Reflection.Assembly.Load(eh);
            }
            var file = Path.Combine(Reference.GameDirectory.FullName, trgName.Name + ".dll");
            if (File.Exists(file))
            {
                return System.Reflection.Assembly.LoadFile(file);
            }
            file = Path.Combine(Reference.GnomodiaDirectory.FullName, trgName.Name + ".dll");
            if (File.Exists(file))
            {
                return System.Reflection.Assembly.LoadFile(file);
            }
            return null;
        }

        static void CustomErrorHandler(Exception err)
        {
            RuntimeModController.Log.Write("UnhandledException", err);
            System.Windows.Forms.MessageBox.Show(
                "Sorry, but Gnomoria has crashed." + Environment.NewLine
                + Environment.NewLine
                + Environment.NewLine
                + "Check out these logfiles for more information:" + Environment.NewLine
                + RuntimeModController.Log.GetLogfile().FullName + Environment.NewLine
                + RuntimeModController.Log.GetGameLogfile().FullName,
                "Gnomoria [modded] has crashed.");
        }

        public static void Run()
        {
            AppDomainSetup domainSetup = new AppDomainSetup
            {
                ApplicationBase = Reference.ModDirectory.FullName,
                ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
                ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName,
                LoaderOptimization = LoaderOptimization.SingleDomain
            };
            var ad = AppDomain.CreateDomain("GnomoriaDebugEnvironment", null, domainSetup);
            ad.SetData("SettingsProxy", new SettingsProxy());

            var cd = (GameLauncher)ad.CreateInstanceFromAndUnwrap(
                new Uri(typeof(GameLauncher).Assembly.CodeBase).LocalPath,
                typeof(GameLauncher).FullName
                );

            cd.RunGame();
        }

        private class SettingsProxy : MarshalByRefObject, ISettings
        {
            private readonly Settings _settings = Settings.Default;

            public string GnomoriaInstallationPath
            {
                get { return _settings.GnomoriaInstallationPath; }
                set { _settings.GnomoriaInstallationPath = value; }
            }
        }
    }
}
