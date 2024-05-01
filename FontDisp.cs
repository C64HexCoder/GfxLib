using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GfxLib
{
    public partial class FontDisp : UserControl
    {
        private int _CellsOnX = 8; int _CellsOnY = 8;
        Bitmap fontBitmap;


        [Description("The Width and height of each cell when in CellSizing.Fixxed mode"), Category("Grid")]

        public int CellsWidthHeight
        {
            get => imageGrid.CellWIdthHeight;
            set => imageGrid.CellWIdthHeight = value;
        }


    /*    [Description("The Backgrond color of the image Displayed in the Grid."), Category("Grid")]
        public Color ImageBackColor
        {
            get => _GridBGColr;
            set
            {
                imageGrid.BackgroundColor = value;
            }
        }*/
        public FontDisp()
        {
            InitializeComponent();
            imageGrid.KeepAspectRatio = false;
            imageGrid.EnableDrawing=false;

         

          

            //Refresh();

        }

        [Description("Used to Indicate which Cell size to use. Fixed Cell Size in which we decide what is the cell size or a dinamic " +
          "in which the cell size decided automatically by devideing the width and heights of the image grid border box"), Category("Grid")]

        public ImageGrid.CellSize CellSizing
        {
            get { return imageGrid.CellSizing; }
            set { imageGrid.CellSizing = value;}
        }

        [Description("Number of Cells in each row of the Grid"), Category("Grid")]
        public int CellsOnX
        {
            get => _CellsOnX;

            set
            {
                _CellsOnX = value;
                imageGrid.CellsOnX = value;
            }
        }

        [Description("Number of raws in the Grid"),Category("Grid")]
        public int CellsOnY
        {
            get => _CellsOnY;

            set
            {
                _CellsOnY = value;
                imageGrid.CellsOnY = value;
            }
        }

    /*    public Font SelectedFont // To Remove?
        {
            get => (Font)_SelectedFont;
            set
            {
                _SelectedFont = value;
                fontChar = new Bitmap((int)_SelectedFont.Size, _SelectedFont.Height);
                Graphics bitGraph = Graphics.FromImage(fontChar);
                bitGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                bitGraph.DrawString("G", _SelectedFont, Brushes.Black, new PointF(0, 0));
                bitGraph.Dispose();
                imageGrid.DrawImage(fontChar);           
            }
        } */

        public void DrawChar (char charToDraw)
        {
            Height = this.Font.Height * imageGrid.CellWIdthHeight + imageGrid.LineWidth;
            fontBitmap = new Bitmap((int)this.Font.Size, this.Font.Height);
            Graphics fontBMGfx = Graphics.FromImage(fontBitmap);
            fontBMGfx.Clear(Color.Transparent);
            fontBMGfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            fontBMGfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            fontBMGfx.DrawString(charToDraw.ToString(), this.Font, Brushes.Black, new PointF(0, 0));
            fontBMGfx.Dispose();
            imageGrid.GridImage = fontBitmap;
            charAsciiTxt.Text = Convert.ToInt16(charToDraw).ToString();
            Width = 1 + charAsciiTxt.Width + 2 + imageGrid.Width;
            Height = imageGrid.Height;

            //Refresh();
            charAsciiTxt.Left = 1;
            charAsciiTxt.Top = (Height - charAsciiTxt.Height) / 2;

            imageGrid.Left = charAsciiTxt.Right + 2;
            imageGrid.Top = 0;

        }
     

        private void FontDisp_Paint(object sender, PaintEventArgs e)
        {
         /*   charAsciiTxt.Left = 1;
            imageGrid.Left = charAsciiTxt.Right + 2;
            //imageGrid.Width = Right - imageGrid.Left;
            Height = imageGrid.Height;
            imageGrid.Top = 0;
            charAsciiTxt.Top = (Height - charAsciiTxt.Height) / 2;
            Width = 1 + charAsciiTxt.Width + 2 + imageGrid.Width;*/
        }

        private void FontDisp_Load(object sender, EventArgs e)
        {
            Width = 1 + charAsciiTxt.Width + 2 + imageGrid.Width;
            Height = imageGrid.Height;

            charAsciiTxt.Left = 1;
            charAsciiTxt.Top = (Height - charAsciiTxt.Height) / 2;

            imageGrid.Left = charAsciiTxt.Right + 2;
            imageGrid.Top = 0;

        }

      
    }
}
