using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
            List<UOFileIndex3D> entries = new List<UOFileIndex3D>();
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
                        DecompressedLength = decompressedLength,
                        File = this,
                        Path = this.Path
                    };

                    hashes.Add(hash, data);
                }
            } while (Seek(nextblock) != 0);

            for (int animID = 0; animID < 2048; animID++)
            {
                for (int grpID = 0; grpID < 100; grpID++)
                {
                    string hashstring = string.Format("build/animationlegacyframe/{0:D6}/{1:D2}.bin", animID, grpID);

                    ulong hash = UOFileUop.CreateHash(hashstring);

                    if (hashes.TryGetValue(hash, out var data))
                    {
                        //entries.Add(new UOFileIndex3D(data.Offset, data.DecompressedLength))
                    }
                }
            }

            Entries3D = entries.ToArray();
        }
    }

    public struct UOFileIndexUopAnimation
    {
        public int Offset { get; set; }
        public int CompressedLength { get; set; }
        public int DecompressedLength { get; set; }
        public UOFile File { get; set; }
        public string Path { get; set; }
    }
}
