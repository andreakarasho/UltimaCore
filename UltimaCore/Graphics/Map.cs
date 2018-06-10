using System;
using System.Collections.Generic;
using System.Text;

namespace UltimaCore.Graphics
{
    public class Map : IDisposable
    {
        private TileMatrix _tiles;

        public Map(int map, int width, int height)
        {
            Index = map; Width = width; Height = height;
        }

        public int Index { get; }
        public int Width { get; }
        public int Height { get; }

        public void Dispose()
        {

        }
    }
}
