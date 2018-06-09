using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace UltimaCore.Graphics
{
    public static class Hues
    {
        private static UOFileMul _file;

        private static int _huesCount;
        private static HuesGroup[] _groups;
        private static float[][] _palette;

        public static void Load()
        {
            string path = Path.Combine(FileManager.UoFolderPath, "hues.mul");
            if (!File.Exists(path))
                throw new FileNotFoundException();

            _file = new UOFileMul(path);

            int entrycount = (int)_file.Length / 708;
            if (entrycount > 375)
                entrycount = 375;

            _huesCount = 0;
            _groups = new HuesGroup[entrycount];

            for (int entry = 0; entry < entrycount; entry++)
            {
                _groups[entry] = new HuesGroup();
                _groups[entry].Header = _file.ReadUInt();
                _groups[entry].Entries = new HuesBlock[8];
                for (int j = 0; j < 8; j++)
                {
                    _huesCount++;

                    _groups[entry].Entries[j] = new HuesBlock();
                    _groups[entry].Entries[j].ColorTable = new ushort[32];

                    for (int i = 0; i < 32; i++)
                    {
                        _groups[entry].Entries[j].ColorTable[i] = _file.ReadUShort();
                    }

                    _groups[entry].Entries[j].Start = _file.ReadUShort();
                    _groups[entry].Entries[j].End = _file.ReadUShort();
                    _groups[entry].Entries[j].Name = Encoding.UTF8.GetString(_file.ReadArray(20));

                }
            }

            _palette = new float[_huesCount][];

            for (int i = 0; i < entrycount; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    float[] a = _palette[(i * 8) + j] = new float[32 * 3];

                    for (int h = 0; h < 32; h++)
                    {
                        int idx = h * 3;
                        ushort c = _groups[i].Entries[j].ColorTable[h];
                        a[idx] = (((c >> 10) & 0x1F) / 31.0f);
                        a[idx + 1] = (((c >> 5) & 0x1F) / 31.0f);
                        a[idx + 2] = ((c & 0x1F) / 31.0f);
                    }
                }
            }
        }

        private static readonly byte[] _table = new byte[32]
        {
            0x00, 0x08, 0x10, 0x18, 0x20, 0x29, 0x31, 0x39,
            0x41, 0x4A, 0x52, 0x5A, 0x62, 0x6A, 0x73, 0x7B,
            0x83, 0x8B, 0x94, 0x9C, 0xA4, 0xAC, 0xB4, 0xBD,
            0xC5, 0xCD, 0xD5, 0xDE, 0xE6, 0xEE, 0xF6, 0xFF
        };

        public static uint Color16To32(ushort c)
            => (uint)(_table[(c >> 10) & 0x1F] |
                     (_table[(c >> 5) & 0x1F] << 8) |
                     (_table[c & 0x1F] << 16));

        public static ushort Color32To16(int c)
            => (ushort)((((c & 0xFF) * 32) / 256) |
                       (((((c >> 16) & 0xff) * 32) / 256) << 10) |
                       (((((c >> 8) & 0xff) * 32) / 256) << 5));

        public static ushort ConvertToGray(ushort c) 
            => (ushort)(((c & 0x1F) * 299 + ((c >> 5) & 0x1F) * 587 + ((c >> 10) & 0x1F) * 114) / 1000);

        public static ushort GetColor16(ushort c, ushort color)
        {
            if (color != 0 && color < _huesCount)
            {
                color--;
                int g = color / 8;
                int e = color % 8;

                return _groups[g].Entries[e].ColorTable[(c >> 10) & 0x1F];
            }
            return c;
        }

        public static uint GetUnicodeFontColor(ushort c, ushort color)
        {
            if (color != 0 && color < _huesCount)
            {
                color--;
                int g = color / 8;
                int e = color % 8;

                return _groups[g].Entries[e].ColorTable[8];
            }
            return Color16To32(c);
        }

        public static uint GetColor(ushort c, ushort color)
        {
            if (color != 0 && color < _huesCount)
            {
                color--;
                int g = color / 8;
                int e = color % 8;

                return Color16To32(_groups[g].Entries[e].ColorTable[(c >> 10) & 0x1F]);
            }
            return Color16To32(c);
        }
    }



    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct HuesBlock
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public ushort[] ColorTable;
        public ushort Start;
        public ushort End;

        [MarshalAs(UnmanagedType.LPStr, SizeConst = 20)]
        public string Name;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HuesGroup
    {
        public uint Header;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public HuesBlock[] Entries;
    }

    
}
