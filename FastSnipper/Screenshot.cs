using System;
using System.Drawing;
using System.Windows;

namespace FastSnipper
{
    public class Screenshot
    {
        #region Fields

        private System.Windows.Point startPosition;
        private int width;
        private int height;

        #endregion

        #region Properties

        public int Width
        {
            get { return this.width; }
            set
            {
                if (value < 0) throw new ArgumentException(nameof(this.Width) + " must be greater than or equal to zero");
                this.width = value;
            }
        }

        public int Height
        {
            get { return this.height; }
            set
            {
                if (value < 0) throw new ArgumentException(nameof(this.Height) + " must be greater than or equal to zero");
                this.height = value;
            }
        }

        public System.Windows.Point StartPosition
        {
            get { return this.startPosition; }
            set
            {
                if (value.X < 0 || value.Y < 0) throw new ArgumentException("coordinates of " + nameof(this.StartPosition) + " must be greater than or equal to zero");
                this.startPosition = value;
            }
        }

        #endregion

        #region Constructors

        public Screenshot(int width, int height, System.Windows.Point startPosition)
        {
            this.Width         = width;
            this.Height        = height;
            this.StartPosition = startPosition;
        }

        public Screenshot(int width, int height, int x, int y)
            : this(width, height, new System.Windows.Point(x,y))
        { }

        #endregion

        #region Methods

        public void Capture()
        {
            using (Bitmap b = new Bitmap(this.Width, this.Height))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.CopyFromScreen((int)this.startPosition.X,
                                     (int)this.StartPosition.Y,
                                     0,
                                     0,
                                     b.Size,
                                     CopyPixelOperation.SourceCopy);
                }
                System.Windows.Forms.Clipboard.SetImage(b);
            }
        }

        #endregion

    }
}
