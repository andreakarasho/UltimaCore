using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UltimaCore.Graphics
{
    public static class Light // is it useful?
    {
        private static UOFileMul _file;

        public static void Load()
        {
            string path = Path.Combine(FileManager.UoFolderPath, "light.mul");
            string pathidx = Path.Combine(FileManager.UoFolderPath, "lightidx.mul");

            if (!File.Exists(path) || !File.Exists(pathidx))
                throw new FileNotFoundException();

            _file = new UOFileMul(path, pathidx, 100);
        }
    }
}
