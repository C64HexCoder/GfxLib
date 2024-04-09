using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System;


namespace GfxLib
{
    public class IFF
    {
        Endian endian = new Endian();
        uint viewportMode;
        byte[] BodyFormat;
        public byte[,] Bitplanes;
        public Bitmap bmp;
        Stream stream;
        BinaryReader IFFReader;
        BinaryWriter IFFWriter;
        protected int WidthInBytes;

        struct Chunk
        {
            int FourCC;
            int Length;
            byte[] Data;
        }

        public Color[] palette;

        public struct BMHD
        {
            public ushort width;
            public ushort height;
            public ushort xOrigin;
            public ushort yOrigin;
            public byte numPlanes;
            public byte mask;
            public byte compression;
            public byte pad1;
            public ushort transClr;
            public byte xAspect;
            public byte yAspect;
            public ushort pageWidth;
            public ushort pageHeight;

        }

        public BMHD bmhd;

        public void Load(string fileName)
        {
            byte[] ChunkId;


            stream = File.OpenRead(fileName);
            IFFReader = new BinaryReader(stream);


            ChunkId = IFFReader.ReadBytes(4);
            string s1 = ChunkId.ToString();

            if (CheckFourCC(ChunkId, "FORM"))
            {
                uint FormSize = endian.Convert((uint)(IFFReader.ReadInt32()));
                int ChunkSize = 0;

                BodyFormat = IFFReader.ReadBytes(4);

                if (CheckFourCC(BodyFormat, "ILBM") || CheckFourCC(BodyFormat, " PBM"))
                {
                    // bool FoundBMHD = false;
                    do
                    {
                        ChunkId = IFFReader.ReadBytes(4);
                        if (CheckFourCC(ChunkId, "BMHD"))
                        {
                            ChunkSize = endian.Convert((int)(IFFReader.ReadInt32()));
                            ReadBMHD();
                        }
                        else if (CheckFourCC(ChunkId, "CMAP"))
                        {
                            ChunkSize = ReadPalette();

                        }
                        else if (CheckFourCC(ChunkId, "CAMG"))
                        {
                            ChunkSize = endian.Convert((int)(IFFReader.ReadInt32()));
                            viewportMode = endian.Convert(IFFReader.ReadUInt32());
                        }
                        else if (CheckFourCC(ChunkId, "BODY"))
                        {
                            ChunkSize = endian.Convert((int)(IFFReader.ReadInt32()));

                            if (CheckFourCC(BodyFormat, "ILBM"))
                            {
                                ExtractILBM();
                            }

                            else if (CheckFourCC(BodyFormat, "PBM "))
                            {
                                ExtractPBM();
                            }
                        }
                        else
                        {
                            ChunkSize = endian.Convert((int)(IFFReader.ReadInt32()));
                            //iffFile.ReadBytes(ChunkSize+1);
                            stream.Seek(ChunkSize, SeekOrigin.Current);
                            if ((stream.Position & 0x01) == 1)
                                stream.Seek(1, SeekOrigin.Current);
                        }


                    } while (stream.Position < FormSize - 12);

                }
                else if (CheckFourCC(BodyFormat, "ACBM"))
                {
                    do
                    {
                        ChunkId = IFFReader.ReadBytes(4);
                        if (CheckFourCC(ChunkId, "BMHD"))
                        {
                            ChunkSize = ReadChunkSize();
                            ReadBMHD();
                        }
                        else if (CheckFourCC(ChunkId, "CMAP"))
                        {
                            ChunkSize = ReadPalette();

                        }
                        else if (CheckFourCC(ChunkId, "CAMG"))
                        {
                            ChunkSize = endian.Convert((int)(IFFReader.ReadInt32()));
                            viewportMode = endian.Convert(IFFReader.ReadUInt32());
                        }
                        else if (CheckFourCC(ChunkId, "ABIT"))
                        {
                            ChunkSize = ReadChunkSize();
                            ExtractACBM();

                        }
                        else
                        {
                            ChunkSize = endian.Convert((int)(IFFReader.ReadInt32()));
                            //iffFile.ReadBytes(ChunkSize+1);
                            stream.Seek(ChunkSize, SeekOrigin.Current);
                            if ((stream.Position & 0x01) == 1)
                                stream.Seek(1, SeekOrigin.Current);
                        }
                    } while (stream.Position < FormSize - 12);
                }
                else
                {
                    MessageBox.Show("The file is not Image file.", "File Error", MessageBoxButtons.OK);

                }
            }
            else
            {
                MessageBox.Show("The file is not IFF file.", "File Error", MessageBoxButtons.OK);

            }


        }

