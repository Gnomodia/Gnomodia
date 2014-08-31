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
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Gnomodia.Attributes;
using Gnomodia.Events;
using Gnomodia.Utility;

namespace Gnomodia
{
    public class RuntimeModController
    {
        private static RuntimeModController Instance { get; set; }

        [Import]
        private IModManager ModManager { get; set; }

        private static void InitializeCompositionContainer()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(IMod).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog(Path.Combine(Path.GetDirectoryName(typeof(IMod).Assembly.Location), "Mods")));

            Container = new CompositionContainer(catalog);
        }

        private static CompositionContainer Container { get; set; }

        public static void Initialize(string[] args)
        {
            InitializeCompositionContainer();
            Instance = new RuntimeModController();
            Container.ComposeParts(Instance);
            Instance.InitializeMods();
        }

        private void InitializeMods()
        {
            try
            {
                ModManager.OnPreGameInitializeEvent(new PreGameInitializeEventArgs());
                ModManager.OnPostGameInitializeEvent(new PostGameInitializeEventArgs());
            }
            catch (Exception err)
            {
                // TODO: Log properly
                //Log.Write(err);
                throw;
            }
        }

        private static string GetSaveLocation(bool fallenKingdom, string worldName)
        {
            var savePath = fallenKingdom ? Game.GnomanEmpire.SaveFolderPath("OldWorlds\\") : Game.GnomanEmpire.SaveFolderPath("Worlds\\");
            // Todo: fallen kingdoms not considered with filename and so on!
            return Path.Combine(savePath, string.Format("{0}.gnomodia", worldName));
        }

        public static void PreSaveGame(Game.GnomanEmpire self, bool fallenKingdom)
        {
            Instance.ModManager.OnPreSaveGameEvent(new PreSaveGameEventArgs(fallenKingdom));
        }

        public static System.Threading.Tasks.Task PostSaveGame(System.Threading.Tasks.Task saveTask, Game.GnomanEmpire self, bool fallenKingdom)
        {
            return saveTask.ContinueWith(task =>
            {
                var saveFile = GetSaveLocation(fallenKingdom, self.CurrentWorld);
                using (FileStream saveStream = File.Create(saveFile))
                {
                    ModDataSaver modDataSaver = new ModDataSaver(saveStream, Instance.ModManager);
                    modDataSaver.Save();
                }

                Instance.ModManager.OnPostSaveGameEvent(new PostSaveGameEventArgs(fallenKingdom));
            });
        }

        public static void PreLoadGame(Game.GnomanEmpire self, string fileName, bool fallenKingdom)
        {
            Instance.ModManager.OnPreLoadGameEvent(new PreLoadGameEventArgs(fallenKingdom));
        }

        public static void PostLoadGame(Game.GnomanEmpire self, string fileName, bool fallenKingdom)
        {
            var saveFile = GetSaveLocation(fallenKingdom, self.CurrentWorld);

            if (File.Exists(saveFile))
                using (FileStream saveStream = File.OpenRead(saveFile))
                {
                    ModDataSaver modDataSaver = new ModDataSaver(saveStream, Instance.ModManager);
                    modDataSaver.Load();
                }

            Instance.ModManager.OnPostLoadGameEvent(new PostLoadGameEventArgs(fallenKingdom));
        }
        public static void PreGenerateMap(Game.Map self, Game.CreateWorldOptions options)
        {
            //modSaveFile = new ModSaveFile();
            foreach (var mod in Instance.ModManager.Mods)
            {
                //mod.PreWorldCreation(modSaveFile.GetDataFor(mod), self, options);
            }
        }
        public static void PostGenerateMap(Game.Map self, Game.CreateWorldOptions options)
        {
            foreach (var mod in Instance.ModManager.Mods)
            {
                //mod.PostWorldCreation(modSaveFile.GetDataFor(mod), self, options);
            }
        }

        public static class Log
        {
            [Flags]
            public enum TargetModes
            {
                UseGlobalSetting = 0x0,
                File = 0x1,
                Screen = 0x2,
                Both = 0x3
            };
            private static TargetModes mTarget = TargetModes.Both;
            public static TargetModes Target
            {
                get
                {
                    return mTarget;
                }
                set
                {
                    if (value == TargetModes.UseGlobalSetting)
                        return;
                    mTarget = TargetModes.Both;
                }
            }
            /// <summary>
            /// This stuff is not yet implemented!
            /// </summary>
            public enum LogLevel { Normal, Info, Warning, Error, Always };
            public static bool SuppressDoubleExceptions = true;

            public const string LogfileName = "GnomoriaModded.log";
            public static System.IO.FileInfo GetLogfile()
            {
                return new System.IO.FileInfo(System.IO.Path.Combine(Reference.GnomodiaDirectory.FullName, LogfileName));
            }
            public static System.IO.FileInfo GetGameLogfile()
            {
                var assembly = Assembly.GetAssembly(typeof(Game.GUI.Controls.Manager));
                string path = Game.GnomanEmpire.SaveFolderPath(null) + System.IO.Path.GetFileNameWithoutExtension(assembly.Location) + ".log";
                return new System.IO.FileInfo(path);
            }

            private static string ObjectContentToString(object obj)
            {
                return (obj == null) ? "-null-" : obj.ToString();
            }
            private static string ObjectContentToStringOrType<T>(T obj)
            {
                if (obj != null)
                {
                    return obj.ToString();
                }
                else
                {
                    return "-null- (" + typeof(T).FullName + ")";
                }
            }
            private static string ObjectToString(object obj)
            {
                if (obj == null)
                    return "-null-";
                else if (obj is string)
                    return obj.ToString();
                else if (obj.GetType().IsValueType)
                    return obj.ToString();
                else
                {
                    return obj.ToString() + "; Hash: " + obj.GetHashCode();
                }
            }

            private static object writeLock = new object();
            private static bool supressWrite = false;
            private static void DoWrite(string text, LogLevel level, TargetModes target)
            {
                lock (writeLock)
                {
                    if (supressWrite)
                        return;
                    supressWrite = true;
                    target = target == TargetModes.UseGlobalSetting ? Target : target;
                    if (target.HasFlag(TargetModes.File))
                    {
                        try
                        {
                            System.IO.File.AppendAllText(GetLogfile().FullName, text);
                        }
                        catch (Exception) { }
                    }
                    if (target.HasFlag(TargetModes.Screen))
                    {
                        try
                        {
                            var g = Game.GnomanEmpire.Instance;
                            if (g != null)
                            {
                                var w = g.World;
                                if (w != null)
                                {
                                    var nm = w.NotificationManager;
                                    if (nm != null)
                                    {
                                        if (g.Region != null)
                                        {
                                            var guiMgr = g.GuiManager;
                                            if (guiMgr != null)
                                            {
                                                if (guiMgr.HUD != null)
                                                {
                                                    nm.AddNotification(text, false);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception) { }
                    }
                    supressWrite = false;
                }
            }

            public static void Write(IEnumerable<String> lines, LogLevel level = LogLevel.Normal, TargetModes target = TargetModes.UseGlobalSetting)
            {
                DoWrite(
                    "Date: " + DateTime.Now.ToString()
                    + Environment.NewLine
                    + String.Join(Environment.NewLine, lines)
                    + Environment.NewLine
                    + Environment.NewLine,
                    level,
                    target
                    );
            }
            public static void Write(LogLevel level = LogLevel.Normal, TargetModes target = TargetModes.UseGlobalSetting, params string[] texts)
            {
                Write((IEnumerable<string>)texts, level, target);
            }
            public static void Write(params string[] texts)
            {
                Write((IEnumerable<string>)texts);
            }
            public static void Write(string text, LogLevel level = LogLevel.Normal, TargetModes target = TargetModes.UseGlobalSetting)
            {
                Write(text.AsEnumerable(), level, target);
            }
            private static Exception lastException;
            public static void Write(string preText, Exception err, LogLevel level = LogLevel.Normal, TargetModes target = TargetModes.UseGlobalSetting)
            {
                lastException = err;
                string errText;
                try
                {
                    errText = ObjectContentToStringOrType(err);
                }
                catch (Exception toTextEx)
                {
                    try
                    {
                        errText = "Failed to retrieve error message: " + toTextEx.ToString();
                    }
                    catch (Exception)
                    {
                        errText = "Fatal error retrieving error message.";
                    }
                }
                Write((preText == null ? "" : preText + " ") + errText, level, target);
            }
            public static void Write(Exception err, LogLevel level = LogLevel.Normal, TargetModes target = TargetModes.UseGlobalSetting)
            {
                if (SuppressDoubleExceptions && (err == lastException))
                {
                    return;
                }
                lastException = err;
                Write((string)null, err /*ObjectContentToStringOrType(err)*/, level, target);
            }
            public static void Write(params object[] what)
            {
                Write(what.Select(el => ObjectToString(el)));
            }
            public static void Write(LogLevel level = LogLevel.Normal, TargetModes target = TargetModes.UseGlobalSetting, params object[] what)
            {
                Write(what.Select(el => ObjectToString(el)), level, target);
            }
            public static void WriteList<T>(IEnumerable<T> collection, LogLevel level = LogLevel.Normal, TargetModes target = TargetModes.UseGlobalSetting)
            {
                var list = collection as IList<T>;
                var pre = new List<String>(1);
                if (list == null)
                {
                    pre.Add("Enumeration<" + typeof(T).FullName + ">: " + ObjectContentToString(collection));
                }
                else
                {
                    pre.Add("List<" + typeof(T).FullName + ">: " + ObjectContentToString(collection));
                }
                Write(collection == null ? pre : pre.Union(collection.Select(el => "> " + ObjectToString(el))), level, target);
            }
            public static void WriteText(string text, LogLevel level, TargetModes target)
            {
                DoWrite(text + Environment.NewLine, level, target);
            }


            /*
            public static void WriteLog(Exception err)
            {

                //short but localized version:
                WriteLog("\n\nERROR (" + DateTime.Now.ToString() + "):\n" + err.ToString());
                return;
                /*
                String error_text = null;
                var t = new System.Threading.Thread(() =>
                {
                    error_text = err.ToString();
                });
                t.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                t.Start();
                t.Join();
                WriteLog("\n\nERROR (" + DateTime.Now.ToString() + "):\n" + error_text);
                 *
                /*
                ExceptionLogger el = new ExceptionLogger(err);
                System.Threading.Thread t = new System.Threading.Thread(el.DoLog);
                t.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                t.Start();
                *
            }
            class ExceptionLogger
            {
                Exception _ex;

                public ExceptionLogger(Exception ex)
                {
                    _ex = ex;
                }

                public void DoLog()
                {
                    WriteLog("\n\nERROR (" + DateTime.Now.ToString() + "):\n" + _ex.ToString());
                    //Console.WriteLine(_ex.ToString()); //Will display en-US message
                }
            }*/

            /*private static List<Game.GUI.Controls.Label> screenLabels = new List<Game.GUI.Controls.Label>();
            public static void WriteScreen(params String[] lines)
            {
                for (var i = 0; i < lines.Length; i++)
                {
                    if (screenLabels.Count <= i)
                    {
                        var screenLabel = new Game.GUI.Controls.Label(Game.GnomanEmpire.Instance.GuiManager.Manager);
                        screenLabel.Init();
                        screenLabel.Left = 10;
                        screenLabel.Top = 80 + (i * 25);
                        screenLabel.Width = 800;
                        screenLabel.Height = 25;
                        Game.GnomanEmpire.Instance.GuiManager.Add(screenLabel);
                        screenLabels.Add(screenLabel);
                    }


                    if (lines[i] != null)
                    {
                        screenLabels[i].Text = lines[i];
                    }
                }
            }*/
        }
    }
}
