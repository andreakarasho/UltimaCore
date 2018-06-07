using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace UltimaCore.Graphics
{
    public static class Animations
    {
        private static List<UOFile> _files = new List<UOFile>();
        
        public static void Load()
        {
            string filepath;

            for (int i = 0; i < 5; i++)
            {
                filepath = Path.Combine(FileManager.UoFolderPath, string.Format("AnimationFrame{0}.uop", i));
                if (File.Exists(filepath))
                    _files.Add(new UOFileUopAnimation(filepath));
                else
                {
                    string name = string.Format("Anim{0}.mul", (i == 0 ? "" : i.ToString()));
                    string idx = string.Format("Anim{0}.idx", (i == 0 ? "" : i.ToString()));

                    filepath = Path.Combine(FileManager.UoFolderPath, name);
                    string fileidxpath = Path.Combine(FileManager.UoFolderPath, idx);

                    if (File.Exists(filepath) && File.Exists(fileidxpath))
                    {
                        try
                        {
                            _files.Add(new UOFileMul(filepath, fileidxpath, i == 0 ? 6 : -1));
                        }
                        catch { }
                    }

                }
            }
        }
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

    public class UOFileUopAnimation : UOFile
    {
        private const uint UOP_MAGIC_NUMBER = 0x50594D;

        public UOFileUopAnimation(string path) : base(path)
        {

            Load();
        }

        public new UOFileIndexUopAnimation[] Entries { get; private set; }

        protected override void Load()
        {
            base.Load();

            Seek(0);
            if (ReadInt() != UOP_MAGIC_NUMBER)
                throw new ArgumentException("Bad uop file");

            Skip(8);
            long nextblock = ReadLong();
            Skip(4);

            Seek(nextblock);

            Dictionary<ulong, UOFileIndexUopAnimation> hashes = new Dictionary<ulong, UOFileIndexUopAnimation>();
            List<UOFileIndexUopAnimation> entries = new List<UOFileIndexUopAnimation>();
            do
            {
                int fileCount = ReadInt();
                nextblock = ReadLong();

                for (int i = 0; i < fileCount; i++)
                {
                    long offset = ReadLong();
                    int headerLength = ReadInt();
                    int compressedLength = ReadInt();
                    int decompressedLength = ReadInt();
                    ulong hash = ReadULong();
                    Skip(6);

                    if (offset == 0)
                        continue;

                    UOFileIndexUopAnimation data = new UOFileIndexUopAnimation
                    {
                        Offset = (int)(offset + headerLength),
                        CompressedLength = compressedLength,
                        DecompressedLength = decompressedLength
                    };

                    hashes.Add(hash, data);
                }
                Seek(nextblock);
            } while (nextblock != 0);

            for (int animID = 0; animID < 2048; animID++)
            {
                for (int grpID = 0; grpID < 100; grpID++)
                {
                    string hashstring = string.Format("build/animationlegacyframe/{0:D6}/{1:D2}.bin", animID, grpID);
                    ulong hash = UOFileUop.CreateHash(hashstring);

                    if (hashes.TryGetValue(hash, out var data))
                    {
                        data.AnimID = animID;
                        entries.Add(data);
                    }
                }
            }

            this.Entries = entries.ToArray();
        }
    }

    public struct UOFileIndexUopAnimation
    {
        public int Offset { get; set; }
        public int CompressedLength { get; set; }
        public int DecompressedLength { get; set; }
        public int AnimID { get; set; }
    }

    public static class BodyConverter
    {
        private static readonly int[][] Table = new int[4][];

        private static readonly int[][] _MountIDConv = 
        {
            new int[]{0x3E94, 0xF3}, // Hiryu
            new int[]{0x3E97, 0xC3}, // Beetle
            new int[]{0x3E98, 0xC2}, // Swamp Dragon
            new int[]{0x3E9A, 0xC1}, // Ridgeback
            new int[]{0x3E9B, 0xC0}, // Unicorn
            new int[]{0x3E9D, 0xC0}, // Unicorn
            new int[]{0x3E9C, 0xBF}, // Ki-Rin
            new int[]{0x3E9E, 0xBE}, // Fire Steed
            new int[]{0x3E9F, 0xC8}, // Horse
            new int[]{0x3EA0, 0xE2}, // Grey Horse
            new int[]{0x3EA1, 0xE4}, // Horse
            new int[]{0x3EA2, 0xCC}, // Brown Horse
            new int[]{0x3EA3, 0xD2}, // Zostrich
            new int[]{0x3EA4, 0xDA}, // Zostrich
            new int[]{0x3EA5, 0xDB}, // Zostrich
            new int[]{0x3EA6, 0xDC}, // Llama
            new int[]{0x3EA7, 0x74}, // Nightmare
            new int[]{0x3EA8, 0x75}, // Silver Steed
            new int[]{0x3EA9, 0x72}, // Nightmare
            new int[]{0x3EAA, 0x73}, // Ethereal Horse
            new int[]{0x3EAB, 0xAA}, // Ethereal Llama
            new int[]{0x3EAC, 0xAB}, // Ethereal Zostrich
            new int[]{0x3EAD, 0x84}, // Ki-Rin
            new int[]{0x3EAF, 0x78}, // Minax Warhorse
            new int[]{0x3EB0, 0x79}, // ShadowLords Warhorse
            new int[]{0x3EB1, 0x77}, // COM Warhorse
            new int[]{0x3EB2, 0x76}, // TrueBritannian Warhorse
            new int[]{0x3EB3, 0x90}, // Seahorse
            new int[]{0x3EB4, 0x7A}, // Unicorn
            new int[]{0x3EB5, 0xB1}, // Nightmare
            new int[]{0x3EB6, 0xB2}, // Nightmare
            new int[]{0x3EB7, 0xB3}, // Dark Nightmare
            new int[]{0x3EB8, 0xBC}, // Ridgeback
            new int[]{0x3EBA, 0xBB}, // Ridgeback
            new int[]{0x3EBB, 0x319}, // Undead Horse
            new int[]{0x3EBC, 0x317}, // Beetle
            new int[]{0x3EBD, 0x31A}, // Swamp Dragon
            new int[]{0x3EBE, 0x31F}, // Armored Swamp Dragon
            new int[]{0x3F6F, 0x9},  // Daemon
            new int[]{0x3EC3, 0x02D4}, // beetle
            new int[]{0x3EC5, 0xD5},
            new int[]{0x3F3A, 0xD5},
            new int[]{0x3E90, 0x114}, // reptalon
            new int[]{0x3E91, 0x115},  // cu sidhe
            new int[]{0x3E92, 0x11C},  // MondainSteed01
            new int[]{0x3EC6, 0x1B0},
            new int[]{0x3EC7, 0x4E6},
            new int[]{0x3EC8, 0x4E7},
        };

        public static void Load()
        {
            string path = Path.Combine(FileManager.UoFolderPath, "bodyconv.def");
            if (!File.Exists(path))
                return;

            List<int> list1 = new List<int>(), list2 = new List<int>(), list3 = new List<int>(), list4 = new List<int>();
            int max1 = 0, max2 = 0, max3 = 0, max4 = 0;

            using (StreamReader reader = new StreamReader(path))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length == 0 || line[0] == '#' || line.StartsWith("\"#"))
                        continue;

                    string[] values = Regex.Split(line, @"\t|\s+", RegexOptions.IgnoreCase);

                    int original = Convert.ToInt32(values[0]);
                    int anim2 = Convert.ToInt32(values[1]);
                    int anim3 = -1, anim4 = -1, anim5 = -1;

                    if (values.Length >= 3)
                    {
                        anim3 = Convert.ToInt32(values[2]);

                        if (values.Length >= 4)
                        {
                            anim4 = Convert.ToInt32(values[3]);

                            if (values.Length >= 5)
                            {
                                anim5 = Convert.ToInt32(values[4]);
                            }
                        }
                    }


                    if (anim2 != -1)
                    {
                        if (anim2 == 68)
                            anim2 = 122;

                        if (original > max1)
                            max1 = original;

                        list1.Add(original); list1.Add(anim2);
                    }

                    if (anim3 != -1)
                    {
                        if (original > max2)
                            max2 = original;
                        list2.Add(original);
                        list2.Add(anim3);
                    }

                    if (anim4 != -1)
                    {
                        if (original > max3)
                            max3 = original;
                        list3.Add(original);
                        list3.Add(anim4);
                    }

                    if (anim5 != -1)
                    {
                        if (original > max4)
                            max4 = original;
                        list4.Add(original);
                        list4.Add(anim5);
                    }

                }
            }

            Table[0] = new int[max1 + 1];

            for (int i = 0; i < Table[0].Length; ++i)
                Table[0][i] = -1;

            for (int i = 0; i < list1.Count; i += 2)
                Table[0][list1[i]] = list1[i + 1];

            Table[1] = new int[max2 + 1];

            for (int i = 0; i < Table[1].Length; ++i)
                Table[1][i] = -1;

            for (int i = 0; i < list2.Count; i += 2)
                Table[1][list2[i]] = list2[i + 1];

            Table[2] = new int[max3 + 1];

            for (int i = 0; i < Table[2].Length; ++i)
                Table[2][i] = -1;

            for (int i = 0; i < list3.Count; i += 2)
                Table[2][list3[i]] = list3[i + 1];

            Table[3] = new int[max4 + 1];

            for (int i = 0; i < Table[3].Length; ++i)
                Table[3][i] = -1;

            for (int i = 0; i < list4.Count; i += 2)
                Table[3][list4[i]] = list4[i + 1];
        }

        public static bool HasBody(int body)
        {
            if (body >= 0)
            {
                for (int i = 0; i < Table.Length; i++)
                {
                    if (body < Table[i].Length && Table[i][body] != -1)
                        return true;
                }
            }
            return false;
        }
    }
}