        private void ConvertInterleavedToNormal(byte[] bytrBitplane)
        {
            //     int WidthInWords = (bmhd.width + 15) / 16;
            //     WidthInBytes = WidthInWords * 2;

            int bp = 0;

            for (int y = 0; y < bmhd.height * bmhd.numPlanes; y++)
            {
                bp = y % bmhd.numPlanes;

                for (int xb = 0; xb < WidthInBytes; xb++)
                {
                    Bitplanes[bp, y / bmhd.numPlanes * WidthInBytes + xb] = bytrBitplane[y * WidthInBytes + xb];

                }
            }

        }
        public byte GetPixel(int x, int y)
        {
            //       int WidthInBytes = ((bmhd.width + 15) >> 4) << 1;

            int bytePtr = y * WidthInBytes + x / 8;
            byte bit = (byte)(7 - (x % 8));
            byte bitMask = (byte)(1 << bit);
            byte ColorReg = 0;

            for (int bpl = 0; bpl < bmhd.numPlanes; bpl++)
            {
                byte pixelBit = (byte)(Bitplanes[bpl, bytePtr] & bitMask);
                byte shiftedByte = (byte)(((pixelBit >> bit) << bpl));
                ColorReg |= shiftedByte;
            }

            return ColorReg;
        }

        public void SetPixel(int x, int y, byte colorReg)
        {
            //     int WidthInBytes = ((bmhd.width + 15) >> 4) << 1;

            int bytePtr = y * WidthInBytes + x / 8;
            byte bit = (byte)(7 - (x % 8));


            for (int bpl = 0; bpl < bmhd.numPlanes; bpl++)
            {
                Bitplanes[bpl, bytePtr] &= (byte)~(1 << bit);
                byte colorBitMask = (byte)((colorReg & (1 << bpl)) >> bpl);
                Bitplanes[bpl, bytePtr] |= (byte)(colorBitMask << bit);

            }


        }

        public Bitmap CreateBitmap()
        {
            //      int WidthInWords = (bmhd.width + 15) / 16;

            bmp = new Bitmap(bmhd.width, bmhd.height);

            for (int y = 0; y < bmhd.height; y++)
                for (int x = 0; x < bmhd.width; x++)
                {
                    byte colorReg = GetPixel(x, y);
                    bmp.SetPixel(x, y, palette[colorReg]);

                }
            return bmp;
        }
        bool CheckFourCC(byte[] a1, string a2)
        {
            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;

            return true;
        }

        private void ReadBMHD()
        {
            bmhd.width = endian.Convert(IFFReader.ReadUInt16());
            bmhd.height = endian.Convert(IFFReader.ReadUInt16());
            bmhd.xOrigin = endian.Convert(IFFReader.ReadUInt16());
            bmhd.yOrigin = endian.Convert(IFFReader.ReadUInt16());
            bmhd.numPlanes = IFFReader.ReadByte();
            bmhd.mask = IFFReader.ReadByte();
            bmhd.compression = IFFReader.ReadByte();
            bmhd.pad1 = IFFReader.ReadByte();
            bmhd.transClr = endian.Convert(IFFReader.ReadUInt16());
            bmhd.xAspect = IFFReader.ReadByte();
            bmhd.yAspect = IFFReader.ReadByte();
            bmhd.pageWidth = endian.Convert(IFFReader.ReadUInt16());
            bmhd.pageHeight = endian.Convert(IFFReader.ReadUInt16());

            //stream.Seek(1, SeekOrigin.Current);

            int WidthInBytes = ((bmhd.width + 15) >> 4) << 1;



        }

        private int ReadChunkSize()
        {
            return endian.Convert((int)(IFFReader.ReadInt32()));
        }

