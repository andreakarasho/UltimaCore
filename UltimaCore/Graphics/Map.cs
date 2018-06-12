using System;
using System.Collections.Generic;
using System.Text;

namespace UltimaCore.Graphics
{
    public sealed class Map : IDisposable
    {
        public static Map Felucca { get; } = new Map(0, 7168, 4096);



        private TileMatrix _tiles;

        public Map(int map, int width, int height)
        {
            Index = map; Width = width; Height = height;
        }

        public int Index { get; }
        public int Width { get; }
        public int Height { get; }
        public TileMatrix Tiles => _tiles;


        public void Load()
        {
            if (_tiles == null)
                _tiles = new TileMatrix(Index, Width, Height);
        }

        public void Dispose()
        {
            
        }
    }
}
