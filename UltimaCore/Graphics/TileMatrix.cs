using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace UltimaCore.Graphics
{
    public sealed class TileMatrix
    {
        private static HuedTileList[][] _list;


        private readonly UOFileIndex3D[] _staticIndex;
        private HuedTile[][][][][] _staticTiles;
        private Tile[][][] _landTiles;
        private bool[][] _removedStaticBlock;
        private List<StaticTile>[][] _staticTilesToAdd;
        private UOFile _file, _filestaidx, _filestatics;



        public TileMatrix(int map, int width, int height)
        {
            Width = width; Height = height;
            BlockWidth = width >> 3;
            BlockHeight = height >> 3;

            string pathmap = Path.Combine(FileManager.UoFolderPath, string.Format("map{0}LegacyMUL.uop", map));
            if (File.Exists(pathmap))
            {
                _file = new UOFileUop(pathmap, ".dat");
            }
            else
            {
                pathmap = Path.Combine(FileManager.UoFolderPath, string.Format("map{0}.mul", map));
                if (!File.Exists(pathmap))
                    throw new FileNotFoundException();
                _file = new UOFileMul(pathmap);
            }

            string pathstaidx = Path.Combine(FileManager.UoFolderPath, string.Format("staidx{0}.mul", map));
            if (!File.Exists(pathstaidx))
                throw new FileNotFoundException();
            _filestaidx = new UOFileMul(pathstaidx);


            string pathstatics = Path.Combine(FileManager.UoFolderPath, string.Format("statics{0}.mul", map));
            if (!File.Exists(pathstatics))
                throw new FileNotFoundException();
            _filestatics = new UOFileMul(pathstatics);


            EmptyStaticBlock = new HuedTile[8][][];

            for (int i = 0; i < 8; i++)
            {
                EmptyStaticBlock[i] = new HuedTile[8][];
                for (int j = 0; j < 8; j++)
                    EmptyStaticBlock[i][j] = new HuedTile[0];
            }

            InvalidLandBlock = new Tile[196];

            _landTiles = new Tile[BlockWidth][][];
            _staticTiles = new HuedTile[BlockWidth][][][][];

            Patch = new TileMatrixPatch(this, map);


            _staticIndex = new UOFileIndex3D[BlockHeight * BlockWidth];

            int count = (int)_filestaidx.Length / 12;
            int minlen = (int)Math.Min(_filestaidx.Length, BlockHeight * BlockWidth);

            for (int i = 0; i < count; i++)
            {
                _staticIndex[i].Offset = _filestaidx.ReadInt();
                _staticIndex[i].Length = _filestaidx.ReadInt();
                _staticIndex[i].Extra = _filestaidx.ReadInt();
            }

            for (int i = minlen; i < BlockHeight * BlockWidth; i++)
            {
                _staticIndex[i].Offset = -1;
                _staticIndex[i].Length = -1;
                _staticIndex[i].Extra = -1;
            }
        }


        public static Tile[] InvalidLandBlock { get; private set; }
        public static HuedTile[][][] EmptyStaticBlock { get; private set; }
        public int BlockWidth { get; private set; }
        public int BlockHeight { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TileMatrixPatch Patch { get; private set; }

        public void SetStaticBlock(int x, int y, HuedTile[][][] value)
        {
            if (x < 0 || y < 0 || x >= BlockWidth || y >= BlockHeight)
                return;

            if (_staticTiles[x] == null)
                _staticTiles[x] = new HuedTile[BlockHeight][][][];
            _staticTiles[x][y] = value;
        }

        public HuedTile[][][] GetStaticBlock(int x, int y) => GetStaticBlock(x, y, true);

        public HuedTile[][][] GetStaticBlock(int x, int y, bool patch)
        {
            if (x < 0 || y < 0 || x >= BlockWidth || y >= BlockHeight)
                return EmptyStaticBlock;
            if (_staticTiles[x] == null)
                _staticTiles[x] = new HuedTile[BlockHeight][][][];

            HuedTile[][][] tiles = _staticTiles[x][y];

            if (tiles == null)
                tiles = _staticTiles[x][y] = ReadStaticBlock(x, y);

            if (patch) // Use DIFF?????!?!??
                if (Patch.StaticBlocksCount > 0)
                    if (Patch.StaticBlocks[x] != null)
                        if (Patch.StaticBlocks[x][y] != null)
                            tiles = Patch.StaticBlocks[x][y];

            return tiles;
        }

        public HuedTile[] GetStaticTiles(int x, int y, bool patch) => GetStaticBlock(x >> 3, y >> 3, patch)[x & 0x7][y & 0x7];

        public HuedTile[] GetStaticTiles(int x, int y) => GetStaticBlock(x >> 3, y >> 3)[x & 0x7][y & 0x7];





        public void SetLandBlock(int x, int y, Tile[] value)
        {
            if (x < 0 || y < 0 || x >= BlockWidth || y >= BlockHeight)
                return;

            if (_landTiles[x] == null)
                _landTiles[x] = new Tile[BlockHeight][];

            _landTiles[x][y] = value;
        }

        /// <summary>
        /// Important: x >> 3, y >> 3
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Tile[] GetLandBlock(int x, int y) => GetLandBlock(x, y, true);

        public Tile[] GetLandBlock(int x, int y, bool patch)
        {
            if (x < 0 || y < 0 || x >= BlockWidth || y >= BlockHeight)
                return InvalidLandBlock;

            if (_landTiles[x] == null)
                _landTiles[x] = new Tile[BlockHeight][];

            Tile[] tiles = _landTiles[x][y];

            if (tiles == null)
                tiles = _landTiles[x][y] = ReadLandBlock(x, y);


            if (patch) // USE MAP DIF?F?!??
                if (Patch.LandBlocksCount > 0)
                    if (Patch.LandBlocks[x] != null)
                        if (Patch.LandBlocks[x][y] != null)
                            tiles = Patch.LandBlocks[x][y];

            return tiles;
        }

        public Tile GetLandTile(int x, int y, bool patch) => GetLandBlock(x >> 3, y >> 3, patch)[((y & 0x7) << 3) + (x & 0x7)];

        public Tile GetLandTile(int x, int y) => GetLandBlock(x >> 3, y >> 3)[((y & 0x7) << 3) + (x & 0x7)];

        public void RemoveStaticBlock(int x, int y)
        {
            if (_removedStaticBlock == null)
                _removedStaticBlock = new bool[BlockWidth][];
            if (_removedStaticBlock[x] == null)
                _removedStaticBlock[x] = new bool[BlockHeight];
            _removedStaticBlock[x][y] = true;
            if (_staticTiles[x] == null)
                _staticTiles[x] = new HuedTile[BlockHeight][][][];
            _staticTiles[x][y] = EmptyStaticBlock;
        }

        public bool IsStaticBlockRemoved(int x, int y) => _removedStaticBlock == null || _removedStaticBlock[x] == null ? false : _removedStaticBlock[x][y];

        public bool IsPendingStatic(int x, int y)
        {
            if (_staticTilesToAdd == null || _staticTilesToAdd[y] == null || _staticTilesToAdd[y][x] == null)
                return false;
            return true;
        }

        public void AddPendingStatic(int x, int y, StaticTile toadd)
        {
            if (_staticTilesToAdd == null)
                _staticTilesToAdd = new List<StaticTile>[BlockHeight][];
            if (_staticTilesToAdd[y] == null)
                _staticTilesToAdd[y] = new List<StaticTile>[BlockWidth];
            if (_staticTilesToAdd[y][x] == null)
                _staticTilesToAdd[y][x] = new List<StaticTile>();
            _staticTilesToAdd[y][x].Add(toadd);
        }

        public StaticTile[] GetPendingStatics(int x, int y) => _staticTilesToAdd == null || _staticTilesToAdd[y] == null || _staticTilesToAdd[y][x] == null ? null : _staticTilesToAdd[y][x].ToArray();

        /// <summary>
        /// Important: x >> 3, y >> 3
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private HuedTile[][][] ReadStaticBlock(int x, int y)
        {
            int lookup = (int)_staticIndex[(x * BlockHeight) + y].Offset;
            int length = _staticIndex[(x * BlockHeight) + y].Length;

            if (lookup < 0 || length <= 0)
                return EmptyStaticBlock;

            int count = length / 7;
            _filestatics.Seek(lookup);

            if (_list == null)
            {
                _list = new HuedTileList[8][];

                for (int i = 0; i < 8; i++)
                {
                    _list[i] = new HuedTileList[8];
                    for (int j = 0; j < 8; j++)
                        _list[i][j] = new HuedTileList();
                }
            }

            HuedTileList[][] list = _list;

            for (int i = 0; i < count; i++)
            {
                StaticTile tile = new StaticTile()
                {
                    ID = _filestatics.ReadUShort(),
                    X = _filestatics.ReadByte(),
                    Y = _filestatics.ReadByte(),
                    Z = _filestatics.ReadSByte(),
                    Hue = _filestatics.ReadUShort()
                };

                list[tile.X & 0x7][tile.Y & 0x7].Add(tile.ID, tile.Hue, tile.Z);
            }

            HuedTile[][][] tiles = new HuedTile[8][][];

            for (int i = 0; i < 8; i++)
            {
                tiles[i] = new HuedTile[8][];
                for (int j = 0; j < 8; j++)
                    tiles[i][j] = list[i][j].ToArray();
            }

            return tiles;
        }

        private Tile[] ReadLandBlock(int x, int y)
        {
            Tile[] tiles = new Tile[64];
            long offset = ((x * BlockHeight) + y) * 194 + 4;

            if (_file is UOFileUop uopfile)
                offset = uopfile.GetOffsetFromUOP(offset);

            _file.Seek(offset);

            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].ID = _file.ReadUShort();
                tiles[i].Z = _file.ReadSByte();
            }

            return tiles;
        }
    }

    public struct StaticTile
    {
        public ushort ID;
        public byte X, Y;
        public sbyte Z;
        public ushort Hue;
    }

    public struct HuedTile
    {
        public HuedTile(ushort id, ushort hue, sbyte z)
        {
            ID = id; Hue = hue; Z = z;
        }

        public sbyte Z;
        public ushort ID;
        public int Hue;

        public void Set(ushort id, short hue, sbyte z)
        {
            ID = id; Hue = hue; Z = z;
        }
    }

    public struct MTile : IComparable
    {
        public MTile(ushort id, sbyte z)
        {
            ID = id; Z = z; Flag = 1; Solver = 0; Unk1 = 0;
        }

        public MTile(ushort id, sbyte z, sbyte flag)
        {
            ID = id; Z = z; Flag = flag; Solver = 0; Unk1 = 0;
        }

        public MTile(ushort id, sbyte z, sbyte flag, int unk1)
        {
            ID = id; Z = z; Flag = flag; Solver = 0; Unk1 = unk1;
        }

        public ushort ID;
        public sbyte Z;
        public sbyte Flag;
        public int Unk1;
        public int Solver;

        public void Set(ushort id, sbyte z)
        {
            ID = id; Z = z;
        }

        public void Set(ushort id, sbyte z, sbyte flag)
        {
            ID = id; Z = z; Flag = flag; Unk1 = 0;
        }

        public void Set(ushort id, sbyte z, sbyte flag, int unk1)
        {
            ID = id; Z = z; Flag = flag; Unk1 = unk1;
        }

        public int CompareTo(object x)
        {
            if (x == null) return 1;
            if (!(x is MTile a))
                throw new ArgumentNullException();

            var ourData = TileData.StaticData[ID];
            var theirData = TileData.StaticData[a.ID];

            int ourTreshold = 0;
            if (ourData.Height > 0)
                ++ourTreshold;
            if (!ourData.IsBackground)
                ++ourTreshold;
            int ourZ = Z;
            int theirTreshold = 0;
            if (theirData.Height > 0)
                ++theirTreshold;
            if (!theirData.IsBackground)
                ++theirTreshold;
            int theirZ = a.Z;

            ourZ += ourTreshold;
            theirZ += theirTreshold;
            int res = ourZ - theirZ;
            if (res == 0)
                res = ourTreshold - theirTreshold;
            if (res == 0)
                res = Solver - a.Solver;
            return res;
        }

    }

    public struct Tile : IComparable
    {
        public Tile(ushort id, sbyte z)
        {
            ID = id; Z = z;
        }

        public ushort ID;
        public sbyte Z;

        public void Set(ushort id, sbyte z)
        {
            ID = id; Z = z;
        }

        public int CompareTo(object x)
        {
            if (x == null)
                return 1;

            if (!(x is Tile))
                throw new ArgumentNullException();

            Tile a = (Tile)x;

            if (Z > a.Z)
                return 1;
            else if (a.Z > Z)
                return -1;

            var ourData = TileData.StaticData[ID];
            var theirData = TileData.StaticData[a.ID];

            if (ourData.Height > theirData.Height)
                return 1;
            else if (theirData.Height > ourData.Height)
                return -1;

            if (ourData.IsBackground && !theirData.IsBackground)
                return -1;
            else if (theirData.IsBackground && !ourData.IsBackground)
                return 1;

            return 0;
        }
    }
}
