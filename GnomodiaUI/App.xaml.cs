using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Gnomodia;
using ICSharpCode.SharpZipLib.Zip;

namespace GnomodiaUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            using (XnbUtilities xnbUtilities = new XnbUtilities(Path.GetTempPath()))
            {
                using (ZipFile skinZip = new ZipFile(Path.Combine(Reference.GameDirectory.FullName, @"Content\UI\Default.skin")))
                {
                    LoadZipXnbResource(xnbUtilities, skinZip, "Images/Window.Caption.xnb", "WindowCaption");
                    LoadZipXnbResource(xnbUtilities, skinZip, "Images/Window.CloseButton.xnb", "WindowCloseButton");
                    LoadZipXnbResource(xnbUtilities, skinZip, "Images/Window.FrameBottom.xnb", "WindowFrameBottom");
                    LoadZipXnbResource(xnbUtilities, skinZip, "Images/Window.FrameLeft.xnb", "WindowFrameLeft");
                    LoadZipXnbResource(xnbUtilities, skinZip, "Images/Window.FrameRight.xnb", "WindowFrameRight");
                }
            }

            Interface.GnomodiaLauncherWindow window = new Interface.GnomodiaLauncherWindow();
            window.Show();
        }

        private void LoadZipXnbResource(XnbUtilities xnbUtilities, ZipFile zipFile, string xnbFile, string resourceName)
        {
            ZipEntry entry = zipFile.GetEntry(xnbFile);
            Stream s = zipFile.GetInputStream(entry);
            string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".xnb");
            using (Stream tmpStream = File.OpenWrite(tempFile))
            {
                s.CopyTo(tmpStream);
            }
            BitmapImage image = xnbUtilities.LoadTextureAsImage(tempFile);
            Resources.Add(resourceName, image);
            File.Delete(tempFile);
        }
    }
}
