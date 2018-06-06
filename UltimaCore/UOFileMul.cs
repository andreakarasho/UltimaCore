using System;
using System.Collections.Generic;
using System.Text;
using UltimaCore.Graphics;

namespace UltimaCore
{
    public class UOFileMul : UOFile
    {
        private readonly UOFileIdxMul _idxFile;

        public UOFileMul(string file, string idxfile) : base(file)
        {
            _idxFile = new UOFileIdxMul(idxfile);
            Load();
        }

        public UOFileMul(string file) : base(file)
        {
            Load();
        }

        protected override void Load()
        {
            base.Load();

            if (_idxFile != null)
            {

                int count = (int)_idxFile.Length / 12;

                Entries3D = new UOFileIndex3D[count];

                for (int i = 0; i < count; i++)
                    Entries3D[i] = new UOFileIndex3D(_idxFile.ReadInt(), _idxFile.ReadInt(), _idxFile.ReadInt());
            }
        }

        private class UOFileIdxMul : UOFile
        {
            public UOFileIdxMul(string idxpath) : base(idxpath) { }
        }
    }
}
