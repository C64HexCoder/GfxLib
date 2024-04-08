using System;
using System.Drawing;
using System.Media;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Imaging;



namespace GfxLib
{
    public partial class Pallate : UserControl
    {

        public int _NumOfColors = 4;
        public Color[] Colors = new Color[256];
        // 0 1 2 3 4  5  6  7
        byte[] Yarray = { 1, 2, 2, 4, 4, 8, 8, 16, 16 };
        byte[] Xarray = { 1, 1, 2, 2, 4, 4, 8, 8, 16 };
        public Color _SelectedColor;
        public int SelectedColorARGB;
        public byte _SelectedColorIdx = 0;

        int[] ColorDialogCustomColors;
        private bool _SelectedColorBox = false;
        int NumOfCellsX;
        int NumOfCellsY;


        public bool AlphChennel {
            get
            {
                return AlphaNum.Visible;
            }
            set
            {
                AlphaLbl.Visible= value;
                AlphaNum.Visible= value;
            }
        }

        public event EventHandler<SelectedColorEventArgs> ColorSelected;

        public class SelectedColorEventArgs : EventArgs
        {
            public Color Color;
            public int colorIndex;
        }

        protected virtual void OnColorSelected (SelectedColorEventArgs e)
        {
            EventHandler<SelectedColorEventArgs> eventHandler = ColorSelected;

            if (eventHandler != null)
            {
                eventHandler (this, e);
            }
        }


        byte _Red, _Green, _Blue;
        public bool RGBVisible
        {
            set
            {
                redNum.Visible = value;
                greenNum.Visible = value;
                blueNum.Visible = value;
                RedLbl.Visible = value;
                GreenLbl.Visible = value;
                BlueLbl.Visible = value;
            }
            get { return redNum.Visible; }
        }

        byte Red => _Red;
        byte Green => _Green;
        byte Blue => _Blue;


        public Color SelectedColor
        {
            get { return _SelectedColor; }
            set {
                _SelectedColor = value;
                SelectedColorARGB = value.ToArgb();

                selectedColorPb.BackColor = _SelectedColor;

                redNum.Value = _SelectedColor.R;
                greenNum.Value = _SelectedColor.G;
                blueNum.Value = _SelectedColor.B;
                AlphaNum.Value = _SelectedColor.A;

            }
        }

        public byte SelectedColorIdx
        {
            get { return _SelectedColorIdx; }
            set
            {
                


                if (colorPanel.Controls.Count > 0)
                {
                    //if (_SelectedColorIdx < colorPanel.Controls.Count)
                        DrawBorder((PictureBox)colorPanel.Controls[_SelectedColorIdx], colorPanel.Controls[_SelectedColorIdx].BackColor, 4);
                    
                    DrawBorder((PictureBox)colorPanel.Controls[value], Color.Black, 4);
                }

                _SelectedColorIdx = value;
                SelectedColor = Colors[value];

                idxNum.ForeColor = Color.FromArgb(255 - _SelectedColor.R, 255 - _SelectedColor.G, 255 - _SelectedColor.B);
                idxNum.Text = _SelectedColorIdx.ToString();
                idxNum.BackColor = _SelectedColor;
             


            }
        }
        public int NumOfColors
        {
            get { return _NumOfColors; }
            set {
                _NumOfColors = value;
                colorPanel.Controls.Clear();
                colorPanel.Refresh();
            }
        }

        public bool SelectedColorBox {
            set
            {
                selectedColorPb.Visible = value;
                _SelectedColorBox = value;
            }
            get
            {
                return selectedColorPb.Enabled;
            }
        }

        public Pallate()
        {
            InitializeComponent();
            Random rnd = new Random();
          

            idxNum.Top = selectedColorPb.Top + (int)(selectedColorPb.Height - idxNum.Height) / 2;
            idxNum.Left = selectedColorPb.Left + (int)(selectedColorPb.Width - idxNum.Width) / 2;


            for (int i = 0; i < NumOfColors; i++)
            {
                    Colors[i] = Color.FromArgb(rnd.Next (255),rnd.Next(255), rnd.Next(255));
            }

            SelectedColorIdx = 0;
        }

        private void colorPanel_Resize(object sender, EventArgs e)
        {
  
            Panel panel = ((Panel)sender);
            panel.Controls.Clear();
  
            panel.Invalidate();
        }

        private void Pallate_Resize(object sender, EventArgs e)
        {
            colorPanel.Width = Width; colorPanel.Height = Height - selectedColorPb.Height - RGBPanel.Height - 8;
            selectedColorPb.Width = Width;
            selectedColorPb.Left = colorPanel.Left;
            selectedColorPb.Top = colorPanel.Bottom + 4;
            RGBPanel.Top = selectedColorPb.Bottom + 4;
            RGBPanel.Left = colorPanel.Left;
            RGBPanel.Width = Width;

            idxNum.Top = selectedColorPb.Top + (int)(selectedColorPb.Height - idxNum.Height) / 2;
            idxNum.Left = selectedColorPb.Left + (int)(selectedColorPb.Width - idxNum.Width) / 2;

            

            colorPanel.Refresh();
        }