        private int ReadPalette()
        {
            int ChunkSize = endian.Convert((int)(IFFReader.ReadInt32()));
            palette = new Color[(int)Math.Pow(2, bmhd.numPlanes)];

            for (int x = 0; x < ChunkSize; x += 3)
            {
                byte Red = IFFReader.ReadByte();
                byte Green = IFFReader.ReadByte();
                byte Blue = IFFReader.ReadByte();

                palette[x / 3] = Color.FromArgb(Red, Green, Blue);
            }

            if ((stream.Position & 0x01) == 1)
                stream.Seek(1, SeekOrigin.Current);

            return ChunkSize;
        }
        private void ExtractILBM()
        {
            WidthInBytes = ((bmhd.width + 15) / 16) * 2;
            Bitplanes = new byte[bmhd.numPlanes, WidthInBytes * bmhd.height];

            switch (bmhd.compression)
            {
                case 0: // No Compression
                    for (int y = 0; y < bmhd.height; y++)
                        for (int bp = 0; bp < bmhd.numPlanes; bp++)
                            for (int x = 0; x < WidthInBytes; x++)
                            {
                                Bitplanes[bp, y * WidthInBytes + x] = IFFReader.ReadByte();
                            }
                    break;

                case 1: // RLE Compression
                    byte val;
                    byte[] DecompInterleaced = new byte[bmhd.numPlanes * WidthInBytes * bmhd.height];
                    bool Finished = false;
                    int FinalLangth = WidthInBytes * bmhd.height * bmhd.numPlanes;
                    int BitplaneIdx = 0;


                    while (BitplaneIdx < FinalLangth && !Finished)
                    {
                        val = IFFReader.ReadByte();
                        if (val > 128)
                        {
                            byte Data = IFFReader.ReadByte();

                            for (int i = 0; i < 257 - val; i++)
                                DecompInterleaced[BitplaneIdx + i] = Data;

                            BitplaneIdx += 257 - val;

                        }
                        else if (val < 128)
                        {
                            for (int i = 0; i < val + 1; i++)
                                DecompInterleaced[BitplaneIdx + i] = IFFReader.ReadByte();

                            BitplaneIdx += val + 1;
                        }
                        else if (val == 128)
                            Finished = true;            // CHANHE TO SKIP   


                    }
                    ConvertInterleavedToNormal(DecompInterleaced);
                    break;

                case 2: // Vertical RLE Compression
                    break;
            }


        }

        private void ExtractPBM()
        {

        }

        private void ExtractACBM()
        {
            WidthInBytes = ((bmhd.width + 15) >> 4) << 1;
            Bitplanes = new byte[bmhd.numPlanes, WidthInBytes * bmhd.height];

            for (int bp = 0; bp < bmhd.numPlanes; bp++)
            {
                for (int y = 0; y < bmhd.height; y++)
                {
                    for (int x = 0; x < WidthInBytes; x++)
                    {
                        Bitplanes[bp, y * WidthInBytes + x] = IFFReader.ReadByte();
                    }
                }
            }
        }

        public virtual void SaveACBM(string filename)
        {
            Stream IFFFile = File.OpenWrite(filename);
            IFFWriter = new BinaryWriter(IFFFile);

            WriteFourCC("FORM");
            uint FORMSizePos = (uint)IFFWriter.BaseStream.Position;        // Remember the position to update it later
            uint FORMSize = 0;// = (uint)(0x14+palette.Length*3+Bitplanes.Length+28);

            IFFWriter.Write(endian.Convert((uint)FORMSize));

            WriteFourCC("ACBMBMHD");
            uint BMHDSizePos = (uint)IFFWriter.BaseStream.Position;
            IFFWriter.Write(endian.Convert((uint)0x14));
            IFFWriter.Write(endian.Convert((ushort)bmp.Width));   //
            IFFWriter.Write(endian.Convert((ushort)bmp.Height));

            IFFWriter.Write((ushort)0);               //x
            IFFWriter.Write((ushort)0);               //y

            IFFWriter.Write((byte)bmhd.numPlanes);   // num of bitplanes
            IFFWriter.Write((byte)bmhd.mask);   // num of bitplanes
            IFFWriter.Write((byte)bmhd.compression);   // num of bitplanes
            IFFWriter.Write((byte)0);   // num of bitplanes

            IFFWriter.Write(endian.Convert((ushort)bmhd.transClr));
            IFFWriter.Write((byte)bmhd.xAspect);   // num of bitplanes
            IFFWriter.Write((byte)bmhd.yAspect);   // num of bitplanes

            IFFWriter.Write(endian.Convert((ushort)bmhd.pageWidth));               //x
            IFFWriter.Write(endian.Convert((ushort)bmhd.pageHeight));

            WriteFourCC("CMAP");

            int NumOfColors = palette.Length * 3;
            IFFWriter.Write(endian.Convert((uint)NumOfColors));

            for (int i = 0; i < palette.Length; i++)
            {
                IFFWriter.Write((byte)palette[i].R);
                IFFWriter.Write((byte)palette[i].G);
                IFFWriter.Write((byte)palette[i].B);

            }

            if ((NumOfColors & 0x01) == 0x01)
                IFFWriter.Seek(1, SeekOrigin.Current);

            WriteFourCC("ABIT");
            IFFWriter.Write(endian.Convert((uint)Bitplanes.Length));


            for (int bp = 0; bp < bmhd.numPlanes; bp++)
                for (int y = 0; y < bmhd.height; y++)
                    for (int x = 0; x < WidthInBytes; x++)
                    {
                        byte outPut = Bitplanes[bp,y * WidthInBytes + x];
                        IFFWriter.Write((byte)outPut);
                    }

            uint FormSize = (uint)IFFWriter.BaseStream.Position - 8;
            IFFWriter.Seek((int)FORMSizePos, SeekOrigin.Begin);
            IFFWriter.Write(endian.Convert((uint)FormSize));

            if ((IFFWriter.BaseStream.Position & 0x01) == 0x01)
                IFFWriter.Seek(1, SeekOrigin.Current);

            IFFWriter.Flush();
            IFFWriter.Close();
        }

