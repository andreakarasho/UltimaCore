using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UltimaCore.Graphics
{
    public sealed class TileMatrixPatch
    {

        private int _blockWidth, _blockHeight;

        private static StaticTile[] _tileBuffer = new StaticTile[128];

        public TileMatrixPatch(TileMatrix matrix, int index)
        {
            _blockWidth = matrix.BlockWidth;
            _blockHeight = matrix.BlockHeight;

            LandBlocksCount = StaticBlocksCount = 0;

            string pathmapdifl = Path.Combine(FileManager.UoFolderPath, string.Format("mapdif{0}.mul", index));
            string pathmapdiflidx = Path.Combine(FileManager.UoFolderPath, string.Format("mapdifl{0}.mul", index));

            if (File.Exists(pathmapdifl) && File.Exists(pathmapdiflidx))
            {
                LandBlocks = new Tile[matrix.BlockWidth][][];
                LandBlocksCount = PatchLand(matrix, pathmapdifl, pathmapdiflidx);
            }

            string pathstadata = Path.Combine(FileManager.UoFolderPath, string.Format("stadif{0}.mul", index));
            string pathindexdata = Path.Combine(FileManager.UoFolderPath, string.Format("stadifl{0}.mul", index));
            string pathlookup = Path.Combine(FileManager.UoFolderPath, string.Format("stadifi{0}.mul", index));

            if (File.Exists(pathstadata) && File.Exists(pathindexdata) && File.Exists(pathlookup))
            {
                StaticBlocks = new HuedTile[matrix.BlockWidth][][][][];
                StaticBlocksCount = PatchStatics(matrix, pathstadata, pathindexdata, pathlookup);
            }
        }

        private int PatchLand(TileMatrix matrix, string path, string pathidx)
        {
            UOFileMul file = new UOFileMul(path, pathidx, 0);

            int count = (int)file.IdxFile.Length / 4;

            for (int i = 0; i < count; i++)
            {
                int blockID = file.IdxFile.ReadInt();
                int x = blockID / matrix.BlockHeight;
                int y = blockID % matrix.BlockHeight;

                file.Skip(4);

                Tile[] tiles = new Tile[64];

                for (int j = 0; j < tiles.Length; j++)
                    tiles[j].Set(file.ReadUShort(), file.ReadSByte());

                if (LandBlocks[x] == null)
                    LandBlocks[x] = new Tile[matrix.BlockHeight][];
                LandBlocks[x][y] = tiles;
            }

            return count;
        }

        private int PatchStatics(TileMatrix matrix, string path, string pathidx, string pathlookup)
        {
            UOFileMul file = new UOFileMul(path, pathidx, 0);
            UOFileMul lookup = new UOFileMul(pathlookup);

            int count = Math.Min((int)file.IdxFile.Length / 4, (int)lookup.Length / 12);

            file.Seek(0);
            file.IdxFile.Seek(0);
            lookup.Seek(0);

            HuedTileList[][] lists = new HuedTileList[8][];

            for (int x = 0; x < 8; x++)
            {
                lists[x] = new HuedTileList[8];
                for (int y = 0; y < 8; y++)
                    lists[x][y] = new HuedTileList();
            }

            for (int i = 0; i < count; i++)
            {
                int blockID = file.IdxFile.ReadInt();
                int blockX = blockID / matrix.BlockHeight;
                int blockY = blockID % matrix.BlockHeight;

                int offset = lookup.ReadInt();
                int length = lookup.ReadInt();

                lookup.Skip(4);

                if (offset < 0 || length < 0)
                {
                    if (StaticBlocks[blockX] == null)
                        StaticBlocks[blockX] = new HuedTile[matrix.BlockHeight][][][];
                    StaticBlocks[blockX][blockY] = TileMatrix.EmptyStaticBlock;
                    continue;
                }

                file.Seek(offset);

                int tileCount = length / 7;
                if (_tileBuffer.Length < tileCount)
                    _tileBuffer = new StaticTile[tileCount];

                StaticTile[] statiles = _tileBuffer;

                for (int j = 0; j < tileCount; j++)
                {
                    statiles[j].ID = file.ReadUShort();
                    statiles[j].X = file.ReadByte();
                    statiles[j].Y = file.ReadByte();
                    statiles[j].Z = file.ReadSByte();
                    statiles[j].Hue = file.ReadUShort();

                    lists[statiles[j].X & 0x7][statiles[j].Y & 0x7].Add(statiles[j].ID, statiles[j].Hue, statiles[j].Z);
                }

                HuedTile[][][] tiles = new HuedTile[8][][];

                for (int x = 0; x < 8; x++)
                {
                    tiles[x] = new HuedTile[8][];
                    for (int y = 0; y < 8; y++)
                        tiles[x][y] = lists[x][y].ToArray();
                }

                if (StaticBlocks[blockX] == null)
                    StaticBlocks[blockX] = new HuedTile[matrix.BlockHeight][][][];
                StaticBlocks[blockX][blockY] = tiles;
            }

            return count;
        }

        public int LandBlocksCount { get; private set; }
        public int StaticBlocksCount { get; private set; }
        public Tile[][][] LandBlocks { get; private set; }
        public HuedTile[][][][][] StaticBlocks { get; private set; }


        public bool IsLandBlockPatched(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _blockWidth || y >= _blockHeight)
                return false;

            if (LandBlocks[x] == null || LandBlocks[x][y] == null)
                return false;

            return true;
        }

        public Tile[] GetLandBlock(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _blockWidth || y >= _blockHeight)
                return TileMatrix.InvalidLandBlock;
            if (LandBlocks[x] == null)
                return TileMatrix.InvalidLandBlock;
            return LandBlocks[x][y];
        }

        public Tile GetLandTile(int x, int y) => GetLandBlock(x >> 3, y >> 3)[((y & 0x7) << 3) + (x & 0x7)];


        public bool IsStaticBlockPatcher(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _blockWidth || y >= _blockHeight)
                return false;

            if (StaticBlocks[x] == null || StaticBlocks[x][y] == null)
                return false;

            return true;
        }

        public HuedTile[][][] GetStaticBlock(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _blockWidth || y >= _blockHeight)
                return TileMatrix.EmptyStaticBlock;
            if (StaticBlocks[x] == null)
                return TileMatrix.EmptyStaticBlock;
            return StaticBlocks[x][y];
        }

        public HuedTile[] GetStaticTile(int x, int y) => GetStaticBlock(x >> 3, y >> 3)[x & 0x7][y & 0x7];
    }
}
