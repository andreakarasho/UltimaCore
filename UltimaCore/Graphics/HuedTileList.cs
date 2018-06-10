using System;
using System.Collections.Generic;
using System.Text;

namespace UltimaCore.Graphics
{
    public sealed class HuedTileList
    {
        private List<HuedTile> _tiles;

        public HuedTileList()
        {
            _tiles = new List<HuedTile>();
        }

        public int Count => _tiles.Count;

        public void Add(ushort id, ushort hue, sbyte z) => _tiles.Add(new HuedTile(id, hue, z));

        public HuedTile[] ToArray()
        {
            HuedTile[] tiles = new HuedTile[Count];
            if (_tiles.Count > 0)
                _tiles.CopyTo(tiles);
            _tiles.Clear();
            return tiles;
        }
    }

    public sealed class TileList
    {
        private List<Tile> _tiles;

        public TileList()
        {
            _tiles = new List<Tile>();
        }

        public int Count => _tiles.Count;
        public Tile this[int i] => _tiles[i];

        public void Add(ushort id, sbyte z) => _tiles.Add(new Tile(id, z));

        public Tile[] ToArray()
        {
            Tile[] tiles = new Tile[Count];
            if (_tiles.Count > 0)
                _tiles.CopyTo(tiles);
            _tiles.Clear();
            return tiles;
        }
    }
}
