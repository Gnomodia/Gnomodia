using System;
using System.ComponentModel;
using System.Windows;

namespace GnomodiaUI.Controls
{
    public static class GnomodiaControls
    {
        private static string s_GnomoriaPath;
        public static string GnomoriaPath
        {
            get
            {
                if (string.IsNullOrEmpty(s_GnomoriaPath) && !DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                    throw new InvalidOperationException("Cannot use GnomodiaUI.Controls without first setting the GnomodiaControls.GnomoriaPath setting!");

                return s_GnomoriaPath;
            }
            set { s_GnomoriaPath = value; }
        }
    }
}
