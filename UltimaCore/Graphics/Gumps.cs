using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UltimaCore.Graphics
{
    public static class Gumps
    {
        private static UOFile _file;

        public static void Load()
        {
            string path = Path.Combine(FileManager.UoFolderPath, "gumpartLegacyMUL.uop");
            if (File.Exists(path))
            {
                _file = new UOFileUop(path, ".tga", 0xFFFF, true);
            }
            else
            {
                path = Path.Combine(FileManager.UoFolderPath, "Gumpart.mul");
                string pathidx = Path.Combine(FileManager.UoFolderPath, "Gumpidx.mul");

                if (File.Exists(path) && File.Exists(pathidx))
                {
                    _file = new UOFileMul(path, pathidx, 0x10000, 12);
                }
            }
        }

        public unsafe static ushort[] GetGump(int index)
        {
            var entry = _file.Entries[index];

            _file.Seek(entry.Offset);

            int extra = entry.Extra;

            if (extra == -1)
                return null;

            int width = (extra >> 16) & 0xFFFF;
            int height = extra & 0xFFFF;

            if (width <= 0 || height <= 0)
                return null;

            int shortToRead = entry.Length - (height * 2);

            if (_file.Length - _file.Position < (shortToRead * 2))
                return null;

            int[] lookups = new int[height];
            for (int i = 0; i < lookups.Length; i++)
                lookups[i] = _file.ReadInt();
            int metrics = _file.Position;
            ushort[] filedata = new ushort[shortToRead];
            for (int i = 0; i < shortToRead; i++)
                filedata[i] = _file.ReadUShort();
            ushort[] pixels = new ushort[width * height];

            fixed (ushort* line = &pixels[0])
            {
                fixed (ushort* data = &filedata[0])
                {
                    for (int y = 0; y < height; y++)
                    {
                        ushort* dataref = data + (lookups[y] - height) * 2;
                        ushort* cur = line + (y * width);
                        ushort* end = cur + width;

                        while (cur < end)
                        {
                            ushort color = *dataref++;
                            ushort* next = cur + *dataref++;
                            if (color == 0)
                            {
                                cur = next;
                            }
                            else
                            {
                                color |= 0x8000;
                                while (cur < next)
                                    *cur++ = color;
                            }
                        }
                    }
                }
            }

            return pixels;
        }
    }
}
