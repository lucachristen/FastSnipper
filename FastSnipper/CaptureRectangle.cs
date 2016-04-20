using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FastSnipper
{
    public class CaptureRectangle
    {
        #region Fields

        private double    dpiX;
        private double    dpiY;
        private Point     startPosition;
        private Point     endPosition;
        private Canvas    canvas;
        private Rectangle rectangle;
        
        #endregion

        #region Properties

        public double DpiX
        {
            get { return this.dpiX; }
            set
            {
                if (value < 0) throw new ArgumentException(nameof(this.DpiX) + " must be greater than or equal to zero");
                this.dpiX = value;
            }
        }

        public double DpiY
        {
            get { return this.dpiY; }
            set
            {
                if (value < 0) throw new ArgumentException(nameof(this.DpiY) + " must be greater than or equal to zero");
                this.dpiY = value;
            }
        }

        public Point StartPosition
        {
            get { return this.startPosition; }
            set
            {
                if (value.X < 0 || value.Y < 0) throw new ArgumentException("coordinates of " + nameof(this.StartPosition) + " must be greater than or equal to zero");
                this.startPosition = value;
            }
        }

        public Point EndPosition
        {
            get { return this.endPosition; }
            set
            {
                if (value.X < 0 || value.Y < 0) throw new ArgumentException("coordinates of " + nameof(this.EndPosition) + " must be greater than or equal to zero");
                this.endPosition = value;
            }
        }

        public double Width        => Math.Abs(this.StartPosition.X - this.EndPosition.X);
        public int    WidthInPixel => this.GetPixel(this.Width, this.DpiX);

        public double Height        => Math.Abs(this.StartPosition.Y - this.EndPosition.Y);
        public int    HeightInPixel => this.GetPixel(this.Height, this.DpiY);

        public Point UpperLeftCorner        => this.GetUpperLeftCorner(this.StartPosition, this.EndPosition);
        public Point UpperLeftCornerInPixel => this.GetUpperLeftCorner(this.StartPosition, this.EndPosition);

        public Canvas Canvas => this.canvas; 

        #endregion

        #region Constructors

        public CaptureRectangle(Canvas canvas)
        {
            this.canvas    = canvas;
            this.rectangle = new Rectangle
            {
                Stroke          = Brushes.Red,
                StrokeThickness = 1,
                Height          = 0,
                Width           = 0
            };

            this.canvas.Children.Add(this.rectangle);
        }

        #endregion

        #region Methods

        public void Update(Point newEndPosition)
        {
            this.EndPosition = newEndPosition;

            var x = Math.Min(this.EndPosition.X, this.StartPosition.X);
            var y = Math.Min(this.EndPosition.Y, this.StartPosition.Y);

            this.rectangle.Width  = Math.Max(this.EndPosition.X, this.StartPosition.X) - x;
            this.rectangle.Height = Math.Max(this.EndPosition.Y, this.StartPosition.Y) - y;

            Canvas.SetLeft(this.rectangle, x);
            Canvas.SetTop(this.rectangle, y);
        }

        public void MakeTransparent()
        {
            this.rectangle.Stroke = Brushes.Transparent;
        }

        private int GetPixel(double pos, double dpi)
        {
            double pixel = pos / 72.0 * dpi;   // 72 points per inch
            pixel = pixel / (1.0 / 3.0 + 1.0); // fix calculation //TODO find out, why calculation is 33% greater than correct value
            return (int)(pixel + 0.5);         // 0.5 for rounding
        }

        private int GetYPixel(Point point)
        {
            return GetPixel(point.Y, this.DpiY);
        }

        private int GetXPixel(Point point)
        {
            return GetPixel(point.X, this.DpiX);
        }

        private Point GetPointInPixel(Point p)
        {
            return new Point(GetXPixel(p), GetYPixel(p));
        }

        private Point GetUpperLeftCorner(Point p1, Point p2)
        {
            double x = Math.Min(p1.X, p2.X);
            double y = Math.Min(p1.Y, p2.Y);
            return new Point(x, y);
        }

        #endregion

    }
}
