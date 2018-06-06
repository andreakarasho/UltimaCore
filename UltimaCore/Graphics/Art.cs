using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UltimaCore.Graphics
{
    public static class Art
    {
        private static UOFile _file;

        public static void Load()
        {
            string filepath = Path.Combine(FileManager.UoFolderPath, "artLegacyMUL.uop");

            if (File.Exists(filepath))
                _file = new UOFileUop(filepath, ".tga", 0x10000);
            else
            {
                filepath = Path.Combine(FileManager.UoFolderPath, "art.mul");
                string idxpath = Path.Combine(FileManager.UoFolderPath, "artidx.mul");
                if (File.Exists(filepath) && File.Exists(idxpath))
                    _file = new UOFileMul(filepath, idxpath);
            }
        }

        public unsafe static ushort[] ReadStaticArt(ushort graphic)
        {
            UOFileIndex3D index = _file.Entries[graphic];
            _file.Seek(index.Offset);
            _file.Skip(4);

            int width = _file.ReadShort();
            int height = _file.ReadShort();

            if (width <= 0 || height <= 0)
                return null;

            ushort[] lookups = new ushort[height];
            for (int i = 0; i < height; i++)
                lookups[i] = _file.ReadUShort();

            ushort[] data = new ushort[index.Length - lookups.Length * 2 - 8];
            for (int i = 0; i < data.Length; i++)
                data[i] = _file.ReadUShort();

            ushort[] pixels = new ushort[width * height];

            fixed (ushort* pdata = pixels)
            {
                ushort* dataRef = pdata;
                int i;
                for (int y = 0; y < height; y++, dataRef += width)
                {
                    i = lookups[y];

                    ushort* start = dataRef;

                    int count, offset;

                    while (((offset = data[i++]) + (count = data[i++])) != 0)
                    {
                        start += offset;
                        ushort* end = start + count;

                        while (start < end)
                        {
                            ushort color = data[i++];
                            *start++ = (ushort)(color | 0x8000);
                        }
                    }
                }
            }

            return pixels;
        }

        public unsafe static ushort[] ReadLandArt(ushort graphic)
        {
            UOFileIndex3D index = _file.Entries[graphic];
            _file.Seek(index.Offset);

            int i = 0;

            ushort[] pixels = new ushort[44 * 44];
            ushort[] data = new ushort[23 * 44];
            for (; i < data.Length; i++)
                data[i] = _file.ReadUShort();

            fixed (ushort* pdata = pixels)
            {
                ushort* dataRef = pdata;

                int count = 2;
                int offset = 21;

                for (int y = 0; y < 22; y++, count += 2, offset--, dataRef += 44)
                {
                    ushort* start = dataRef + offset;
                    ushort* end = start + count;

                    while (start < end)
                    {
                        ushort color = data[i++];
                        *start++ = (ushort)(color | 0x8000);
                    }
                }

                count = 44;
                offset = 0;

                for (int y = 0; y < 22; y++, count -= 2, offset++, dataRef += 44)
                {
                    ushort* start = dataRef + offset;
                    ushort* end = start + count;

                    while (start < end)
                    {
                        ushort color = data[i++];
                        *start++ = (ushort)(color | 0x8000);
                    }
                }
            }

            return pixels;
        }
    }
}
