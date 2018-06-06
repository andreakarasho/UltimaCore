using System;
using System.Collections.Generic;
using System.Text;

namespace UltimaCore
{
    public struct UOFileIndex
    {
        public UOFileIndex(long offset, int length, int extra = 0)
        {
            Offset = offset; Length = length; Extra = extra;
        }

        public long Offset;
        public int Length;
        public int Extra;
    }
}
