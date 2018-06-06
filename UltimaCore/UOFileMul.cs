using System;
using System.Collections.Generic;
using System.Text;

namespace UltimaCore
{
    public class UOFileMul : UOFile
    {
        private readonly UOFileIdxMul _idxFile;


        public UOFileMul(string file, string idxfile) : base(file)
        {
            _idxFile = new UOFileIdxMul(idxfile);
        }

        protected override void Load()
        {
            base.Load();

            int count = (int)_idxFile.Length / 12;

            Entries = new UOFileIndex[count];

            for (int i = 0; i < count; i++)
                Entries[i] = new UOFileIndex(_idxFile.ReadInt(), _idxFile.ReadInt(), _idxFile.ReadInt());

            // verdata patch
        }

        private class UOFileIdxMul : UOFile
        {
            public UOFileIdxMul(string idxpath) : base(idxpath) { }
        }
    }
}
