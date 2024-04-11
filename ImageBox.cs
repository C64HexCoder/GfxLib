using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GfxLib
{
    public partial class ImageBox : PictureBox
    {
        Bitmap ScaledImage;
        private int VertSBarValue = 0, HorizSBarValue = 0;
        public ScrollBar HrizontalScrollBar { get; set; }
        public ScrollBar VerticalScrollBar { get; set; }

        EventHandler HscrollBar;
        EventHandler VscrollBar;
        public ImageBox()
        {
            InitializeComponent();

        }

        public bool AutoScaleImageBox { get; set; }
        public int ScaleFactor { get; set; } = 1;
        public float ScaleFactorFloat { get; set; } = 1f;

   /*     public int ImageWidth
        {
            get {
                if (ScaledImage == null)
                    return Image.Width;
                else
                    return ScaledImage.Width;
            }

        }*/

        int ScreenWidth
        {
            get
            {
                return SystemInformation.VirtualScreen.Width;
            }
        }

        int ScreenHeight
        {
            get { return SystemInformation.VirtualScreen.Height;}
        }

        public new Image Image
        {
            get
            {
                if (ScaledImage == null)
                    return (Image)base.Image;
                else
                    return ScaledImage;
            }

            set {
                ScaledImage = null;
                base.Image = value;
            }

        }

        public int ImageHeight
        {
            get
            {
                if (ScaledImage == null)
                    return Image.Height;
                else
                    return ScaledImage.Height;
            }

        }

        public class ScrollBarEventArgs : EventArgs
        {
            public ScrollBarEventArgs()
            {
            }

            public ScrollBarEventArgs(int Maximum)
            {
                this.Maximum = Maximum;
            }

            public string Message { get; set; }
            public int Maximum { get; set; }
            public int Position { get; set; }
            public bool Enabled { get; set; }
        }

        //public delegate void ScrollBarEventHandler(object sender, ScrollBarEventArgs args);
        public delegate void VerticalScrollBarEventHundler(object sender, ScrollBarEventArgs args);
        public delegate void HorizontalScrollBarEventHundler(object sender, ScrollBarEventArgs args);

        //public event ScrollBarEventHandler ScrollBarChanged;
        public event HorizontalScrollBarEventHundler HorizontalScrollBarChanged;
        public event VerticalScrollBarEventHundler VerticalScrollBarChanged;

        public void AutoScale ()
        {
            int ScakeFactor;

            if ((float)Image.Width/Image.Height >= (float)Width/Height)
                ScaleFactor = Width/Image.Width;
            else
                ScaleFactor = Height/Image.Height;

            ScaleImage(ScaleFactor);
        }

        // Wrap event invocations inside a protected virtual method
        // to allow derived classes to override the event invocation behavior
        protected virtual void OnHorizontalBarChanged(ScrollBarEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            HorizontalScrollBarEventHundler raiseEvent = HorizontalScrollBarChanged;

            // Event will be null if there are no subscribers
            if (raiseEvent != null)
            {
                // Format the string to send inside the CustomEventArgs parameter
                e.Message += $" at {DateTime.Now}";

                // Call to raise the event.
                raiseEvent(this, e);
            }
        }



        // Wrap event invocations inside a protected virtual method
        // to allow derived classes to override the event invocation behavior
        protected virtual void OnVerticalBarChanged(ScrollBarEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            VerticalScrollBarEventHundler raiseEvent = VerticalScrollBarChanged;

            // Event will be null if there are no subscribers
            if (raiseEvent != null)
            {
                // Format the string to send inside the CustomEventArgs parameter
                e.Message += $" at {DateTime.Now}";

                // Call to raise the event.
                raiseEvent(this, e);
            }
        }
        public void ScaleImage()
        {
            Bitmap scaledImage;

            if (Image != null) {
                if (Image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format1bppIndexed || Image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format4bppIndexed || Image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                    scaledImage = new Bitmap(Image.Width * ScaleFactor, Image.Height * ScaleFactor);
                else
                    scaledImage = new Bitmap(Image.Width * ScaleFactor, Image.Height * ScaleFactor, Graphics.FromImage(Image));

                Graphics g = Graphics.FromImage(scaledImage);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.DrawImage(Image, 0, 0, scaledImage.Width, scaledImage.Height);
                g.Dispose();

                if (scaledImage.Width > Width && SizeMode != PictureBoxSizeMode.AutoSize)
                {
                    Image = new Bitmap(Width, Height);
                    ScaledImage = scaledImage;
                    DrawImagePart(0, 0);

                    ScrollBarEventArgs EArg = new ScrollBarEventArgs();
                    EArg.Position = 0;
                    EArg.Enabled = true;
                    EArg.Maximum = scaledImage.Height - Height;
                    OnVerticalBarChanged(EArg);

                    EArg.Maximum = scaledImage.Width - Width;
                    OnHorizontalBarChanged(EArg);
                }
                else
                {
                    Image = scaledImage;

                    ScrollBarEventArgs EArg = new ScrollBarEventArgs();
                    EArg.Position = 0;
                    EArg.Enabled = false;
                    EArg.Maximum = 0;
                    OnVerticalBarChanged(EArg);
                    OnHorizontalBarChanged(EArg);
                }
                //Refresh();

}        }

        public void ScaleImage(int scaleFactor)
        {
            Bitmap scaledImage;

            if (Image != null)
            {
                if (Image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format1bppIndexed || Image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format4bppIndexed || Image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                    scaledImage = new Bitmap (Image.Width * scaleFactor, Image.Height * scaleFactor);
                else
                    scaledImage = new Bitmap(Image.Width * scaleFactor, Image.Height * scaleFactor, Graphics.FromImage(Image));

                Graphics g = Graphics.FromImage(scaledImage);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.DrawImage(Image, 0, 0, scaledImage.Width, scaledImage.Height);
                g.Dispose();

                if (scaledImage.Width > Width && SizeMode != PictureBoxSizeMode.AutoSize)
                {
                    Image = new Bitmap(Width, Height);
                    ScaledImage = scaledImage;
                    DrawImagePart(0,0);

                    ScrollBarEventArgs EArg = new ScrollBarEventArgs();
                    EArg.Position = 0;
                    EArg.Enabled = true;
                    EArg.Maximum = scaledImage.Height - Height;
                    OnVerticalBarChanged(EArg);

                    EArg.Maximum = scaledImage.Width - Width;
                    OnHorizontalBarChanged(EArg);

                }
                else
                {
                    Image = scaledImage;

                    ScrollBarEventArgs EArg = new ScrollBarEventArgs();
                    EArg.Position = 0;
                    EArg.Enabled = false;
                    EArg.Maximum = 0;
                    OnVerticalBarChanged(EArg);
                    OnHorizontalBarChanged(EArg);
                }
                //Refresh();

            }
        }

        public void ScaleImage(int scaleFactor, bool autoSize = true)
        {
            Bitmap scaledImage;

            if (Image != null)
            {
                if (Image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format1bppIndexed || Image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format4bppIndexed || Image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                    scaledImage = new Bitmap(Image.Width * scaleFactor, Image.Height * scaleFactor);
                else
                    scaledImage = new Bitmap(Image.Width * scaleFactor, Image.Height * scaleFactor, Graphics.FromImage(Image));

                Graphics g = Graphics.FromImage(scaledImage);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.DrawImage(Image, 0, 0, scaledImage.Width, scaledImage.Height);
                g.Dispose();

                if (!autoSize)
                {
                    Image = new Bitmap(Width, Height);
                    ScaledImage = scaledImage;
                    DrawImagePart(0, 0);

                    ScrollBarEventArgs EArg = new ScrollBarEventArgs();
                    EArg.Position = 0;
                    EArg.Enabled = true;
                    EArg.Maximum = scaledImage.Height - Height;
                    OnVerticalBarChanged(EArg);

                    EArg.Maximum = scaledImage.Width - Width;
                    OnHorizontalBarChanged(EArg);

                }
                else
                {
                    Image = scaledImage;

                    ScrollBarEventArgs EArg = new ScrollBarEventArgs();
                    EArg.Position = 0;
                    EArg.Enabled = false;
                    EArg.Maximum = 0;
                    OnVerticalBarChanged(EArg);
                    OnHorizontalBarChanged(EArg);
                }
                //Refresh();

            }
        }

        public void ScaleImage(float scaleFactor, bool autoSize = true)
        {
            Bitmap scaledImage;

            if (Image != null)
            {
                if (Image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format1bppIndexed || Image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format4bppIndexed || Image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                    scaledImage = new Bitmap((int)(Image.Width * scaleFactor), (int)(Image.Height * scaleFactor));
                else
                    scaledImage = new Bitmap((int)(Image.Width * scaleFactor), (int)(Image.Height * scaleFactor), Graphics.FromImage(Image));

                Graphics g = Graphics.FromImage(scaledImage);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.DrawImage(Image, 0, 0, scaledImage.Width, scaledImage.Height);
                g.Dispose();

                if (!autoSize)
                {
                    Image = new Bitmap(Width, Height);
                    ScaledImage = scaledImage;
                    DrawImagePart(0, 0);


                    ScrollBarEventArgs EArg = new ScrollBarEventArgs();
                    EArg.Position = 0;
                    EArg.Enabled = true;
                    EArg.Maximum = scaledImage.Height - Height;
                    OnVerticalBarChanged(EArg);

                    EArg.Maximum = scaledImage.Width - Width;
                    OnHorizontalBarChanged(EArg);
                }
                else
                {
                    Image = scaledImage;

                    ScrollBarEventArgs EArg = new ScrollBarEventArgs();
                    EArg.Position = 0;
                    EArg.Enabled = false;
                    EArg.Maximum = 0;
                    OnVerticalBarChanged(EArg);
                    OnHorizontalBarChanged(EArg);
                }                //Refresh();

            }
        }

        public void ScaleToMax(float percentageOfScreen)
        {
            float scaleFactor;


            if (Image != null)
            {
                if (Image.Width > Image.Height)
                {
                    scaleFactor = ScreenWidth*0.8f / Image.Width;
                }
                else { scaleFactor = ScreenHeight*0.8f / Image.Height; }

                Bitmap scaledImage = new Bitmap((int)(Image.Width * scaleFactor),(int)( Image.Height * scaleFactor), Graphics.FromImage(Image));
                Graphics g = Graphics.FromImage(scaledImage);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.DrawImage(Image, 0, 0, scaledImage.Width, scaledImage.Height);
                g.Dispose();

     
               Image = scaledImage;
                //Refresh();

            }
        }


        public void ScaleToHeight ()
        {
            float ScaleFactor = Height / Image.Height; 
        }

        public void DrawImagePart(int Xpos, int Ypos)
        {
            if (ScaledImage == null)
                return;
            
            Rectangle destRec = new Rectangle(0, 0, Width, Height);
            Rectangle srcRec = new Rectangle(Xpos, Ypos, Width, Height);

            Graphics graphics = Graphics.FromImage(base.Image);
            graphics.Clear(Color.FromArgb(0));
            graphics.DrawImage(ScaledImage, destRec, Xpos, Ypos, Width, Height, GraphicsUnit.Pixel);
            //graphics.DrawImage(ScaledImage, 0, 0, srcRec, GraphicsUnit.Pixel);
            graphics.Dispose();
            Invalidate();

           // ScrollBarEventArgs VerticalBarEventArgs = new ScrollBarEventArgs("Vertical changed");
            //ScrollBarEventArgs HorizotalBarEventArgs = new ScrollBarEventArgs("Horizontal changed");

            //OnHorizontalBarChanged(HorizotalBarEventArgs);
            //OnVerticalBarChanged(VerticalBarEventArgs);
            
        }

        public void ScaleImage(float scaleFactor)
        {
            if (Image != null)
            {
                Bitmap scaledImage = new Bitmap((int)(Image.Width * scaleFactor), (int)(Image.Height * scaleFactor));
                scaledImage.SetResolution(Image.HorizontalResolution, Image.VerticalResolution);
                Graphics g = Graphics.FromImage(scaledImage);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.DrawImage(Image, 0, 0, scaledImage.Width, scaledImage.Height);
                g.Dispose();
                Image = scaledImage;
                //Refresh();

            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            base.OnPaint(e);
        }
    }
}
