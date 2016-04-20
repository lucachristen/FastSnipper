using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using System.Windows.Threading;

namespace FastSnipper
{
    /// <summary>
    /// Interaction logic for FastSnipperWindow.xaml
    /// </summary>
    public partial class FastSnipperWindow : Window
    {
        #region Fields

        private bool             captureMode;
        private CaptureRectangle captureRect;

        #endregion

        #region Constructors

        public FastSnipperWindow()
        {
            InitializeComponent();
            this.captureMode = false;
            this.captureRect = new CaptureRectangle(this.RectCanvas);
        }

        #endregion

        #region Methods

        #region Events

        private void FastSnipper_Loaded(object sender, RoutedEventArgs e)
        {
            PresentationSource source = PresentationSource.FromVisual(this);

            this.captureRect.DpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
            this.captureRect.DpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            captureMode               = true;
            captureRect.StartPosition = Mouse.GetPosition(Application.Current.MainWindow);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (captureMode)
            {
               captureRect.Update(e.GetPosition(RectCanvas));
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.captureMode             = false;
            this.captureRect.EndPosition = Mouse.GetPosition(Application.Current.MainWindow);

            Dispatcher.Invoke(() =>
            {
                this.captureRect.MakeTransparent();
                this.Background.Opacity = 0;
            }, DispatcherPriority.Render);

            Application.Current.Shutdown();
        }

        private void FastSnipper_Closed(object sender, EventArgs e)
        {
            new Screenshot(this.captureRect.WidthInPixel,
                           this.captureRect.HeightInPixel,
                           this.captureRect.UpperLeftCornerInPixel).Capture();
        }

        #endregion

        #endregion
    }
}
