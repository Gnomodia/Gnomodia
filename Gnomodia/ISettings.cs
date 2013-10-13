/*
 * We need this silly little trick because Settings change from one app domain to the other.
 * By having the Settings class implement the ISettings interface, we can do a special
 * trick in the Reference class that allows it to read setting through a MarshalByRef proxy
 * in the original app domain.
 */

namespace Gnomodia
{
    internal interface ISettings
    {
        string GnomoriaInstallationPath { get; set; }
    }
}

namespace Gnomodia.Properties
{
    partial class Settings : ISettings { }
}
