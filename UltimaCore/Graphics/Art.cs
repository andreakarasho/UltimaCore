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
                    _file = new UOFileMul(filepath, idxpath, 0x10000);
            }
        }

        public unsafe static ushort[] ReadStaticArt(ushort graphic, out short width, out short height)
        {
            graphic += 0x4000;
            graphic &= FileManager.GraphicMask;

            (int length, int extra, bool patcher) = _file.SeekByEntryIndex(graphic);

            _file.Skip(4);

            width = _file.ReadShort();
            height = _file.ReadShort();

            if (width <= 0 || height <= 0)
                return null;

            ushort[] pixels = new ushort[width * height];

            ushort* ptr = (ushort*)(_file.StartAddress + _file.Position);
            ushort* lineoffsets = ptr;
            byte* datastart = (byte*)(ptr) + (height * 2);

            int x = 0;
            int y = 0;
            ushort xoffs = 0;
            ushort run = 0;

            ptr = (ushort*)(datastart + (lineoffsets[0] * 2));

            while (y < height)
            {
                xoffs = *ptr;
                ptr++;
                run = *ptr;
                ptr++;

                if (xoffs + run >= 2048)
                {
                    pixels = new ushort[width * height];
                    return pixels;
                }
                else if (xoffs + run != 0)
                {
                    x += xoffs;
                    int pos = y * width + x;
                    for (int j = 0; j < run; j++)
                    {
                        ushort val = *ptr++;
                        if (val > 0)
                        {
                            val = (ushort)(0x8000 | val);
                        }
                        pixels[pos++] = val;
                    }
                    x += run;
                }
                else
                {
                    x = 0;
                    y++;
                    ptr = (ushort*)(datastart + (lineoffsets[y] * 2));
                }
            }
            /*ushort[] lookups = new ushort[height];
            for (int i = 0; i < height; i++)
                lookups[i] = _file.ReadUShort();

            ushort[] data = new ushort[length - lookups.Length * 2 - 8];
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
            }*/

            return pixels;
        }

        public unsafe static ushort[] ReadLandArt(ushort graphic)
        {
            graphic &= FileManager.GraphicMask;

            (int length, int extra, bool patcher) = _file.SeekByEntryIndex(graphic);

            //int i = 0;

            ushort[] pixels = new ushort[44 * 44];

            for(int i = 0; i < 22; i++)
            {
                int start = (22 - (i + 1));
                int pos = i * 44 + start;
                int end = start + ((int)i + 1) * 2;

                for (int j = start; j < end; j++)
                {
                    ushort val = _file.ReadUShort();
                    if (val > 0)
                        val = (ushort)(0x8000 | val);

                    pixels[pos++] = val;
                }
            }
            for (int i = 0; i < 22; i++)
            {
                int pos = (i + 22) * 44 + i;
                int end = i + (22 - i) * 2;

                for (int j = i; j < end; j++)
                {
                    ushort val = _file.ReadUShort();
                    if (val > 0)
                        val = (ushort)(0x8000 | val);

                    pixels[pos++] = val;
                }
            }
            
                 /* ushort[] data = new ushort[length / 2];
                  for (; i < data.Length; i++)
                      data[i] = _file.ReadUShort();
                  i = 0;
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
                              *start++ = (ushort)(data[i++] | 0x8000);
                      }

                      count = 44;
                      offset = 0;

                      for (int y = 0; y < 22; y++, count -= 2, offset++, dataRef += 44)
                      {
                          ushort* start = dataRef + offset;
                          ushort* end = start + count;

                          while (start < end)
                              *start++ = (ushort)(data[i++] | 0x8000);
                      }
                  }
                  */
                return pixels;
        }
    }
}
