/*
 *  Gnomodia UI Controls
 *
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

using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.SharpZipLib.Zip;

namespace GnomodiaUI.Controls
{
    public class GnomoriaTabControl : TabControl
    {
        static GnomoriaTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GnomoriaTabControl),
                new FrameworkPropertyMetadata(typeof(GnomoriaTabControl)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new GnomoriaTabItem();
        }

        public override void OnApplyTemplate()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
                using (ZipFile skinZip = new ZipFile(Path.Combine(GnomodiaControls.GnomoriaPath, @"Content\UI\Default.skin")))
                {
                    using (XnbUtilities xnbUtilities = new XnbUtilities(skinZip))
                    {
                        Resources["TabControl"] = xnbUtilities.LoadTextureAsImage("Images/TabControl");
                    }
                }

            Image leftMiddleTabDecoration = GetTemplateChild("leftMiddleTabDecoration") as Image;
            leftMiddleTabDecoration.Source = (ImageSource)Resources["TabControl"];

            Image rightMiddleTabDecoration = GetTemplateChild("rightMiddleTabDecoration") as Image;
            rightMiddleTabDecoration.Source = (ImageSource)Resources["TabControl"];

            Image bottomLeftTabDecoration = GetTemplateChild("bottomLeftTabDecoration") as Image;
            bottomLeftTabDecoration.Source = (ImageSource)Resources["TabControl"];

            Image bottomRightTabDecoration = GetTemplateChild("bottomRightTabDecoration") as Image;
            bottomRightTabDecoration.Source = (ImageSource)Resources["TabControl"];

            Image bottomMiddleTabDecoration = GetTemplateChild("bottomMiddleTabDecoration") as Image;
            bottomMiddleTabDecoration.Source = (ImageSource)Resources["TabControl"];

            Image topLeftTabDecoration = GetTemplateChild("topLeftTabDecoration") as Image;
            topLeftTabDecoration.Source = (ImageSource)Resources["TabControl"];

            Image topRightTabDecoration = GetTemplateChild("topRightTabDecoration") as Image;
            topRightTabDecoration.Source = (ImageSource)Resources["TabControl"];

            Image topMiddleTabDecoration = GetTemplateChild("topMiddleTabDecoration") as Image;
            topMiddleTabDecoration.Source = (ImageSource)Resources["TabControl"];
        }
    }

    public class GnomoriaTabItem : TabItem
    {
        static GnomoriaTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GnomoriaTabItem),
                new FrameworkPropertyMetadata(typeof(GnomoriaTabItem)));
        }

        public override void OnApplyTemplate()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
                using (ZipFile skinZip = new ZipFile(Path.Combine(GnomodiaControls.GnomoriaPath, @"Content\UI\Default.skin")))
                {
                    using (XnbUtilities xnbUtilities = new XnbUtilities(skinZip))
                    {
                        Resources["TabControlHeader"] = xnbUtilities.LoadTextureAsImage("Images/TabControl.Header");
                    }
                }

            int tabIndex = ItemsControl.ItemsControlFromItemContainer(this).ItemContainerGenerator.IndexFromContainer(this);

            Image leftTabDecoration = GetTemplateChild("leftTabDecoration") as Image;
            leftTabDecoration.Source = (ImageSource)Resources["TabControlHeader"];

            Image middleTabDecoration = GetTemplateChild("middleTabDecoration") as Image;
            middleTabDecoration.Source = (ImageSource)Resources["TabControlHeader"];

            Image rightTabDecoration = GetTemplateChild("rightTabDecoration") as Image;
            rightTabDecoration.Source = (ImageSource)Resources["TabControlHeader"];
        }
    }
}
