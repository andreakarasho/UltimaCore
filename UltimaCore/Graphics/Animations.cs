using System;
using System.Collections.Generic;
using System.Text;

namespace UltimaCore.Graphics
{
    public static class Animations
    {

    }

    public class AnimationFrame
    {
        public static readonly AnimationFrame Null = new AnimationFrame();
        public static readonly AnimationFrame[] Empty = { Null };

        const int DOUBLE_XOR = (0x200 << 22) | (0x200 << 12);
        const int END_OF_FRAME = 0x7FFF7FFF;

        private AnimationFrame()
        {
            CenterX = 0;
            CenterY = 0;
        }

        public unsafe AnimationFrame(ushort[] palette, UOFile file)
        {
            int x = file.ReadShort();
            int y = file.ReadShort();
            int width = file.ReadUShort();
            int heigth = file.ReadUShort();

            if (width == 0 || heigth == 0)
                return;

            // sittings ?

            ushort[] data = new ushort[width * heigth];

            fixed (ushort* pdata = data)
            {
                ushort* dataRef = pdata;

                int header;

                while ((header = file.ReadInt()) != END_OF_FRAME)
                {
                    header ^= DOUBLE_XOR;

                    int xx = ((header >> 22) & 0x3FF) + x - 0x200;
                    int yy = ((header >> 12) & 0x3FF) + y + header - 0x200;

                    ushort* cur = dataRef + yy * width + xx;
                    ushort* end = cur + (header & 0xFFF);
                    int filecount = 0;
                    byte[] filedata = file.ReadArray(header & 0xFFF);
                    while (cur < end)
                        *cur++ = palette[filedata[filecount++]];
                }

            }

            CenterX = x;
            CenterY = y;
            Data = data;
        }

        public int CenterX { get; }
        public int CenterY { get; }
        public ushort[] Data { get; }
    }
}
