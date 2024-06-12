using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace GfxLib
{
    public partial class ImageGrid : UserControl
    {
        private int _XCels = 16, _YCels = 16;
        private int _LineWidth = 2;
        private int CellWidth, CellHeight;
        private Bitmap _GridImage;
        private Color _DrawColor = Color.Blue;
        private bool _KeepAspectRatio = true;
        private DisplayModes _DisplayMode;
        public Color _BackgroundColor = Color.Transparent;
        //public bool _CellBorder = true;
        private CellSize _CellAize;
        private int _CellWidthHeight = 8;
        private bool _EnableDrawing = true;

        [Description("The Width and height of each cell when in CellSizing.Fixxed mode"),Category("Grid")]
        public int CellWIdthHeight
        {
            get => _CellWidthHeight;
            set => _CellWidthHeight = value;
        }

        [Description("If true then Drawing with the mouse is possible"),Category("Grid")]
        public bool EnableDrawing
        {
            get { return _EnableDrawing; }
            set { _EnableDrawing = value; }
        }

        [Description("Used to Indicate which Cell size to use. Fixed Cell Size in which we decide what is the cell size or a dinamic " +
            "in which the cell size decided automatically by devideing the width and heights of the image grid border box"), Category("Grid")]
        public enum CellSize
        {
            Fixed,
            Dinamic
        }

        public CellSize CellSizing
        {
            get => _CellAize;
            set => _CellAize = value;
        }
        public enum ResizeTypes
        {
            None,
            ResizeWidthToFit,
            ResizeHeightToFit
        }

        [Description("Choose whether the image in the grid will be in color or in mono"), Category("Grid")]
        public enum DisplayModes
        {
            Color,
            Mono
        }

        private ResizeTypes _ResizeToFitCells;

        [Description("NONE; The Grid will not resize to keep aspect ratio of the cells (Keep them rectangle shaped)\n\n" +
            "Horizontal: Scale horizontally to keep Cells aspect ratio\n\n" +
            "Vertical: Scale Vertically to keep Cells aspect ratio"),Category("Grid")]
        public ResizeTypes ResizeToFitCells
        {
            get => _ResizeToFitCells;
            set => _ResizeToFitCells = value;
        }


        public DisplayModes DisplayMode
        {
            get => _DisplayMode;
            set => _DisplayMode = value;
        }
        [Description("Use to SET or DISABLE The Grid Border"),Category("Grid")]
        public bool CellBorder { get; set; } = true;

        [Description("The color used to Fill the Cells when Drawing with the mouse" +
            "Or when drawing an image into the grid using Mono mode"),Category("Grid")]
        public Color DrawColor {
            get  => _DrawColor;
            set
            {
                  _DrawColor = value;
            }
        }

        Graphics gfx;

        [Description("Define the Grid line Width"),Category("Grid")]
        public int LineWidth { 
            get => _LineWidth; 
            set
            {
                _LineWidth = value;
                Refresh();
            }
        
        }

        [Description("The number of Cells in a Raw"),Category("Grid")]
        public int CellsOnX
        {
            get => _XCels;
            set
            {
                _XCels = value;
               // CellWidth = (Width) / CellsOnX;
              
                Refresh();
            }
        }

        [Description("The number of Raws"),Category("Grid")]
        public int CellsOnY
        {
            get => _YCels;
            set
            {
                _YCels = value;
                //CellHeight = (Height) / CellsOnY;
                Refresh();
            }
        }

        [Description("The image to be displayed in the Grid"),Category("Grid")]
        public Bitmap GridImage
        {
            get => _GridImage;
            set
            {
                _GridImage = value;
                if (value != null)
                {
                    DrawImage(value);
                }
            } 


        }

        [Description("Define if to Keep the aspect resio of the Cells"),Category("Grid")]
        public bool KeepAspectRatio
        {
            get => _KeepAspectRatio;
            set => _KeepAspectRatio = value;
        }

        [Description("The background color of the image.\n" +
            "This used to filter the background in MONO mode"),Category("Grid")]
        public Color BackgroundColor
        {
            get => _BackgroundColor;
            set => _BackgroundColor = value;
        }

        /// <summary>
        /// Set the Dimention of the Cells
        /// </summary>
        /// <param name="width">Set the Width of the Cells</param>
        /// <param name="height">Set the Height of the Cells</param>
        public void SetCellDimention (int width, int height)
        {
            CellWidth = width;
            CellHeight = height;

        }
        
        /// <summary>
        /// Sets the resolution of the Grid.
        /// </summary>
        /// <param name="cellsPerLines">Set the number of Cells in each line.</param>
        /// <param name="heightInCells">Set the number of lines in the Grid</param>
        public void SetResolution (int cellsPerLines, int heightInCells)
        {
            Width = CellWidth * cellsPerLines;
            Height = CellHeight * heightInCells;
        }

        /// <summary>
        /// Sets the Resolution of the grid and the Cells Width and Height.
        /// </summary>
        /// <param name="cellsPerLines">Set the number of Cells per line</param>
        /// <param name="heightInCells">Set the number of lines</param>
        /// <param name="cellWidth">Set the width of the Cells</param>
        /// <param name="cellHeight">Set the height of the Cells</param>
        public void SetResolution (int cellsPerLines, int heightInCells,int cellWidth,int cellHeight)
        {
            CellWidth= cellWidth;
            CellHeight= cellHeight;

            CellsOnX= cellsPerLines;
            CellsOnY = heightInCells;

            Width = CellWidth * cellsPerLines;
            Height = CellHeight * heightInCells;
        }

        private void ImageGrid_Paint(object sender, PaintEventArgs e)
        {
            //CellWidth = (int)Math.Ceiling((decimal)Width / CellsOnX);

            switch (CellSizing) {
                case CellSize.Fixed:
                    CellWidth = CellHeight = CellWIdthHeight;
                    Width = CellWidth * CellsOnX + _LineWidth;
                    Height = CellHeight * CellsOnY + _LineWidth;
                    break;

                case CellSize.Dinamic:

                    CellWidth = (Width / CellsOnX) == 0 ? 1 : Width / CellsOnX;       // if Widrg smaller then cellOnX its softlock

                    if (_KeepAspectRatio)
                    {
                        CellHeight = CellWidth;
                        Height = CellHeight * CellsOnY + _LineWidth;
                    }
                    else
                        CellHeight = Height / CellsOnY == 0 ? 1 : Height / CellsOnY;
                break;

            //Width = CellsOnX * CellWidth;
            }

            // Draw the Grid Lines
            Pen LinePen = new Pen(Color.Black, LineWidth);
                
            for (int y = 0; y < (Height); y += CellHeight)
                e.Graphics.DrawLine(LinePen, 0, y, CellWidth * CellsOnX, y);

            for (int x = 0; x < (Width); x += CellWidth)
                e.Graphics.DrawLine(LinePen, x, 0, x, CellHeight * CellsOnY);

            // if Image attached then drawing the image
            if (_GridImage != null)
            {
                switch (DisplayMode)
                {
                    case DisplayModes.Color:
                        for (int y = 0; y < _GridImage.Height; y++)
                            for (int x = 0; x < _GridImage.Width; x++)
                                SetPixel(x, y, _GridImage.GetPixel(x, y));
                        break;

                    case DisplayModes.Mono:
                        for (int y = 0; y < _GridImage.Height; y++)
                            for (int x = 0; x < _GridImage.Width; x++)
                                if (_GridImage.GetPixel(x, y).A != 0x00)
                                    SetPixel(x, y, DrawColor);
                        break;
                }
            }

        }

        /// <summary>
        /// Fill a Cell With Color.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel (int x,int y, Color color)
        {
            gfx = CreateGraphics();
            if (CellBorder)
                gfx.FillRectangle(new SolidBrush(color), new Rectangle(x * CellWidth+LineWidth, y * CellHeight+LineWidth, CellWidth-LineWidth, CellHeight-LineWidth));
            else
                gfx.FillRectangle(new SolidBrush(color), new Rectangle(x*CellWidth/*+LineWidth*/, y*CellHeight/*+LineWidth*/, CellWidth/*-LineWidth*/, CellHeight/*-LineWidth*/));
        }

        public Color GetPixel(int x, int y)
        {
            return _GridImage.GetPixel(x, y);
        }

        public bool IsPixelOn (int x, int y)
        {
            if (_GridImage.GetPixel(x,y).A != 0x00) return true;
            return false;
        }
        private void ImageGrid_MouseClick(object sender, MouseEventArgs e)
        {

            if (EnableDrawing)
            {
                Point Cell = PointToClient(Cursor.Position);

                int XPos = Cell.X / CellWidth;
                int YPos = Cell.Y / CellHeight;

                SetPixel(XPos, YPos, DrawColor);   
            }          
        }

    

        
      /*  private void ImageGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (EnableDrawing)
            {
                Point LocalCurPos = PointToClient(Cursor.Position);
                int XPos = LocalCurPos.X / CellWidth;
                int YPos = LocalCurPos.Y / CellHeight;


                SetPixel(XPos, YPos, DrawColor); 
            }
        
        }*/

        private void ImageGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (EnableDrawing)
            {
                Point LocalCurPos = PointToClient(Cursor.Position);
                int XPos = LocalCurPos.X / CellWidth;
                int YPos = LocalCurPos.Y / CellHeight;


                if (Control.MouseButtons == MouseButtons.Left)
                {
                    SetPixel(XPos, YPos, DrawColor);
                } 
            }
        }

   
   

        private void ImageGrid_Resize(object sender, EventArgs e)
        {
           /* if (_GridImage == null)
                return;

            switch (DisplayMode)
            {
                case DisplayModes.Color:
                    for (int y = 0; y < _GridImage.Height; y++)
                        for (int x = 0; x < _GridImage.Width; x++)
                            SetPixel(x, y, _GridImage.GetPixel(x, y));
                    break;

                case DisplayModes.Mono:
                    for (int y = 0; y < _GridImage.Height; y++)
                        for (int x = 0; x < _GridImage.Width; x++)
                            if (_GridImage.GetPixel(x, y).A != 0x00)
                                SetPixel(x, y, DrawColor);
                    break;
            }*/
        }

        private void ImageGrid_Load(object sender, EventArgs e)
        {
    /*        if (_GridImage == null)
                 return;

            switch (DisplayMode)
            {
                case DisplayModes.Color:
                    for (int y = 0; y < _GridImage.Height; y++)
                        for (int x = 0; x < _GridImage.Width; x++)
                            SetPixel(x, y, _GridImage.GetPixel(x, y));
                    break;

                case DisplayModes.Mono:
                    for (int y = 0; y < _GridImage.Height; y++)
                        for (int x = 0; x < _GridImage.Width; x++)
                            if (_GridImage.GetPixel(x, y).A != 0x00)
                                SetPixel(x, y, DrawColor);
                    break;
            }*/
        }

    

        public ImageGrid()
        {
            InitializeComponent();
            CellsOnX = 16;
            CellsOnY = 16;

            LineWidth = 2;

            CellWidth = (Width) / CellsOnX;
            CellHeight = (Height) / CellsOnY;

           


        }

        public void DrawImage (Bitmap image)
        {
            _GridImage = image;

            CellsOnX = image.Width;
            CellsOnY = image.Height;

            if (CellSizing == CellSize.Fixed)
            {
                Width = CellsOnX * CellWIdthHeight + LineWidth;
                Height = CellsOnY * CellWIdthHeight + LineWidth;
            }
            else
            {
                Width = CellsOnX * CellWidth + LineWidth;
                Height = CellsOnY * CellHeight + LineWidth;
            }
         

            //CellHeight = CellWidth;

   


            switch (DisplayMode)
            {
                case DisplayModes.Color:
                    for (int y = 0; y < image.Height; y++)
                        for (int x = 0; x < image.Width; x++)
                            SetPixel(x, y, image.GetPixel(x, y));
                break;

                case DisplayModes.Mono:
                    for (int y = 0;y < image.Height; y++)
                        for(int x = 0;x < image.Width; x++)
                            if (image.GetPixel(x, y).A != 0x00)
                                SetPixel(x, y, DrawColor);
                break;
            }
        }


    }
}
