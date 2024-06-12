using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;

namespace GfxLib
{
    internal class GLCD
    {
        public enum LanguageFormat { C, Asm, Bin };

        public enum Orientation { Vertical, Horizontal };
        void SaveImageAsSourceFile (Bitmap bitmap, string filename)
        {
     
        }

        void SaveImageAsSourceFile (Bitmap bitmap)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                saveFileDialog.Filter = "C Source Code|*.c|Asm Source COde|*.a|*.s|Binary File|*.bin";

                switch (saveFileDialog.FilterIndex)
                {
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3: 
                        break;
                }
            }
        }

   /*     private Boolean generateCfile(String arrayName,bool compression = false,string asmDirective,Orientation orientation)
        {
            StreamWriter stream;
            SaveFileDialog Sdlg = new SaveFileDialog();
            bool success = false;

            Sdlg.Filter = "C/CPP Source File|*.c;*.cpp";


            if (Sdlg.ShowDialog() == DialogResult.OK)
            {
                // if (settings.RLEcompressionCB.CheckState == CheckState.Checked) compression.RLECompress (bitmapFile.RawFormat;


                try
                {
                    stream = new StreamWriter(Sdlg.FileName);


                    stream.WriteLine("// Generated using GLCDToolskit by Yossi Shelly \u00A9Copyright");
                    stream.WriteLine("// The data is stream of pages of vertical bytes exactly as needed for GLCD screens");
                    stream.WriteLine("// Image: Monochrome 128x64 (128x8 pages) 8 pages = 64/8");
                    stream.WriteLine("// Data format: page 1: (vb1,vb2,vb3..... vb128) page 2: (vb1,vb2.... vb12).....page 8()");
                    stream.WriteLine();

                    String OnlyFileName = Sdlg.FileName.Substring(Sdlg.FileName.LastIndexOf("\\") + 1);
                    OnlyFileName = OnlyFileName.Substring(0, OnlyFileName.IndexOf(".")).Replace(" ", "_");

                   
                    stream.Write("const unsigned char " + arrayName + "[] ");
                   
                    stream.WriteLine("= {");
                    if (compression)
                        RLECompressAndCconvert(stream);
                    else
                        C_ConvertToFile(stream);


                    stream.WriteLine();
                    stream.Close();

                    success = true;
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("You are not allowed to access this file", "Access deniged");
                    success = false;
                }
            }
            return success;
        }

        private void RLECompressAndCconvert(Image image,StreamWriter stream,LanguageFormat languageFormat,string lable,string asmDirective,Orientation orientation)
        {
            byte BytesPerLine = 0;
            int TotalBytes = 0;
            byte Counter = 0;
            if (languageFormat == LanguageFormat.Asm)
            {
                stream.WriteLine(l + ":");
                stream.Write(asmDirective);
            }
            byte ByteToCount;
            if (orientation == Orientation.Vertical)
            {
                ByteToCount = image.VerticalByte(0, 0);
                for (byte page = 0; page < image.bitmap.Height / 8; ++page)
                {
                    for (byte X = 0; X < image.bitmap.Width; ++X)
                    {
                        byte VerticalByte;
                        if ((int)(VerticalByte = image.VerticalByte(X, page)) == ByteToCount)
                        {
                            ++Counter;
                        }
                        else
                        {
                            if (languageFormat == LanguageFormat.C)
                                stream.Write("0x{0:X2},0x{1:X2},", Counter, ByteToCount);
                            else if (languageFormat == LanguageFormat.Asm)
                                stream.Write("{0:X2},{1:X2},", Counter, ByteToCount);

                            Counter = 1;
                            ByteToCount = VerticalByte;
                            TotalBytes += 2;
                            BytesPerLine += 2;

                            if (BytesPerLine == 16)
                            {
                                stream.WriteLine();
                                if (languageFormat == LanguageFormat.Asm)
                                    stream.Write(asmDirective);
                                BytesPerLine = 0;
                            }
                        }
                    }
                }
            }
            else
            {
                ByteToCount = image.HorizontalByte((byte)0, (byte)0);
                for (byte Ypos = 0; Ypos < image.bitmap.Height; ++Ypos)
                {
                    for (byte Xpos = 0; Xpos < image.bitmap.Width / 8; ++Xpos)
                    {
                        byte HorizontalByte;
                        if ((int)(HorizontalByte = image.HorizontalByte(Xpos, Ypos)) == ByteToCount)
                        {
                            ++Counter;
                        }
                        else
                        {
                            if (languageFormat == LanguageFormat.C)
                                stream.Write("0x{0:X2},0x{1:X2},", Counter, ByteToCount);
                            else if (languageFormat == LanguageFormat.Asm)
                                stream.Write("{0:X2},{1:X2},", Counter, ByteToCount);

                            Counter = 1;
                            ByteToCount = HorizontalByte;
                            TotalBytes += 2;
                            BytesPerLine += 2;

                            if (BytesPerLine == 16)
                            {
                                stream.WriteLine();
                                if (languageFormat == LanguageFormat.Asm)
                                    stream.Write(this.settings.AssemblerDirective);
                                BytesPerLine = (byte)0;
                            }
                        }
                    }
                }
            }
            if (languageFormat == LanguageFormat.C)
                stream.Write("0x{0:X2},0x{1:X2}", Counter, ByteToCount);
            else if (languageFormat == LanguageFormat.Asm)
                stream.Write("{0:X2},{1:X2}", Counter, ByteToCount);

            TotalBytes += 2;

            if (languageFormat ==  LanguageFormat.C)
            {
                stream.Write("};");
    
                stream.WriteLine("\r\n\r\n");
                stream.WriteLine("struct header_t {");
                stream.WriteLine("  unsigned char Width = {0}, Hight = {1};", (object)GLCDToolsKitForm.image.bitmap.Width, (object)GLCDToolsKitForm.image.bitmap.Height);
                stream.WriteLine("  unsigned int RleLength = {0}", TotalBytes);
                stream.WriteLine("} header;");
                
            }
        }*/

    }
}