        private void RGBPanel_Resize(object sender, EventArgs e)
        {
            Panel RGBPanel = ((Panel)sender);
            int NumUDWidth,SpaceBetweenNUD;

            if (AlphChennel)
            {
                NumUDWidth = (int)(RGBPanel.Width * 0.2f);
                SpaceBetweenNUD = (int)(RGBPanel.Width * 0.04f);

            }
            else
            {
                NumUDWidth = (int)(RGBPanel.Width * 0.27f);
                SpaceBetweenNUD = (int)(RGBPanel.Width * 0.05f);
            }

            redNum.Width = NumUDWidth;
            greenNum.Width = NumUDWidth;
            blueNum.Width = NumUDWidth;
            AlphaNum.Width = NumUDWidth;


            redNum.Left = RGBPanel.Left + SpaceBetweenNUD;
            greenNum.Left = redNum.Right + SpaceBetweenNUD;
            blueNum.Left = greenNum.Right + SpaceBetweenNUD;
            AlphaNum.Left = blueNum.Right+ SpaceBetweenNUD;

            RedLbl.Left = redNum.Left + (int)((redNum.Width - RedLbl.Width) / 2);
            GreenLbl.Left = greenNum.Left + (int)((greenNum.Width - GreenLbl.Width) / 2);
            BlueLbl.Left = blueNum.Left + (int)((blueNum.Width - BlueLbl.Width) / 2);
            AlphaLbl.Left = AlphaNum.Left + (int)((AlphaNum.Width - AlphaLbl.Width) / 2);
        }

        private void selectedColorPb_Resize(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
          //  idxNum.Top = pb.Top + (int)(pb.Height - idxNum.Height)/2;
         //   idxNum.Left = pb.Left+ (int)(pb.Width - idxNum.Width) / 2;
        }


        private void colorPanel_Paint(object sender, PaintEventArgs e)
        {
            int margim = 0;
            //int RawAndClomn = (int)Math.Sqrt(NumOfColors);
            int index = (int)(Math.Log(NumOfColors) / Math.Log(2));
            NumOfCellsX = Xarray[index];
            NumOfCellsY = Yarray[index];

            for (int y = 0; y < NumOfCellsY; y++)
            {
                for (int x = 0; x < NumOfCellsX; x++)
                {
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Size = new Size(colorPanel.Width / NumOfCellsX- margim, colorPanel.Height / NumOfCellsY- margim);
                    pictureBox.BackColor = Colors[y * (NumOfCellsX) + x];
                    pictureBox.MouseClick += ColorMouseClick;
                    pictureBox.BorderStyle = BorderStyle.FixedSingle;
                    pictureBox.Tag = y * NumOfCellsX + x;
                    pictureBox.Location = new Point(colorPanel.Width / NumOfCellsX * x+ margim, colorPanel.Height / NumOfCellsY * y+margim);

                    ControlPaint.DrawBorder(e.Graphics, pictureBox.ClientRectangle, Color.Blue, ButtonBorderStyle.Solid);
                    ((Panel)sender).Controls.Add(pictureBox);
                }
            }
        }

        private void ColorMouseClick(object sender, MouseEventArgs e)
        {
            
            PictureBox pickedColor = (PictureBox)sender;
            byte index = byte.Parse(pickedColor.Tag.ToString());

            if (e.Button == MouseButtons.Right && !(pickedColor.Tag.ToString() == "0"))
            {
                ColorDialog colorDialog = new ColorDialog();

                if (ColorDialogCustomColors != null)
                    colorDialog.CustomColors = ColorDialogCustomColors;

                colorDialog.Color = pickedColor.BackColor;
                colorDialog.FullOpen = true;
                colorDialog.AnyColor = true;
                colorDialog.SolidColorOnly = false;
                colorDialog.ShowDialog();
                ColorDialogCustomColors = colorDialog.CustomColors;
                // Color3P.BackColor = colorDialog.Color;
                pickedColor.BackColor = colorDialog.Color;

                Colors[int.Parse(pickedColor.Tag.ToString())] = pickedColor.BackColor;
                //Colors[int.Parse((pictureBox.Tag.ToString()))] = pictureBox.BackColor;

            }

            SelectedColorIdx= index;


            SelectedColorEventArgs args = new SelectedColorEventArgs();
            args.Color = SelectedColor;
            args.colorIndex = index;

            OnColorSelected(args);
        }

        public void SetPalette(Color[] palette)
        {
            NumOfColors= palette.Length;
            Colors = palette;
            _SelectedColorIdx = 0;
            
        }
        public void SetPalette (Image image)
        {
            if (image != null)
            {
                if (image.Palette != null)
                {
                    NumOfColors = image.Palette.Entries.Length;
                    Colors = image.Palette.Entries;
                    _SelectedColorIdx = 0;
                }
                else 
                {
                    MessageBox.Show("The image does not have Pallate of colors.");
                }
            }
        }

        public byte FindColorInPallate (Color color)
        {
            byte i = 0;
            while (i < NumOfColors && Colors[i].ToArgb() != color.ToArgb())
            {
              i++;
            } 
            return i;

        }


        private void Pallate_Enter(object sender, EventArgs e)
        {
            SelectedColorIdx = 0;

            SoundPlayer soundPlayer = new SoundPlayer();
            soundPlayer.Play();
        }

        private void DrawBorder (PictureBox pb,Color color, int BorderWidth)
        {
            Graphics g = pb.CreateGraphics();
            Pen pen = new Pen(color, BorderWidth);
            g.DrawRectangle(pen, pb.ClientRectangle);
            g.Dispose();
        }

        private void RemoveBorder (PictureBox pictureBox)
        {
            Graphics g = pictureBox.CreateGraphics();
            g.Clear(System.Drawing.SystemColors.Control);
        }
    }
}