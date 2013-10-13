using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Gnomodia;
using Gnomodia.Properties;
using GnomodiaUI.Properties;
using GnomoriaModUI;

namespace GnomodiaUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
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

            // Todo: Offer a "just build [now], do not start" mode
            var buildAndQuit = false;
            foreach (var arg in args)
            {
                switch (arg.ToUpper())
                {
                    case "-LAUNCH":
                        GameLauncher.Run();
                        return;
                    case "-BUILD":
                        buildAndQuit = true;
                        //todo: actually implement this
                        break;
                }
            }

            switch (2)
            {
                case 0:
                    App.Main();
                    break;
                case 1:
                    //Application.EnableVisualStyles();
                    //Application.SetCompatibleTextRenderingDefault(false);
                    //Application.Run(new NewUI());
                    break;
                case 2:
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                    break;
            }
        }

        private static bool VerifyOrConfigureGnomoriaInstallationPath()
        {
            string installationPath = Settings.Default.GnomoriaInstallationPath;
            string gnomoriaPath = Path.Combine(installationPath, Reference.OriginalExecutable);
            if (File.Exists(gnomoriaPath))
                return true;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            while (!File.Exists(gnomoriaPath))
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog
                {
                    Description = "Gnomoria installation folder (containing Gnomoria.exe)",
                    ShowNewFolderButton = false
                })
                {
                    if (fbd.ShowDialog() != DialogResult.OK)
                        return false;

                    installationPath = Settings.Default.GnomoriaInstallationPath = fbd.SelectedPath;
                    gnomoriaPath = Path.Combine(installationPath, Reference.OriginalExecutable);
                }
                Settings.Default.Save();
            }

            // We must restart after showing the folder browser dialog, as it interferes with XNA otherwise.
            //GnomodiaModulizer.Instance.RestartGame();
            return false;
        }
    }
}
