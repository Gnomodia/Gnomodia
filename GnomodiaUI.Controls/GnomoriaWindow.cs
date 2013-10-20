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

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using ICSharpCode.SharpZipLib.Zip;
using Path = System.IO.Path;

namespace GnomodiaUI.Controls
{
    public class GnomoriaWindow : Window
    {
        #region Click events

        protected void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        static GnomoriaWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GnomoriaWindow),
                new FrameworkPropertyMetadata(typeof(GnomoriaWindow)));
        }

        public GnomoriaWindow()
        {
            PreviewMouseMove += OnPreviewMouseMove;
        }

        protected void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
                Cursor = Cursors.Arrow;
        }

        public override void OnApplyTemplate()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
                using (ZipFile skinZip = new ZipFile(Path.Combine(GnomodiaControls.GnomoriaPath, @"Content\UI\Default.skin")))
                {
                    using (XnbUtilities xnbUtilities = new XnbUtilities(skinZip))
                    {
                        Resources["WindowCaption"] = xnbUtilities.LoadTextureAsImage("Images/Window.Caption");
                        Resources["WindowCloseButton"] = xnbUtilities.LoadTextureAsImage("Images/Window.CloseButton");
                        Resources["WindowFrameBottom"] = xnbUtilities.LoadTextureAsImage("Images/Window.FrameBottom");
                        Resources["WindowFrameLeft"] = xnbUtilities.LoadTextureAsImage("Images/Window.FrameLeft");
                        Resources["WindowFrameRight"] = xnbUtilities.LoadTextureAsImage("Images/Window.FrameRight");
                    }
                }

            Button closeButton = GetTemplateChild("closeButton") as Button;
            if (closeButton != null)
                closeButton.Click += CloseClick;

            Grid resizeGrid = GetTemplateChild("resizeGrid") as Grid;
            if (resizeGrid != null)
            {
                foreach (Rectangle resizeRectangle in resizeGrid.Children.OfType<Rectangle>())
                {
                    resizeRectangle.PreviewMouseDown += ResizeRectangle_PreviewMouseDown;
                    resizeRectangle.MouseMove += ResizeRectangle_MouseMove;
                }
            }

            Rectangle moveRectangle = GetTemplateChild("moveRectangle") as Rectangle;
            if (moveRectangle != null)
                moveRectangle.PreviewMouseDown += moveRectangle_PreviewMouseDown;

            Image topLeftWindowDecoration = GetTemplateChild("topLeftWindowDecoration") as Image;
            topLeftWindowDecoration.Source = (ImageSource)Resources["WindowCaption"];

            Image topMiddleWindowDecoration = GetTemplateChild("topMiddleWindowDecoration") as Image;
            topMiddleWindowDecoration.Source = topLeftWindowDecoration.Source;

            closeButton.ApplyTemplate();
            Image closeButtonDecoration = closeButton.Template.FindName("closeButtonDecoration", closeButton) as Image;
            closeButtonDecoration.Source = (ImageSource)Resources["WindowCloseButton"];

            Image leftMiddleWindowDecoration = GetTemplateChild("leftMiddleWindowDecoration") as Image;
            leftMiddleWindowDecoration.Source = (ImageSource)Resources["WindowFrameLeft"];

            Image rightMiddleWindowDecoration = GetTemplateChild("rightMiddleWindowDecoration") as Image;
            rightMiddleWindowDecoration.Source = leftMiddleWindowDecoration.Source;

            Image bottomLeftWindowDecoration = GetTemplateChild("bottomLeftWindowDecoration") as Image;
            bottomLeftWindowDecoration.Source = (ImageSource)Resources["WindowFrameBottom"];

            Image bottomMiddleWindowDecoration = GetTemplateChild("bottomMiddleWindowDecoration") as Image;
            bottomMiddleWindowDecoration.Source = bottomLeftWindowDecoration.Source;

            Image bottomMiddleWindowDecoration2 = GetTemplateChild("bottomMiddleWindowDecoration2") as Image;
            bottomMiddleWindowDecoration2.Source = bottomLeftWindowDecoration.Source;

            Image bottomRightWindowDecoration = GetTemplateChild("bottomRightWindowDecoration") as Image;
            bottomRightWindowDecoration.Source = bottomLeftWindowDecoration.Source;

            base.OnApplyTemplate();
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        protected void ResizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            switch (rectangle.Name)
            {
                case "top":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "bottom":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "left":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "right":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "topLeft":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "topRight":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "bottomLeft":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                case "bottomRight":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;
            }
        }

        private HwndSource _hwndSource;

        protected override void OnInitialized(EventArgs e)
        {
            SourceInitialized += OnSourceInitialized;
            base.OnInitialized(e);
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            _hwndSource = (HwndSource)PresentationSource.FromVisual(this);
        }

        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(_hwndSource.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        private enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

        private void moveRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        protected void ResizeRectangle_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            switch (rectangle.Name)
            {
                case "top":
                    Cursor = Cursors.SizeNS;
                    break;
                case "bottom":
                    Cursor = Cursors.SizeNS;
                    break;
                case "left":
                    Cursor = Cursors.SizeWE;
                    break;
                case "right":
                    Cursor = Cursors.SizeWE;
                    break;
                case "topLeft":
                    Cursor = Cursors.SizeNWSE;
                    break;
                case "topRight":
                    Cursor = Cursors.SizeNESW;
                    break;
                case "bottomLeft":
                    Cursor = Cursors.SizeNESW;
                    break;
                case "bottomRight":
                    Cursor = Cursors.SizeNWSE;
                    break;
            }
        }
    }
}
