using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;


namespace GfxLib
{
    public class ObjectFile
    {
        protected Endian Endian = new Endian();
        protected uint TableSize,FirstHunk,LastHunk;

        
        enum HunkType
        {
            HUNK_UNIT = 0x3e7,
            HUNK_NAME = 0x3e8,
            HUNK_CODE = 0x3e9,
            HUNK_DATA = 0x3ea,
            HUNK_BSS = 0x3eb,
            HUNK_RELOC32 = 0x3ec,
            HUNK_RELOC16 = 0x3ed,
            HUNK_RELOC8 = 0x3ee,
            HUNK_EXT = 0x3ef,
            HUNK_SYMBOL = 0x3f0,
            HUNK_DEBUG = 0x3f1,
            HUNK_END = 0x3f2,
            HUNK_HEADER = 0x3f3,
            HUNK_OVERLAY = 0x3f5,
            HUNK_BREAK = 0x3f6,
            HUNK_DREL32 = 0x3f7,
            HUNK_DREL16 = 0x3f8,
            HUNK_DREL8 = 0x3f9,
            HUNK_LIB = 0x3fa,
            HUNK_INDEX = 0x3fb,
            HUNK_RELOC32SHORT = 0x3fc,
            HUNK_RELOC16SHORT = 0x3fd,
            HUNK_RELOC8SHORT = 0x3fe,
            HUNK_PPC_CODE = 0x4e9,      // Extended hunk format
            HUNK_RELRELOC26 = 0x4ec     // Extended hunk format
        };

        HunkType hunk = HunkType.HUNK_UNIT;

        protected Stream fileStream;
        protected BinaryReader binReader;

        List<String> libraries = new List<String>();
        List<uint> HunkSizes = new List<uint>();

        protected uint[] Codes,Data;

        private void ReadHeader()
        {
            string LibraryName;
            do
            {
                LibraryName = ReadString();
            
                if (LibraryName != "")
                {
                    libraries.Add(LibraryName);
                }

            } while (LibraryName != "");

            TableSize = Endian.Convert(binReader.ReadUInt32());
            FirstHunk = Endian.Convert(binReader.ReadUInt32());
            LastHunk = Endian.Convert(binReader.ReadUInt32());

            for (int i = 0; i < TableSize; i++)
            {
                HunkSizes.Add(Endian.Convert ( binReader.ReadUInt32()));
            }

        }

        private string ReadString ()
        {
            uint StringLength = Endian.Convert(binReader.ReadUInt32());
            string str = "";

            if (StringLength > 0)
            {
                for (int i = 0; i < StringLength; i++)
                    str += Endian.Convert(binReader.ReadUInt32()).ToString();
            }

            return str;
        }

        private void WriteHeader() { }

        public void OpenObjectFile ()
        {
            OpenFileDialog ofd = new OpenFileDialog ();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                fileStream = File.OpenRead (ofd.FileName);
                binReader = new BinaryReader(fileStream);

                uint MagicCoocky = ReadMagicCoocky();

                while (MagicCoocky != ((int)HunkType.HUNK_END))
                {
                    switch (MagicCoocky)
                    {
                        case (int)HunkType.HUNK_HEADER:
                            ReadHeader();
                            break;

                        case (int)HunkType.HUNK_CODE:
                            ReadCode();
                            break;

                        case (int)HunkType.HUNK_DATA: 
                            ReadData();
                            break;

                        case (int)HunkType.HUNK_RELOC32:
                            ReadRelocate32();
                            break;

                        case (int)HunkType.HUNK_END:
                            break;
                    }

                    MagicCoocky = ReadMagicCoocky();
                }

                binReader.Close ();


            }
        }

        protected virtual void ReadCode ()
        {
            uint Length = Endian.Convert(binReader.ReadUInt32());

            Codes = new uint[Length];

            for (int i = 0;i < Length;i++)
                Codes[i] = Endian.Convert(binReader.ReadUInt32());


        }

        private void ReadData ()
        {
            uint Length = Endian.Convert(binReader.ReadUInt32());

            Data = new uint[Length];

            for (int i = 0; i < Length; i++)
                Data[i] = Endian.Convert(binReader.ReadUInt32());

        }

        private void ReadRelocate32()
        {
            uint NumOfRelocates;

            do
            {
                // Skip relocation data. where not running on an amiga...
                NumOfRelocates = Endian.Convert(binReader.ReadUInt32());
                if (NumOfRelocates != 0)
                {
                    uint HunkNumber = Endian.Convert(binReader.ReadUInt32());

                    for (int i = 0; i < NumOfRelocates; i++)
                        binReader.ReadUInt32();
                }

            } while (NumOfRelocates != 0);
           
        }

        private uint ReadMagicCoocky () { return (uint)Endian.Convert( binReader.ReadUInt32());}

        public void SaveObjectFile (string path)
        {
            SaveFileDialog ofd = new SaveFileDialog ();
            if (ofd.ShowDialog () == DialogResult.OK)
            {

            }
        }
    }
}
