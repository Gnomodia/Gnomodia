using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GnomodiaUI.Interface
{
    public class GnomoriaWindow : Window
    {
        #region Click events

        protected void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

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
            topLeftWindowDecoration.Source = (ImageSource)Application.Current.Resources["WindowCaption"];

            Image topMiddleWindowDecoration = GetTemplateChild("topMiddleWindowDecoration") as Image;
            topMiddleWindowDecoration.Source = topLeftWindowDecoration.Source;

            closeButton.ApplyTemplate();
            Image closeButtonDecoration = closeButton.Template.FindName("closeButtonDecoration", closeButton) as Image;
            closeButtonDecoration.Source = (ImageSource)Application.Current.Resources["WindowCloseButton"];

            Image leftMiddleWindowDecoration = GetTemplateChild("leftMiddleWindowDecoration") as Image;
            leftMiddleWindowDecoration.Source = (ImageSource)Application.Current.Resources["WindowFrameLeft"];

            Image rightMiddleWindowDecoration = GetTemplateChild("rightMiddleWindowDecoration") as Image;
            rightMiddleWindowDecoration.Source = leftMiddleWindowDecoration.Source;

            Image bottomLeftWindowDecoration = GetTemplateChild("bottomLeftWindowDecoration") as Image;
            bottomLeftWindowDecoration.Source = (ImageSource)Application.Current.Resources["WindowFrameBottom"];

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
