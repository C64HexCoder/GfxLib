using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
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
        public int xSize;
        public int ySize;
        private char _character;
        private int _Ascii;
        private bool _Selected;
        private Color _selecterColor = Color.Yellow;
        private SoundPlayer _SoundPlayer;
        private string _SelectedSound;
        //private int _HightInPages;

        
        public int HightInPages
        {
            get
            {
                return ySize % 8 == 0 ? ySize / 8 : ySize / 8 + 1;
            }
        }

        public string SelectedSound
        {
            get { return _SelectedSound; }
            set
            {
             //   OpenFileDialog dlg = new OpenFileDialog();
             //   dlg.Filter = "Wave|*.wav";
              //  if (dlg.ShowDialog() == DialogResult.OK)
               //     _selectedSound = dlg.FileName;

                _SelectedSound = value;
            }
        }

        [Description("The sound to play when control selected"),Category("Behavior")]
        public SoundPlayer SoundPlayer { get { return _SoundPlayer; } set { _SoundPlayer = value; } }

        [Description("The color in which che control will be painted when selected"),Category("Behavior")]
        public Color SelectedColor { get => _selecterColor; set => _selecterColor = value; }

        [Description("Select the FontDisp and highlight it"),Category("Behavior")]
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                if (value == true)
                    BackColor = _selecterColor;
                else
                    BackColor = SystemColors.Control;

                _Selected = value;
            }
        }

        [Description("The ascii code of the character"),Category("Information")]
        public int Ascii
        {
            get => _Ascii; set => _Ascii = value;
        }

        [Description("The character printer in the grid"),Category("Information")]
        public char Character
        {
            get => _character; set => _character = value;
        }

        

        [Description("Return the Width of the grid (Read only)"),Category("Grid")]
        public int GridWidth { get => xSize;}
        [Description("Return the Height of the grid (Read only)"), Category("Grid")]
        public int GridHeight { get => ySize;}

    [Description("The Width and height of each cell when in CellSizing.Fixxed mode"), Category("Grid")]

        public int CellsWidthHeight
        {
            get => imageGrid.CellWIdthHeight;
            set => imageGrid.CellWIdthHeight = value;
        }


   
        public FontDisp()
        {
            InitializeComponent();
            imageGrid.KeepAspectRatio = false;
            imageGrid.EnableDrawing=false;

        }

        [Description("Constructor which defines the Width of the Control")]
        public FontDisp(int width)
        {
            InitializeComponent();
            imageGrid.KeepAspectRatio = true;
            imageGrid.EnableDrawing = false;
            AutoSize = false;
            Width = width;

        }

        public FontDisp(Font font, char charToDraw)
        {
            InitializeComponent();
            imageGrid.KeepAspectRatio = false;
            imageGrid.EnableDrawing = false;
            Font = font;
            DrawChar (charToDraw);
        }

        public FontDisp(Font font, char charToDraw, int width)
        {
            InitializeComponent();
            imageGrid.KeepAspectRatio = false;
            imageGrid.EnableDrawing = false;
            Font = font;
            DrawChar(charToDraw,width);
        }

        public FontDisp(char charToDraw)
        {
            InitializeComponent();
            imageGrid.KeepAspectRatio = false;
            imageGrid.EnableDrawing = false;
            DrawChar(charToDraw);
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


        public void DrawChar (char charToDraw)
        {
            //Height = this.Font.Height * imagseGrid.CellWIdthHeight + imageGrid.LineWidth;
            
            Character = charToDraw;
            Ascii = Convert.ToInt32(charToDraw);
            
            Graphics tmpGfx = CreateGraphics();
            SizeF strSize = tmpGfx.MeasureString(charToDraw.ToString(), Font);
            xSize = (int)Math.Round(strSize.Width);
            ySize = (int)Math.Round(strSize.Height);
            fontBitmap = new Bitmap(xSize, ySize);
            Graphics fontBMGfx = Graphics.FromImage(fontBitmap);
            fontBMGfx.Clear(Color.Transparent);
            fontBMGfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            fontBMGfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            fontBMGfx.DrawString(charToDraw.ToString(), this.Font, Brushes.Black, new PointF(0, 0));
            fontBMGfx.Dispose();
       
            imageGrid.GridImage = fontBitmap;
            charAsciiTxt.Text = Convert.ToInt16(charToDraw).ToString();

            if (AutoSize == true)
            {         
                 Width = 1 + charAsciiTxt.Width + 2 + imageGrid.Width;
                 Height = imageGrid.Height;

                charAsciiTxt.Left = 1;
                charAsciiTxt.Top = (Height - charAsciiTxt.Height) / 2;

                imageGrid.Left = charAsciiTxt.Right + 2;
                imageGrid.Top = 0;
            }
            else
            {
                //imageGrid.Left = this.Bounds.Right -  imageGrid.Width;
            }

          
        }

        public void DrawChar(char charToDraw, int width)
        {
            //Height = this.Font.Height * imagseGrid.CellWIdthHeight + imageGrid.LineWidth;
            Character = charToDraw;
            Ascii = Convert.ToInt32(charToDraw);

            Graphics tmpGfx = CreateGraphics();
            SizeF strSize = tmpGfx.MeasureString(charToDraw.ToString(), Font);
            xSize = (int)Math.Round(strSize.Width);
            ySize = (int)Math.Round(strSize.Height);
            fontBitmap = new Bitmap(xSize, ySize);
            Graphics fontBMGfx = Graphics.FromImage(fontBitmap);
            fontBMGfx.Clear(Color.Transparent);
            fontBMGfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            fontBMGfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            fontBMGfx.DrawString(charToDraw.ToString(), this.Font, Brushes.Black, new PointF(0, 0));
            fontBMGfx.Dispose();

            imageGrid.GridImage = fontBitmap;
            charAsciiTxt.Text = Convert.ToInt16(charToDraw).ToString();

            if (AutoSize == true)
            {
                Width = 1 + charAsciiTxt.Width + 2 + imageGrid.Width;
                Height = imageGrid.Height;

                charAsciiTxt.Left = 1;
                charAsciiTxt.Top = (Height - charAsciiTxt.Height) / 2;

                imageGrid.Left = charAsciiTxt.Right + 2;
                imageGrid.Top = 0;
            }
            else
            {
                Width = width;
                //imageGrid.Left = this.Bounds.Right -  imageGrid.Width;
            }


        }


        private void imageGrid_Resize(object sender, EventArgs e)
        {
            Width = 1 + charAsciiTxt.Width + 2 + imageGrid.Width;
            Height = imageGrid.Height;

            charAsciiTxt.Left = 1;
            charAsciiTxt.Top = (Height - charAsciiTxt.Height) / 2;

            imageGrid.Left = charAsciiTxt.Right + 2;
            imageGrid.Top = 0;
        }

        public byte GetVerticalByte (int x, int page)
        {
            int Yoffset = (int)page*8;
            byte ReturnValue = 0;

            for (int y = 0; y<8;y++)
            {
                if (page*8+y == ySize)
                    return ReturnValue;

                if (imageGrid.GetPixel(x,page*8+y).A !=  0)
                {
                    ReturnValue |= (byte)(1 << y);
                }
            }
            return ReturnValue;
        }

        private void FontDisp_Click(object sender, EventArgs e)
        {
    /*        if (this.Focused == true)
            {
                this.BackColor = Color.Red;
                this.BorderStyle = BorderStyle.Fixed3D;
            }
            else
            {
                this.BackColor = Color.Green;
            }*/
        }
    }
}
