using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UltimaCore.Graphics
{
    public static class Verdata
    {
        public static UOFileIndex5D[] Patches { get; private set; }
        public static UOFileMul File { get; private set; }

        static Verdata()
        {
            string path = Path.Combine(FileManager.UoFolderPath, "verdata.mul");

            if (!System.IO.File.Exists(path))
            {
                Patches = new UOFileIndex5D[0];
                File = null;
            }
            else
            {
                File = new UOFileMul("verdata.mul");
                Patches = new UOFileIndex5D[File.ReadInt()];

                for (int i = 0; i < Patches.Length; i++)
                {
                    Patches[i].File = File.ReadInt();
                    Patches[i].Index = File.ReadInt();
                    Patches[i].Offset = File.ReadInt();
                    Patches[i].Length = File.ReadInt();
                    Patches[i].Extra = File.ReadInt();
                }
            }
        }
    }
}
