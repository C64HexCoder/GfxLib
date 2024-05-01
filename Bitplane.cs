using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace GfxLib
{
    public class Bitplane : IFF
    {
        //uint[] Pallate = new uint[32];
        //private Bitmap bmp;
        public Bitmap Mask;
        byte[] ImageMask;

        //public Color[] palette;
        public int PaletteSize;
        public byte NumOfBitmaps;
       // public byte[,] Bitplanes;
        public PictureBox externalPictureBox;
        Endian endian = new Endian();
        List<Color> colors = new List<Color>();
        public int stride;
        public int actualWidth;
        int _NewWidth;//, WidthInBytes;

        //public IFF IFFImage = new IFF();

        public ImageGrid imageGrid;

        public Color[] Pallate
        {
            get { return palette; }
            set
            {
                List<Color> ImpPallette = value.ToList<Color>();
                PaletteSize = CalculatePaletteSize(ImpPallette.Count);
                palette = new Color[PaletteSize];
                if (!ImpPallette.Find(C => C.A == 0).IsEmpty)
                {
                    palette[0] = ImpPallette.Find(C => C.A == 0);
                    ImpPallette.Remove(ImpPallette.Find(C => C.A == 0));
                    ImpPallette.CopyTo(palette, 1);
                }
                else
                    ImpPallette.CopyTo(palette);

            }
        }

        public Bitmap bitmap
        {
            get { return bmp; }
            set
            {
                //Pallate = null;

                bmp = value;
                WidthInBytes = (bmp.Width % 8 == 0 ? bmp.Width / 8 : bmp.Width / 8 + 1);
                switch (bmp.PixelFormat)
                {
                    case PixelFormat.Format64bppArgb:
                    case PixelFormat.Format32bppArgb:
                    case PixelFormat.Format32bppRgb:
                    case PixelFormat.Format24bppRgb:
                        ConvertImageToBitmaps();
                        break;
                    case PixelFormat.Format16bppArgb1555:
                    case PixelFormat.Format16bppRgb565:
                    case PixelFormat.Format16bppRgb555:
                        break;
                    case PixelFormat.Format8bppIndexed:
                        Convert8bppImageToBitmaps();
                        break;
                    case PixelFormat.Format4bppIndexed:
                        Convert4bppImageToBitmaps();
                        break;
                    case PixelFormat.Format1bppIndexed:
                        Convert1bppImageToBitmap();
                        break;

                }
               // CreateMask();

                if (externalPictureBox != null)
                    externalPictureBox.Image = value;
            }
        }
        public enum Alignment
        {
            Byte,
            Word,
            Long,
            DoubleLong
        }

       public enum BlitWord
        {
            None,
            Left,
            Right,
            Both
        }

    
        public int Stride
        {
            get { return stride; }
            set { stride = value; 
                switch (bmp.PixelFormat)
                {
                    case PixelFormat.Format1bppIndexed:
                        actualWidth = value * 8;
                        break;
                    case PixelFormat.Format4bppIndexed:
                        actualWidth = value * 2;
                        break;
                    case PixelFormat.Format8bppIndexed:
                        actualWidth = value;
                        break;

                }            
            }
        }

        public int ActualWidth
        {
            get { return actualWidth; }
            set { actualWidth = value; }
        }

        public int Width
        {
            get { return bmp.Width; }
        }
        public int Height
        {
            get { return bmp.Height; }
        }

        public int NewWidth
        {
          //  get { return _NewWidth; }
            get => _NewWidth;
            set => _NewWidth = value;
        }

        public enum OutputSize
        {
            Byte,
            Word,
            Long
        }

        public void LoadImage (string fileName)
        {
            //_Pallate = null;
            bmp = new Bitmap(fileName);
            WidthInBytes = (bmp.Width % 8 == 0 ? bmp.Width / 8 : bmp.Width / 8 + 1);
            switch (bmp.PixelFormat)
            {
                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format24bppRgb:
                    ConvertImageToBitmaps();
                    break;
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppRgb565:
                case PixelFormat.Format16bppRgb555:
                    break;
                case PixelFormat.Format8bppIndexed:
                    Convert8bppImageToBitmaps();
                    break;
                case PixelFormat.Format4bppIndexed:
                    Convert4bppImageToBitmaps();
                    break;
                case PixelFormat.Format1bppIndexed:
                    Convert1bppImageToBitmap ();
                    break;
        
      
            }
            //CreateMask();

            if (externalPictureBox != null)
                externalPictureBox.Load(fileName);

        }



        public Alignment CheckImageAlignment()
        {
            if (bmp == null)
            {
                MessageBox.Show ("Please load image first","No Image loaded",MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw new NullReferenceException("Bitmap refrence is null, Load Image first");
                return Alignment.Byte;
            }
            if (bmp.Width % 64 == 0)
            {
                return Alignment.DoubleLong;
            }
            else if (bmp.Width % 32 == 0)
            {
                return Alignment.Long;   
            }
            else if (bmp.Width % 16 == 0)
            {
                return Alignment.Word;
            }
            else
                return Alignment.Byte;
        }


        public void AlienWidth (Alignment alien)
        {

            if (bmp == null)
                throw new NullReferenceException("Bitmap refrence is null, Load Image first");

            if (alien == Alignment.Word && bmp.Width % 16 != 0)
            {
                int newWidth = (int)Math.Ceiling((double)bmp.Width / 16) * 16;
                Bitmap newBmp = new Bitmap(newWidth, bmp.Height);
                Graphics g = Graphics.FromImage(newBmp);
                int Margin = (newWidth - bmp.Width) / 2;
                g.Clear(Pallate[0]);
                g.DrawImage(bmp, Margin, 0, bmp.Width, bmp.Height);
                g.Dispose();
                bmp = newBmp;
            }
            else if (alien == Alignment.Long && bmp.Width % 32 != 0)
            {
                int newWidth = (int)Math.Ceiling((double)bmp.Width / 32) * 32;
                Bitmap newBmp = new Bitmap(newWidth, bmp.Height);
                Graphics g = Graphics.FromImage(newBmp);
                int Margin = (newWidth - bmp.Width) / 2;
                g.Clear(Pallate[0]);
                g.DrawImage(bmp, Margin, 0, bmp.Width, bmp.Height);
                g.Dispose();
                bmp = newBmp;
            }
            else if (alien == Alignment.DoubleLong && bmp.Width % 64 != 0)
            {
                int newWidth = (int)Math.Ceiling((double)bmp.Width / 64) * 64;
                Bitmap newBmp = new Bitmap(newWidth, bmp.Height);
                Graphics g = Graphics.FromImage(newBmp);
                int Margin = (newWidth - bmp.Width) / 2;
                g.Clear(Pallate[0]);
                g.DrawImage(bmp, Margin, 0, bmp.Width, bmp.Height);
                g.Dispose();
                bmp = newBmp;
            }
            ConvertImageToBitmaps();
        }

        public void AddBlitterWord (BlitWord blitWord)
        {
            Bitmap NewBitmap;
            Graphics g;
            switch (blitWord)
            {
                case BlitWord.None:
                    break;
                case BlitWord.Left:
                    NewBitmap = new Bitmap(bitmap.Width + 16, bitmap.Height);
                    g = Graphics.FromImage(NewBitmap);
                    g.Clear(Pallate[0]);
                    g.DrawImage(bitmap, 16, 0, bitmap.Width, bitmap.Height);
                    g.Dispose();
                    bitmap = NewBitmap;  
                    break;
                case BlitWord.Right:
                    NewBitmap = new Bitmap(bitmap.Width + 16, bitmap.Height);
                    g = Graphics.FromImage(NewBitmap);
                    g.Clear(Pallate[0]);
                    g.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
                    g.Dispose();
                    bitmap = NewBitmap;
                    break;
                case BlitWord.Both:
                    NewBitmap = new Bitmap(bitmap.Width + 32, bitmap.Height);
                    g = Graphics.FromImage(NewBitmap);
                    g.Clear(Pallate[0]);
                    g.DrawImage(bitmap, 16, 0, bitmap.Width, bitmap.Height);
                    g.Dispose();
                    bitmap = NewBitmap;
                    break;
            }
        }

        public void ChangeWidth (int NewWidth)
        {

            if (bmp == null)
                throw new NullReferenceException("Bitmap refrence is null, Load Image first");
          
            Bitmap newBmp = new Bitmap(NewWidth, bmp.Height);
            Graphics g = Graphics.FromImage(newBmp);
            int Margin = (NewWidth - bmp.Width) / 2;
            g.Clear(Pallate[0]);
            g.DrawImage(bmp, Margin, 0, bmp.Width, bmp.Height);
            g.Dispose();
            bmp = newBmp;
            
            ConvertImageToBitmaps();
        }

        // Resize the image
        // If Width Or Hieght == 0 then calculate them while keeping the Aspect Ratio
        public void Resize (int Width = 0, int Height = 0)
        {
            if (Width != this.Width || Height != this.Height)
            {
                if (Width == 0 )
                {
                    float Ratio = (float)this.Width / (float)this.Height;
                    Width = (int) Math.Round (Height * Ratio);
                }
                else if (Height == 0)
                {
                    float Ratio = (float)this.Height / (float)this.Width;
                    Height = (int) Math.Round (Width * Ratio);

                }

                Bitmap resizedBitmap = new Bitmap(Width, Height);
                Graphics g = Graphics.FromImage(resizedBitmap);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(bitmap, 0, 0, Width, Height);
                g.Dispose();
                bmp = resizedBitmap;
                

                ConvertImageToBitmaps();
            }
        }

        private void CreateBitMaps()
        {
            // resize bitmap to be word aligned and calculate the new widht in bytes
            // all bitplanes need to be word aligned...
            // in AGA mode the bitmaps could be 32bit (long) aligned and even 64 bits aligned (double long).
            WidthInBytes = (bmp.Width % 8 == 0 ? bmp.Width/8 : bmp.Width/8 + 1);
           Bitplanes = new byte[NumOfBitmaps, WidthInBytes*bmp.Height];
           // NewWidth = WidthInBytes*8;
        }

        private byte CalculateNumOfBitmaps ()
        {
            return (byte)Math.Ceiling(Math.Log (PaletteSize)/Math.Log(2));
        }

        public void SaveBitmapsAsBinaryFile (string fileName)
        {
            Stream stream = new FileStream (fileName, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(stream);   

            for (int bitplane = 0; bitplane < NumOfBitmaps; bitplane++)
                for (int i =0; i < bmp.Width*bmp.Height/8; i++)
                    binaryWriter.Write (Bitplanes[bitplane, i]);

            binaryWriter.Flush();
            binaryWriter.Close();

            
        }

        public void SaveBitmapsAsLongBinaryFile(string fileName)
        {
            int BytesToPad = Bitplanes.Length / 4 == 0 ? 0 : Bitplanes.Length % 4;

            Stream stream = new FileStream(fileName, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(stream);

            for (int bitplane = 0; bitplane < NumOfBitmaps; bitplane++)
                for (int i = 0; i < bmp.Width * bmp.Height / 8; i++)
                    binaryWriter.Write(Bitplanes[bitplane, i]);

            for (int i=0; i<BytesToPad; i++)
                binaryWriter.Write((byte)0x00);

            binaryWriter.Flush();
            binaryWriter.Close();


        }

        public void SaveBitmapsAsLongBinaryFile(BinaryWriter binaryWriter)
        {
            int BytesToPad = Bitplanes.Length / 4 == 0 ? 0 : Bitplanes.Length % 4;

            for (int bitplane = 0; bitplane < NumOfBitmaps; bitplane++)
                for (int i = 0; i < bmp.Width * bmp.Height / 8; i++)
                    binaryWriter.Write(Bitplanes[bitplane, i]);

            for (int i = 0; i < BytesToPad; i++)
                binaryWriter.Write((byte)0x00);


        }
        public void SaveBitmapsAsInterleavedBinaryFile (string fileName)
        {
            Stream stream = new FileStream(fileName, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(stream);
            int WidthInBytes = bmp.Width % 8 == 0 ? bmp.Width / 8 : bmp.Width / 8 + 1;

            for (int y = 0; y < bmp.Height; y++)
                for (int bitpmap =0; bitpmap < NumOfBitmaps; bitpmap++)
                    for (int x = 0; x < WidthInBytes; x++)
                    {
                        binaryWriter.Write (Bitplanes[bitpmap, y * bmp.Width/8 + x]);
                    }

            binaryWriter.Flush();
            stream.Close();

        }


        // The Stride is 32 bit aligned so the Stride with (width in bytes) can be sometimes wider the the bitmap width
        // so the output width is depended on the stride (bytes per line) and not the width in pixels...
        private void Convert1bppImageToBitmap()
        {
          
            BitmapData imageData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            IntPtr intPtr = imageData.Scan0;
            int imageSizeInBytes = imageData.Stride * imageData.Height;
            Stride = imageData.Stride;

            //if (Pallate == null)
            Pallate = bmp.Palette.Entries;

            NumOfBitmaps = CalculateNumOfBitmaps();
            //CreateBitMaps();

            byte[] imageDataBytes = new byte[imageSizeInBytes];
            Marshal.Copy(intPtr, imageDataBytes, 0, imageSizeInBytes);
            Bitplanes = new byte[1, WidthInBytes*Height];

            int OldWidthInBytes = (int) Math.Ceiling((double)bmp.Width / 8);
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < OldWidthInBytes; x++)
                    Bitplanes[0,y * WidthInBytes + x] = imageDataBytes[y * Stride + x];

            bmp.UnlockBits(imageData);
        }

        private void CopyImageDataToArray(byte[] ImageDataBytes)
        {

        }

        //--------------------------------------------------------------------------------------------------------------------------------------
        // Converting the BitMap raw data image to BitPlanes.
        // The BitMap Raw data uses 4 bits per pixel in 1bpp and 4bpp images.....
        // I had alot of head ake until i figured this out...
        //--------------------------------------------------------------------------------------------------------------------------------------
        private void Convert4bppImageToBitmaps()
        {
            BitmapData imageData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            IntPtr intPtr = imageData.Scan0;
            int imageSizeInBytes = imageData.Stride * imageData.Height;
          
            Stride = imageData.Stride;

            int bpp;
            byte[] imageDataBytes = new byte[imageSizeInBytes];
            Marshal.Copy(intPtr, imageDataBytes, 0, imageSizeInBytes);
            
            //if (Pallate== null)
            Pallate = bmp.Palette.Entries;

            NumOfBitmaps = CalculateNumOfBitmaps();

    
            CreateBitMaps();

            bpp = 4;

            int NumOfPixelsDataInByte = 8 / bpp; // in Bitmap raw data it uses 4 bits per pixel in both 1bpp and 4bpp images. So one byte always will hold two pixels data
            byte BitToShift;

            for (int y = 0; y < imageData.Height; y++)       // go over hight of image
                for (int x = 0; x < imageData.Width; x++)    // go over all the pixels in each line
                {
                    // get the byte from the image data. each byte holds 2 pixels colors
                    // each consist of 4 bits (nimble)
                    byte imageByte = imageDataBytes[y * imageData.Stride + x/2];

                    byte Nimble = (byte)((x & 0x01) == 0 ? imageByte >> 4 : imageByte);
                    BitToShift =(byte)(7 - (x & 0x07));
                    for (int Bitmap = 0; Bitmap < NumOfBitmaps; Bitmap++)
                    {

                        byte Temp = (byte)(((Nimble >> Bitmap) & 0b00000001) << BitToShift);
                        // set the color bit info in the appropiate byte.. each byte holds 8 pixel colors info
                        Bitplanes[Bitmap, y * WidthInBytes + x / 8] |= Temp;
                    }
                }

            bmp.UnlockBits(imageData);
        }

        //--------------------------------------------------------------------------------------------------------------------------------------
        // Converting the BitMap raw data image to BitPlanes.
        // The BitMap Raw data uses 4 bits per pixel in 1bpp and 4bpp images..... not true
        // I had alot of head ake until i figured this out...
        //--------------------------------------------------------------------------------------------------------------------------------------
        private void Convert8bppImageToBitmaps()
        {
            BitmapData imageData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            IntPtr intPtr = imageData.Scan0;
            int imageSizeInBytes = imageData.Stride * imageData.Height;
            Stride = imageData.Stride;

            byte[] imageDataBytes = new byte[imageSizeInBytes];
            Marshal.Copy(intPtr, imageDataBytes, 0, imageSizeInBytes);
            //if (Pallate == null)
            Pallate = bmp.Palette.Entries;

            NumOfBitmaps = CalculateNumOfBitmaps();
            CreateBitMaps();

            sbyte ShiftLeft = 7;
            for (int y = 0; y < imageData.Height; y++)
                for (int x = 0; x < imageData.Stride; x++)
                {
                    byte imageByte = imageDataBytes[y * imageData.Stride + x];
                    for (int bitmap =0; bitmap < NumOfBitmaps; bitmap++)
                    {
                        byte Bit = (byte)(((imageByte >> bitmap) & 0x1) << ShiftLeft);
                        Bitplanes[bitmap, y * imageData.Stride/8 + x/8] |= Bit;
                    }

                    if (--ShiftLeft < 0) ShiftLeft = 7;
                }



            bmp.UnlockBits(imageData);
        }

        // --------------------------------------------------------------------------------------------------------
        // Converts RGB image to bitplanes 
        // The number of bitplanes is disided by the number of colors found in the picture Automatically
        // If there is only 2 colors then it will make 1 bitplane data out of it...
        // --------------------------------------------------------------------------------------------------------
        public void ConvertImageToBitmaps ()
        {
            //WidthInBytes = bmp.Width % 8 == 0 ? bmp.Width / 8 : bmp.Width / 8 + 1;

            CreatePallate();
            NumOfBitmaps = CalculateNumOfBitmaps();
            CreateBitMaps();

            Color PixelColor;
            byte colorIndex;
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                {
                    PixelColor = bmp.GetPixel(x, y);
                    colorIndex = GetColorPositionInPallate(PixelColor);
                    SetPixelDataAtBitmaps(x, y, colorIndex);
                }
        }

        // ----------------------------------------------------------------------------------------------------------
        // Going over All the Bitmap pixels and building a Pallate
        // ----------------------------------------------------------------------------------------------------------
        private void CreatePallate ()
        {
            bool FoundBackground = false;
            Color PixelColor;
            for (int y =0; y < bmp.Height; y++) 
                for (int x = 0; x < bmp.Width; x++)
                {
                    PixelColor = bmp.GetPixel(x, y);
                    // Add all colors to colors list exept for transparent color which will be color 0 (background)
                    if (PixelColor.A != 0 && !colors.Contains(PixelColor))
                        colors.Add (bmp.GetPixel (x,y));
                    else if (PixelColor.A == 0)
                                FoundBackground= true;
                }
            int NumOfColors = FoundBackground? colors.Count+1 : colors.Count;
            PaletteSize = CalculatePaletteSize(NumOfColors);

            palette = new Color[PaletteSize];

            if (FoundBackground)
                colors.CopyTo(palette, 1);
            else
                colors.CopyTo(palette, 0);

            // copy all colors from colors List exept color 0 which is the background color (transparent)

            colors.Clear();
        }

        private int CalculatePaletteSize (int NumOfColors)
        {
            double p = Math.Ceiling(Math.Log(NumOfColors) / Math.Log(2));
            return (int)Math.Pow(2, p);
        }

        public byte GetColorPositionInPallate(Color colorToFind)
        {
            for (byte i = 0; i < Pallate.Length; i++)
            {
                if (Pallate[i].ToArgb() == colorToFind.ToArgb())
                {
                    return i;
                }
            }

            return 0;
        }

        public void SavePallate (string fileName, ushort baseColor = 0x180)
        {
            StreamWriter sw = new StreamWriter(fileName);
            ushort colorRegister = baseColor;
            for (int color = 0; color < Pallate.Length; color++)
            {
                sw.WriteLine("\tDC.W ${0:X4},${1:X4}", colorRegister,ConvertRGBFrom32To12Bit(Pallate[color]));
                colorRegister += 2;
            }
            sw.Close();
        }

        private void SetPixelDataAtBitmaps (int x, int y, byte color)
        {
            //int WidthInBytes = bmp.Width % 8  == 0? bmp.Width / 8 : bmp.Width/8 +1;
            int byteXPos = x / 8;
            int bitInByte = (8 - (x % 8) -1);

            for (int bitmap = 0; bitmap < NumOfBitmaps; bitmap++)
            {
                Bitplanes[bitmap, y * WidthInBytes + byteXPos] |=  (byte)(((color >> bitmap) & 0x00000001) << bitInByte);
            }
        }
        public void SaveBitmapsAsAssemblerSourceCode (string fileName)
        {
            int BytesInLine = 0;

            StreamWriter streamWriter = new StreamWriter(fileName);

            streamWriter.Write("\tDC.B ");
            for (int y = 0; y < bmp.Height; y++)
                for (int bitpmap = 0; bitpmap < NumOfBitmaps; bitpmap++)
                    for (int x = 0; x < bmp.Width / 8; x++)
                    {

                        streamWriter.Write($"${Bitplanes[bitpmap, y * bmp.Width / 8 + x]:X2}");
                        if (++BytesInLine == 8)
                        {
                            streamWriter.Write($"{Environment.NewLine}\tDC.B ");
                            BytesInLine = 0;
                        }
                        else
                            streamWriter.Write(",");

                    }

            streamWriter.Close();
        }
        
        public void SaveBitmapsAsAssemblerSourceCode(string fileName, OutputSize outputSize, int numInRaw = 8)
        {
            
            StreamWriter streamWriter = new StreamWriter(fileName);
            int NumInARaw = 0;

            streamWriter.WriteLine(";-------------------------------------------------------------------------------------------------------");
            streamWriter.WriteLine(";                Converted using AmigaGraphicsToolsKit by Yossi Shelly");
            streamWriter.WriteLine($";  Image Width: {NewWidth}, Height {Height}, bpp: {NumOfBitmaps}, Width In Bytes: {NewWidth/8}");
            streamWriter.WriteLine(";-------------------------------------------------------------------------------------------------------\r\n");
            streamWriter.WriteLine("\tSECTION bitmap,DATA_C");
            switch (outputSize)
            {
                case OutputSize.Byte:
                    byte oneByte;
                    int size = Bitplanes.GetLength(1);
                    streamWriter.WriteLine("\tEVEN\r\nBitMap:");
                    streamWriter.Write("\tdc.b\t");
                    for (int b = 0; b < NumOfBitmaps; b++)
                        for (int i = 0; i < Bitplanes.GetLength(1); i++)
                        {
                            oneByte = Bitplanes[b, i];
                            streamWriter.Write($"${oneByte:X2}");
                           if (b*size+i < (NumOfBitmaps * size)-1)
                            {
                                if (++NumInARaw == numInRaw)
                                {
                                    streamWriter.Write($"{Environment.NewLine}\tdc.b\t");
                                    NumInARaw = 0;
                                }
                                else
                                    streamWriter.Write(",");
                            }
                        }
                    break;
                case OutputSize.Word:
                    ushort oneWord;
                    int SizeInWords = Bitplanes.Length % 2 == 0 ? Bitplanes.Length / 2 : Bitplanes.Length / 2 + 1;
                    ushort[] BitplanesWord = new ushort[SizeInWords];
                    Buffer.BlockCopy(Bitplanes,0,BitplanesWord,0,Bitplanes.Length);
                    streamWriter.WriteLine("\tEVEN\r\nBitMap:");
                    streamWriter.Write("\tdc.w\t");
                    for (int i = 0; i < BitplanesWord.Length; i++)
                    {
                        oneWord = BitplanesWord[i];
                        streamWriter.Write($"${endian.Convert((ushort)oneWord):X4}");
                        if (i < BitplanesWord.Length - 1)
                        {
                            if (++NumInARaw == numInRaw)
                            {
                                streamWriter.Write($"{Environment.NewLine}\tdc.w\t");
                                NumInARaw = 0;
                            }
                            else
                                streamWriter.Write(",");
                        }
                    }
                    break;
                case OutputSize.Long:
                    uint oneLong;
                    int SizeInLongs = Bitplanes.Length % 4 == 0 ? Bitplanes.Length / 4 : Bitplanes.Length / 4 + 1;
                    uint[] BitplanesLong = new uint[SizeInLongs];
                    Buffer.BlockCopy(Bitplanes, 0, BitplanesLong, 0, Bitplanes.Length);
                    streamWriter.WriteLine("\tCNOP 0,4\r\nBitMap:");
                    streamWriter.Write("\tdc.l\t");
                    for (int i = 0; i < BitplanesLong.Length; i++)
                    {
                        oneLong = BitplanesLong[i];
                        streamWriter.Write($"${endian.Convert(oneLong):X8}");
                        if (i < BitplanesLong.Length - 1)
                        {
                            if (++NumInARaw == numInRaw)
                            {
                                streamWriter.Write($"{Environment.NewLine}\tdc.l\t");
                                NumInARaw = 0;
                            }
                            else
                                streamWriter.Write(",");
                        }
                    }
                    break;
            }
            streamWriter.Flush();
            streamWriter.Close();
        }

        public void SaveBitmapsAsInterleavedAssemblerSourceCode (string fileName, OutputSize outputSize, int numInRaw = 8)
        {

            StreamWriter streamWriter = new StreamWriter(fileName);
            int NumInARaw = 0;

            streamWriter.WriteLine("\tSECTION bitmap,DATA_C");
            switch (outputSize)
            {
                case OutputSize.Byte:
                    byte oneByte;
                    int size = Bitplanes.GetLength(1);
                    streamWriter.Write("\tdc.b\t");
                    for (int y = 0; y < Height; y++)
                    for (int b = 0; b < NumOfBitmaps; b++)
                        for (int i = 0; i < Width/8; i++)
                        {
                            oneByte = Bitplanes[b, y * (Width/8) + i];
                            streamWriter.Write($"${oneByte:X2}");
                            if (b * Bitplanes.GetLength(1) + y * (Width/8) + i < (NumOfBitmaps * size) - 1)
                            {
                                if (++NumInARaw == numInRaw)
                                {
                                    streamWriter.Write($"{Environment.NewLine}\tdc.b\t");
                                    NumInARaw = 0;
                                }
                                else
                                    streamWriter.Write(",");
                            }
                        }
                    break;
                case OutputSize.Word:
                    ushort oneWord;
                    ushort[] BitplanesWord = new ushort[Bitplanes.Length / 2];
                    Buffer.BlockCopy(Bitplanes, 0, BitplanesWord, 0, Bitplanes.Length);
                    streamWriter.WriteLine("\tEVEN\r\nBitMap:");
                    streamWriter.Write("\tdc.w\t");
                    for (ushort y = 0; y < Height; y++) 
                        for (ushort b = 0; b < NumOfBitmaps; b++)
                            for (ushort i = 0; i < Width/16; i++)
                            {
                                oneWord = BitplanesWord[b*(Bitplanes.GetLength(1)/2) + y * (Width/16) + i];
                                streamWriter.Write($"${endian.Convert((ushort)oneWord):X4}");
                                if (b * (Bitplanes.GetLength(1) / 2) + y * (Width / 16) + i < BitplanesWord.Length - 1)
                                {
                                    if (++NumInARaw == numInRaw)
                                    {
                                        streamWriter.Write($"{Environment.NewLine}\tdc.w\t");
                                        NumInARaw = 0;
                                    }
                                    else
                                        streamWriter.Write(",");
                                }
                            }
                    break;
                case OutputSize.Long:
                    uint oneLong;
                    uint[] BitplanesLong = new uint[Bitplanes.Length / 4];
                    Buffer.BlockCopy(Bitplanes, 0, BitplanesLong, 0, Bitplanes.Length);
                    streamWriter.WriteLine("\tCNOP 0,4\r\nBitMap:");
                    streamWriter.Write("\tdc.l\t");
                    for (ushort y = 0; y < Height; y++)
                        for (ushort b = 0; b < NumOfBitmaps; b++)
                            for (ushort i = 0; i < Width/32; i++)
                            {
                                oneLong = BitplanesLong[b * (Bitplanes.GetLength(1) / 4) + y * (Width / 32) + i];
                                streamWriter.Write($"${endian.Convert(oneLong):X8}");
                                if (b * (Bitplanes.GetLength(1) / 4) + y * (Width / 32) + i < BitplanesLong.Length - 1)
                                {
                                    if (++NumInARaw == numInRaw)
                                    {
                                        streamWriter.Write($"{Environment.NewLine}\tdc.l\t");
                                        NumInARaw = 0;
                                    }
                                    else
                                        streamWriter.Write(",");
                                }
                            }
                    break;
            }
            streamWriter.Flush();
            streamWriter.Close();
        }

        public void SaveBitmapsAsInterleavedCPPSourceCode(string fileName, OutputSize outputSize, int numInRaw = 8)
        {

            StreamWriter streamWriter = new StreamWriter(fileName);
            int NumInARaw = 0;

            
            switch (outputSize)
            {
                case OutputSize.Byte:
                    byte oneByte;
                    int size = Bitplanes.GetLength(1);
                    streamWriter.WriteLine("unsigned char ImageData[] = {");
                    streamWriter.Write("\t");
                    for (int y = 0; y < Height; y++)
                        for (int b = 0; b < NumOfBitmaps; b++)
                            for (int i = 0; i < Width / 8; i++)
                            {
                                oneByte = Bitplanes[b, y * (Width / 8) + i];
                                streamWriter.Write($"0x{oneByte:X2}");
                                if (b * Bitplanes.GetLength(1) + y * (Width / 8) + i < (NumOfBitmaps * size) - 1)
                                {
                                    streamWriter.Write(",");

                                    if (++NumInARaw == numInRaw)
                                    {
                                        streamWriter.Write($"{Environment.NewLine}\t");
                                        NumInARaw = 0;
                                    }
                                        
                                }
                            }
                    streamWriter.Write("};");
                    break;
                case OutputSize.Word:
                    ushort oneWord;
                    ushort[] BitplanesWord = new ushort[Bitplanes.Length / 2];
                    Buffer.BlockCopy(Bitplanes, 0, BitplanesWord, 0, Bitplanes.Length);
                    streamWriter.WriteLine("unsigned short ImageData[] = {");
                    streamWriter.Write("\t");
                    for (ushort y = 0; y < Height; y++)
                        for (ushort b = 0; b < NumOfBitmaps; b++)
                            for (ushort i = 0; i < Width / 16; i++)
                            {
                                oneWord = BitplanesWord[b * (Bitplanes.GetLength(1) / 2) + y * (Width / 16) + i];
                                streamWriter.Write($"0x{endian.Convert((ushort)oneWord):X4}");
                                if (b * (Bitplanes.GetLength(1) / 2) + y * (Width / 16) + i < BitplanesWord.Length - 1)
                                {
                                    streamWriter.Write(",");

                                    if (++NumInARaw == numInRaw)
                                    {
                                        streamWriter.Write($"{Environment.NewLine}\t");
                                        NumInARaw = 0;
                                    }                                                                  
                                }
                            }
                    streamWriter.Write("};");
                    break;
                case OutputSize.Long:
                    uint oneLong;
                    uint[] BitplanesLong = new uint[Bitplanes.Length / 4];
                    Buffer.BlockCopy(Bitplanes, 0, BitplanesLong, 0, Bitplanes.Length);
                    streamWriter.WriteLine("unsigned int ImageData[] = {");
                    streamWriter.Write("\t");
                    for (ushort y = 0; y < Height; y++)
                        for (ushort b = 0; b < NumOfBitmaps; b++)
                            for (ushort i = 0; i < Width / 32; i++)
                            {
                                oneLong = BitplanesLong[b * (Bitplanes.GetLength(1) / 4) + y * (Width / 32) + i];
                                streamWriter.Write($"0x{endian.Convert(oneLong):X8}");
                                if (b * (Bitplanes.GetLength(1) / 4) + y * (Width / 32) + i < BitplanesLong.Length - 1)
                                {
                                    streamWriter.Write(",");

                                    if (++NumInARaw == numInRaw)
                                    {
                                        streamWriter.Write($"{Environment.NewLine}\t");
                                        NumInARaw = 0;
                                    }                                                                        
                                }
                            }
                    streamWriter.Write("};");
                    break;
            }
            streamWriter.Flush();
            streamWriter.Close();
        }


        public void SaveBitmapsAsCPPSourceCode(string fileName, OutputSize outputSize, int numInRaw = 8)
        {

            StreamWriter streamWriter = new StreamWriter(fileName);
            int NumInARaw = 0;

            switch (outputSize)
            {
                case OutputSize.Byte:
                    byte oneByte;
                    int size = Bitplanes.GetLength(1);
                    streamWriter.WriteLine("unsigned char ImageData[] = {");
                    streamWriter.Write("\t");
                    for (int b = 0; b < NumOfBitmaps; b++)
                        for (int i = 0; i < Bitplanes.GetLength(1); i++)
                        {
                            oneByte = Bitplanes[b, i];
                            streamWriter.Write($"0x{oneByte:X2}");
                            if (b * size + i < (NumOfBitmaps * size) - 1)
                            {
                                streamWriter.Write(",");

                                if (++NumInARaw == numInRaw)
                                {
                                    streamWriter.Write($"{Environment.NewLine}\t");
                                    NumInARaw = 0;
                                }                                 
                            }
                        }
                    streamWriter.Write("};");
                    break;
                case OutputSize.Word:
                    ushort oneWord;
 //                   int Width = Stride * 2;
 //                   int WidthInBytes = Width / 8;
 //                   int WidthInWords = WidthInBytes / 2;

                    streamWriter.WriteLine("unsigned short ImageData[] = {");
                    streamWriter.Write("\t");


                    // Do this in case Width devided by 16 bits (WORD ALIGNED)
                    //int WordWidth = Width % 16 == 0 ? Width / 16 : (Width / 16) + 1;
                    ushort[] BitplanesWord = new ushort[Bitplanes.Length / 2];
                    Buffer.BlockCopy(Bitplanes, 0, BitplanesWord, 0, Bitplanes.Length);
                     
                    for (int i = 0; i < BitplanesWord.Length; i++)
                    {
                        oneWord = BitplanesWord[i];
                        streamWriter.Write($"0x{endian.Convert((ushort)oneWord):X4}");
                        if (i < BitplanesWord.Length - 1)
                        {
                            streamWriter.Write(",");

                            if (++NumInARaw == numInRaw)
                            {
                                streamWriter.Write($"{Environment.NewLine}\t");
                                NumInARaw = 0;
                            }
                        }
                    }
                    
                    streamWriter.Write("};");
                    break;
                case OutputSize.Long:
                    uint oneLong;
                    uint[] BitplanesLong = new uint[Bitplanes.Length / 4];
                    Buffer.BlockCopy(Bitplanes, 0, BitplanesLong, 0, Bitplanes.Length);
                    streamWriter.WriteLine("unsigned int ImageData[] = {");
                    streamWriter.Write("\t");
                    for (int i = 0; i < BitplanesLong.Length; i++)
                    {
                        oneLong = BitplanesLong[i];
                        streamWriter.Write($"0x{endian.Convert(oneLong):X8}");
                        if (i < BitplanesLong.Length - 1)
                        {
                            streamWriter.Write(",");

                            if (++NumInARaw == numInRaw)
                            {
                                streamWriter.Write($"{Environment.NewLine}\t");
                                NumInARaw = 0;
                            }                  
                        }
                    }
                    streamWriter.Write("};");
                    break;
            }
            streamWriter.Flush();
            streamWriter.Close();
        }

        private ushort ConvertRGBFrom32To12Bit(Color color)
        {
            return (ushort)(((((color.R / 16) & 0x0f) << 8) | ((color.G / 16) & 0x0f) << 4 | ((color.B / 16) & 0x0f) & 0x0fff));
        }

        new public byte GetPixel (int x, int y)
        {

            int bytePtr = y * WidthInBytes + x / 8;
            byte bit = (byte)(7 - (x % 8));
            byte bitMask = (byte)( 1 << bit);
            byte ColorReg = 0;

            for (int bpl=0; bpl < NumOfBitmaps; bpl++)
            {
                byte pixelBit = (byte)(Bitplanes[bpl, bytePtr] & bitMask);
                byte shiftedByte = (byte)(((pixelBit >> bit)<<bpl));
                ColorReg |= shiftedByte;    
            }

            return ColorReg;
        }

        new public void SetPixel (int x, int y, byte colorReg)
        {

            int bytePtr = y * WidthInBytes + x / 8;
            byte bit = (byte)(7 - (x % 8));


            for (int bpl = 0; bpl < NumOfBitmaps; bpl++)
            {
                Bitplanes[bpl, bytePtr] &= (byte)~(1 << bit);
                byte colorBitMask = (byte)((colorReg & (1 << bpl)) >> bpl);
                Bitplanes[bpl, bytePtr] |=  (byte)(colorBitMask << bit);
                
            }

            if (imageGrid != null)
                imageGrid.SetPixel(x, y, colors[colorReg]);

        }

        public void SwapColorsPosition (byte colorReg1, byte colorReg2 )
        {

        }

        public void SetBackgroundColor (byte paletteIndex)
        {
            Color colorFromIdxZero = Pallate[0];
            Color colorToBeBackGround = Pallate[paletteIndex];
            Pallate[0] = colorToBeBackGround;
            Pallate [paletteIndex] = colorFromIdxZero;


            for (int y = 0; y < Height; y++)
                for (int x = 0; x < WidthInBytes; x++)
                {
                    if (GetPixel(x,y) == 0)
                    {
                        SetPixel(x, y, paletteIndex);
                    }
                    else if (GetPixel(x,y) == paletteIndex)
                        SetPixel(x, y, 0);
                }
        }

        public void SelectBackgroundColor (byte newBGColor)
        {
            Color oldBGColor = Pallate[0];
            Pallate[0] = Pallate[newBGColor];
            Pallate[newBGColor] = oldBGColor;

            for (int y=0; y<Height; y++)
                for (int x=0; x<Width; x++) 
                {
                    byte colorIdx = GetPixel(x,y);
                    if (colorIdx == newBGColor)
                    {
                        SetPixel (x, y, 0);
                    }
                    else if (colorIdx == 0)
                    {
                        SetPixel (x, y, newBGColor);
                    }
                                      
                }


        }
    

        public void CreateMask ()
        {
            Bitmap Mask = new Bitmap(Width, Height);
            Graphics g = Graphics.FromImage(Mask);
            g.Clear(Color.Black);

            ImageMask = new byte[WidthInBytes * Height];
            for (int y = 0; y < bitmap.Height; y ++)
            {
                for (int x = 0; x < bitmap.Width; x ++)
                {
                    byte pixel = GetPixel(x, y);
                    if (pixel!= 0)
                    {
                        SetMaskBit(x, y);
                        Mask.SetPixel(x, y, Color.White);
                    }

                }
            }

            this.Mask = Mask;
            g.Dispose();
        }

        public void CreateMaskBitmap ()
        {
            Bitmap Mask = new Bitmap (Width, Height);
            Graphics g = Graphics.FromImage(Mask);
            g.Clear(Color.Black);

            for (int y=0; y<Height; y++)
                for (int x=0; x<Width ; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    if (pixel != Pallate[0])
                    {
                        Mask.SetPixel(x, y, Color.White);
                        SetMaskBit(x, y);
                    }
                }

            this.Mask = Mask;
            g.Dispose();
        }

        public void SetMaskBit (int x, int y)
        {
            ImageMask[y * WidthInBytes + (x / 8)] |= (byte)(1 << (x % 8));

        }

        public byte GetMaskBit (int x, int y)
        {
            return (byte)(ImageMask[y * WidthInBytes + (x / 8)] & (1 << (x % 8))); 
        }

        public void SaveMaskAs (OutputSize outputSize = OutputSize.Byte)
        {
            SaveFileDialog maskSaveDlg = new SaveFileDialog();
            maskSaveDlg.Filter = "Assembler Source|*.asm;*.s|C/C++ Source|*.c;*.cpp|Binary file|*.bin";
            if (maskSaveDlg.ShowDialog() == DialogResult.OK)
            {

                if (Mask != null)
                {
                    switch (maskSaveDlg.FilterIndex)
                    {
                   
                        case 1:
                            SaveMaskAsAssemblerSource(maskSaveDlg.FileName,outputSize);
                            break;
                        case 2:
                            SaveMaskAsCSource(maskSaveDlg.FileName,outputSize);
                            break;

                    }
                }
            }
            
        }

        public void SaveMaskAsAssemblerSource(string fileName, OutputSize dataWidth)
        {
            StreamWriter maskFile = new StreamWriter(fileName);
            
            maskFile.Write("Mask: ");

            switch (dataWidth)
            {
                case OutputSize.Byte:
                    for (int i = 0; i < ImageMask.Length; i++)
                    {
                        if (i % 8 == 0)
                        {
                            maskFile.Write("\r\n\tDC.B\t");
                        }
                        else maskFile.Write(",");
                        maskFile.Write($"${ImageMask[i]:X2}");


                    }
                    break;
                case OutputSize.Word:
                    int NumOfWords = (int)Math.Ceiling(ImageMask.Length / 2f);
                    ushort[] Mask = new ushort[NumOfWords];
                    Buffer.BlockCopy(ImageMask, 0,Mask, 0,ImageMask.Length);
                    
                    for (int i = 0; i < Mask.Length; i++)
                    {
                        if (i % 8 == 0)
                        {
                            maskFile.Write("\r\n\tDC.W\t");
                        }
                        else maskFile.Write(",");
                        maskFile.Write($"${endian.Convert(Mask[i]):X4}");


                    }

                    break;
                case OutputSize.Long:
                    int NumOfLongs = (int)Math.Ceiling(ImageMask.Length / 4f);
                    uint[] LongMask = new uint[NumOfLongs];
                    Buffer.BlockCopy(ImageMask, 0, LongMask, 0, ImageMask.Length);

                    for (int i = 0; i < LongMask.Length; i++)
                    {
                        if (i % 8 == 0)
                        {
                            maskFile.Write("\r\n\tDC.W\t");
                        }
                        else maskFile.Write(",");
                        maskFile.Write($"${endian.Convert(LongMask[i]):X8}");


                    }
                    break;
            }

        
            maskFile.Flush();
            maskFile.Close();
                
        }

        public void SaveMaskAsCSource(string fileName, OutputSize dataWidth)
        {
            StreamWriter maskFile = new StreamWriter(fileName);

        

            switch (dataWidth)
            {
                case OutputSize.Byte:
                    maskFile.Write("UBYTE Mask[] {\r\n\t");

                    for (int i = 0; i < ImageMask.Length; i++)
                    {
                        maskFile.Write($"0x{ImageMask[i]:X2}");
                        if (i < ImageMask.Length - 1)
                        {
                            if ((i + 1) % 8 == 0)
                            {
                                maskFile.Write(",\r\n\t");
                            }
                            else maskFile.Write(",");
                        }

                    }
                    break;
                case OutputSize.Word:
                    maskFile.Write("UWORD Mask[] {\r\n\t");


                    int NumOfWords = (int)Math.Ceiling(ImageMask.Length / 2f);
                    ushort[] Mask = new ushort[NumOfWords];
                    Buffer.BlockCopy(ImageMask, 0, Mask, 0, ImageMask.Length);

                    for (int i = 0; i < Mask.Length; i++)
                    {
                        maskFile.Write($"0x{endian.Convert(Mask[i]):X4}");
                        if (i < Mask.Length - 1)
                        {
                            if ((i + 1) % 8 == 0)
                            {
                                maskFile.Write(",\r\n\t");
                            }
                            else maskFile.Write(",");
                        }
                    }

                        break;
                case OutputSize.Long:
                    maskFile.Write("ULONG Mask[] {\r\n\t");

                    int NumOfLongs = (int)Math.Ceiling(ImageMask.Length / 4f);
                    uint[] LongMask = new uint[NumOfLongs];
                    Buffer.BlockCopy(ImageMask, 0, LongMask, 0, ImageMask.Length);

                    for (int i = 0; i < LongMask.Length; i++)
                    {
                        maskFile.Write($"0x{endian.Convert(LongMask[i]):X8}");
                        if (i < LongMask.Length - 1)
                        {
                            if ((i + 1) % 8 == 0)
                            {
                                maskFile.Write(",\r\n\t");
                            }
                            else maskFile.Write(",");
                        }

                    }
                    break;
            }

            maskFile.WriteLine ( "};");
            maskFile.Flush();
            maskFile.Close();

        }

        public void LoadIFF (string fileName)
        {
            Load(fileName); 
            NumOfBitmaps = bmhd.numPlanes;
            CreateBitmap();
        }

        public void SaveIFF()
        { 

        }

        public override void SaveILBM(string filename)
        {
            PopulateBMHD();

            if (WidthInBytes % 2 != 0)
            {
                int newWidth = (int)WidthInBytes + 1;
                SetBufferWidthToWords(newWidth);
            }

            base.SaveILBM(filename);
        }
        public override void SaveACBM(string filename)
        {
            PopulateBMHD();
            if (WidthInBytes % 2 != 0)
            {
                int newWidth = (int)WidthInBytes + 1;
                SetBufferWidthToWords(newWidth);
            }
            base.SaveACBM(filename);
        }
        
        private void PopulateBMHD ()
        {
            bmhd.width = (ushort)bmp.Width;
            bmhd.height = (ushort)bmp.Height;
            bmhd.xOrigin= 0;
            bmhd.yOrigin= 0;
            bmhd.numPlanes = NumOfBitmaps;
            bmhd.mask = 0;  // for now , need to add it later
            bmhd.compression= 0; // for now...
            bmhd.pad1 = 0;
            bmhd.transClr = 0;
            bmhd.xAspect  = 1; 
            bmhd.yAspect = 1;
            bmhd.pageWidth= (ushort)bmp.Width;
            bmhd.pageHeight= (ushort)bmp.Height;

        }

        private void SetBufferWidthToWords (int NewWidth)
        {
            byte[,] NewBitplanes = new byte[NumOfBitmaps, NewWidth * Height];

            for (int bp = 0; bp < NumOfBitmaps; bp++) 
            {
                for (int y=0; y < Height; y++) 
                {
                    for (int x =0; x < WidthInBytes; x++) 
                    {
                        NewBitplanes[bp,y * NewWidth + x] = Bitplanes[bp,y * WidthInBytes + x];
                    }
                }

            }
            WidthInBytes= NewWidth;
            Bitplanes = NewBitplanes;
        }

    }
}
