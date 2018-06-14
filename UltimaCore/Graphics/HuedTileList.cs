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

    public sealed class MTileList
    {
        private List<MTile> _tiles;

        public MTileList()
        {
            _tiles = new List<MTile>();
        }

        public int Count => _tiles.Count;
        public MTile this[int idx] { get => _tiles[idx]; set => _tiles[idx] = value; }


        public void Add(ushort id, sbyte z) => _tiles.Add(new MTile(id, z));
        public void Add(ushort id, sbyte z, sbyte flag) => _tiles.Add(new MTile(id, z, flag));
        public void Add(ushort id, sbyte z, sbyte flag, int unk1) => _tiles.Add(new MTile(id, z, flag, unk1));

        public MTile[] ToArray()
        {
            MTile[] tiles = new MTile[Count];

            if (_tiles.Count > 0)
                _tiles.CopyTo(tiles);
            _tiles.Clear();
            return tiles;
        }

        public void Remove(int i) => _tiles.RemoveAt(i);
    }
}