        public virtual void SaveILBM(string filename)
        {
            Stream IFFFile = File.OpenWrite(filename);
            IFFWriter = new BinaryWriter(IFFFile);

            WriteFourCC("FORM");
            uint FORMSizePos = (uint)IFFWriter.BaseStream.Position;        // Remember the position to update it later
            uint FORMSize = 0;// = (uint)(0x14+palette.Length*3+Bitplanes.Length+28);

            IFFWriter.Write(endian.Convert((uint)FORMSize));

            WriteFourCC("ILBMBMHD");
            uint BMHDSizePos = (uint)IFFWriter.BaseStream.Position;
            IFFWriter.Write(endian.Convert((uint)0x14));
            IFFWriter.Write(endian.Convert((ushort)bmp.Width));   //
            IFFWriter.Write(endian.Convert((ushort)bmp.Height));

            IFFWriter.Write((ushort)0);               //x
            IFFWriter.Write((ushort)0);               //y

            IFFWriter.Write((byte)bmhd.numPlanes);   // num of bitplanes
            IFFWriter.Write((byte)bmhd.mask);   // num of bitplanes
            IFFWriter.Write((byte)bmhd.compression);   // num of bitplanes
            IFFWriter.Write((byte)0);   // num of bitplanes

            IFFWriter.Write(endian.Convert((ushort)bmhd.transClr));
            IFFWriter.Write((byte)bmhd.xAspect);   // num of bitplanes
            IFFWriter.Write((byte)bmhd.yAspect);   // num of bitplanes

            IFFWriter.Write(endian.Convert((ushort)bmhd.pageWidth));               //x
            IFFWriter.Write(endian.Convert((ushort)bmhd.pageHeight));

            WriteFourCC("CMAP");

            int NumOfColors = palette.Length * 3;
            IFFWriter.Write(endian.Convert((uint)NumOfColors));

            for (int i = 0; i < palette.Length; i++)
            {
                IFFWriter.Write((byte)palette[i].R);
                IFFWriter.Write((byte)palette[i].G);
                IFFWriter.Write((byte)palette[i].B);

            }

            if ((NumOfColors & 0x01) == 0x01)
                IFFWriter.Seek(1, SeekOrigin.Current);

            WriteFourCC("BODY");
            IFFWriter.Write(endian.Convert((uint)Bitplanes.Length));

            for (int y = 0; y < bmhd.height; y++)
                for (int bp = 0; bp < bmhd.numPlanes; bp++)
                    for (int x = 0; x < WidthInBytes; x++)
                    {
                        byte outPut = Bitplanes[bp, y * WidthInBytes + x];
                        IFFWriter.Write((byte)outPut);
                    }

            uint FormSize = (uint)IFFWriter.BaseStream.Position - 8;
            IFFWriter.Seek((int)FORMSizePos, SeekOrigin.Begin);
            IFFWriter.Write(endian.Convert((uint)FormSize));

            if ((IFFWriter.BaseStream.Position & 0x01) == 0x01)
                IFFWriter.Seek(1, SeekOrigin.Current);

            IFFWriter.Flush();
            IFFWriter.Close();
        }

        private void WriteFourCC(string chunkID)
        {
            for (int i = 0; i < chunkID.Length; i++)
            {
                IFFWriter.Write(chunkID[i]);
            }
        }

        public virtual void PopulateBMHD(BMHD bmhdSrc)
        {
            bmhd.width = bmhdSrc.width;
            bmhd.height = bmhdSrc.height;
            bmhd.xOrigin = bmhdSrc.xOrigin;
            bmhd.yOrigin = bmhdSrc.yOrigin;
            bmhd.numPlanes = bmhd.numPlanes;
            bmhd.mask = bmhdSrc.mask;
            bmhd.compression = bmhdSrc.compression;
            bmhd.pad1 = bmhdSrc.pad1;
            bmhd.transClr = bmhdSrc.transClr;
            bmhd.xAspect = bmhdSrc.xAspect;
            bmhd.yAspect = bmhdSrc.yAspect;
            bmhd.pageWidth = bmhdSrc.pageWidth;
            bmhd.pageHeight = bmhdSrc.pageHeight;

        }
    }
}

    