using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GfxLib
{
    public partial class ImageGrid : UserControl
    {
        public int _XCels = 16, _YCels = 16;
        public int _LineWidth = 2;
        private int CellWidth , CellHeight;
        public Bitmap _GridImage;
        public Color _DrawColor = Color.Blue;
        public bool _KeepAspectRatio = true;
        //public bool _CellBorder = true;

        public bool CellBorder { get; set; } = true;

        public Color DrawColor {
            get  => _DrawColor;
            set
            {
                  _DrawColor = value;
            }
        }

        Graphics gfx;
        public int LineWidth { 
            get => _LineWidth; 
            set
            {
                _LineWidth = value;
                Refresh();
            }
        
        }

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

        public bool KeepAspectRatio
        {
            get => _KeepAspectRatio;
            set => _KeepAspectRatio = value;
        }
        public void SetCellDimention (int width, int height)
        {
            CellWidth = width;
            CellHeight = height;

        }

        public void SetResolution (int cellsPerLines, int heightInCells)
        {
            Width = CellWidth * cellsPerLines;
            Height = CellHeight * heightInCells;
        }

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
            CellWidth = Width / CellsOnX;

            if (_KeepAspectRatio)
            {
                CellHeight = CellWidth;
                Height = CellHeight * CellsOnY+ _LineWidth;
            }
            else
                CellHeight = Height / CellsOnY;

            //Width = CellsOnX * CellWidth;
            
            Pen LinePen = new Pen(Color.Black, LineWidth);
                
            for (int y = 0; y < (Height); y += CellHeight)
                e.Graphics.DrawLine(LinePen, 0, y, CellWidth * CellsOnX, y);

            for (int x = 0; x < (Width); x += CellWidth)
                e.Graphics.DrawLine(LinePen, x, 0, x, CellHeight * CellsOnY);

        

        }


        public void SetPixel (int x,int y, Color color)
        {
            gfx = CreateGraphics();
            if (CellBorder)
                gfx.FillRectangle(new SolidBrush(color), new Rectangle(x * CellWidth+LineWidth, y * CellHeight+LineWidth, CellWidth-LineWidth, CellHeight-LineWidth));
            else
                gfx.FillRectangle(new SolidBrush(color), new Rectangle(x*CellWidth/*+LineWidth*/, y*CellHeight/*+LineWidth*/, CellWidth/*-LineWidth*/, CellHeight/*-LineWidth*/));
        }

        private void ImageGrid_MouseClick(object sender, MouseEventArgs e)
        {
            
            Point Cell = PointToClient(Cursor.Position);

            int XPos = Cell.X / CellWidth;
            int YPos = Cell.Y / CellHeight;

            SetPixel (XPos, YPos, DrawColor);            
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

            //CellHeight = CellWidth;

            Width = CellsOnX * CellWidth+LineWidth;
            Height = CellsOnY * CellHeight+LineWidth;

            for (int y =0; y < image.Height; y++)
                for (int x =0; x < image.Width; x++)
                    SetPixel(x, y, image.GetPixel(x, y));
        }

    }
}
