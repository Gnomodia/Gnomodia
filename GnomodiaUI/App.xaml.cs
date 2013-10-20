using System.Windows;
using Gnomodia;
using GnomodiaUI.Controls;
using GnomodiaUI.Interface;

namespace GnomodiaUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            GnomodiaControls.GnomoriaPath = Reference.GameDirectory.FullName;

            GnomodiaLauncherWindow window = new GnomodiaLauncherWindow();
            window.Show();
        }
    }
}
