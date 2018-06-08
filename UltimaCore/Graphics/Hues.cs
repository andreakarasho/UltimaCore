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

        public static void Load()
        {
            string path = Path.Combine(FileManager.UoFolderPath, "hues.mul");
            if (!File.Exists(path))
                throw new FileNotFoundException();

            _file = new UOFileMul(path);

            int entrycount = (int)_file.Length / 708;
            if (entrycount > 375)
                entrycount = 375;

            _groups = new HuesGroup[entrycount];
       
            for (int entry = 0; entry < entrycount; entry++)
            {
                _groups[entry] = new HuesGroup();
                _groups[entry].Header = _file.ReadUInt();
                _groups[entry].Entries = new HuesBlock[8];
                for (int j = 0; j < 8; j++)
                {
                    _groups[entry].Entries[j] = new HuesBlock();
                    _groups[entry].Entries[j].ColorTable = new ushort[32];

                    for (int i = 0; i < 32; i++)
                    {
                        _groups[entry].Entries[j].ColorTable[i] = _file.ReadUShort();
                        _huesCount++;
                    }

                    _groups[entry].Entries[j].Start = _file.ReadUShort();
                    _groups[entry].Entries[j].End = _file.ReadUShort();
                    _groups[entry].Entries[j].Name = Encoding.UTF8.GetString(_file.ReadArray(20));

                }
            }
        }

        public static HuesBlock GetHue(int i)
        {
            /*i &= 0x3FFF;
            if (i >= 0 && i < _huesCount)
                return _groups[]*/
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
