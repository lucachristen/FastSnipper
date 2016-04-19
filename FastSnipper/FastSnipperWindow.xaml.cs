using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Drawing.Imaging;
using System.Windows.Controls;
using System;
using System.Windows.Threading;
using System.Threading;

namespace FastSnipper
{
    /// <summary>
    /// Interaction logic for FastSnipperWindow.xaml
    /// </summary>
    public partial class FastSnipperWindow : Window
    {
        #region Fields

        private System.Windows.Point startPos;
        private System.Windows.Point endPos;

        private bool captureMode = false;

        private System.Windows.Shapes.Rectangle currentRect = new System.Windows.Shapes.Rectangle
                                                              {
                                                                  Stroke = System.Windows.Media.Brushes.Red,
                                                                  StrokeThickness = 1,
                                                                  Height = 0,
                                                                  Width = 0
                                                              };

        private double dpiX = 0;
        private double dpiY = 0;

        #endregion

        #region Constructors

        public FastSnipperWindow()
        {
            InitializeComponent();
        }

        #endregion

        private void FastSnipper_Loaded(object sender, RoutedEventArgs e)
        {
            PresentationSource source = PresentationSource.FromVisual(this);

            dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
            dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            captureMode = true;
            startPos    = Mouse.GetPosition(Application.Current.MainWindow);

            Canvas.SetLeft(RectCanvas, startPos.X);
            Canvas.SetTop(RectCanvas, startPos.Y);
            this.RectCanvas.Children.Add(currentRect);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (captureMode)
            {
                var pos = e.GetPosition(RectCanvas);

                var x = Math.Min(pos.X, startPos.X);
                var y = Math.Min(pos.Y, startPos.Y);

                currentRect.Width  = Math.Max(pos.X, startPos.X) - x;
                currentRect.Height = Math.Max(pos.Y, startPos.Y) - y;

                Canvas.SetLeft(currentRect, x);
                Canvas.SetTop(currentRect, y);
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            captureMode = false;
            endPos      = Mouse.GetPosition(Application.Current.MainWindow);

            Dispatcher.Invoke(() =>
            {
                currentRect.Stroke = System.Windows.Media.Brushes.Transparent;
                currentRect.StrokeThickness = 0;
                RectCanvas.Opacity = 0;
                this.Background.Opacity = 0;
            }, DispatcherPriority.Render);

            System.Windows.Application.Current.Shutdown();
        }

        private void CaptureScreenshot()
        {
            using (Bitmap screenshot = new Bitmap(GetPixel(Math.Abs(startPos.X - endPos.X), dpiX),
                                                  GetPixel(Math.Abs(startPos.Y - endPos.Y), dpiY)))
            {
                using (Graphics g = Graphics.FromImage(screenshot))
                {
                    var pos = GetLeftMostPoint(startPos, endPos);
                    g.CopyFromScreen(GetXPixel(pos),
                                     GetYPixel(pos),
                                     0,
                                     0,
                                     screenshot.Size,
                                     CopyPixelOperation.SourceCopy);
                }
                System.Windows.Forms.Clipboard.SetImage(screenshot);
            }
        }

        private int GetPixel(double pos, double dpi)
        {
            return (int)((pos * dpi / 72.0 / (1.0/3.0 + 1.0)) + 0.5);
        }

        private int GetYPixel(System.Windows.Point point)
        {
            return GetPixel(point.Y, dpiY);
        }

        private int GetXPixel(System.Windows.Point point)
        {
            return GetPixel(point.X, dpiX);
        }

        private System.Windows.Point GetLeftMostPoint(System.Windows.Point p1, System.Windows.Point p2)
        {
            double x = Math.Min(p1.X, p2.X);
            double y = Math.Min(p1.Y, p2.Y);
            return new System.Windows.Point(x, y);
        }

        private void FastSnipper_Closed(object sender, EventArgs e)
        {
            CaptureScreenshot();
        }
    }
}
