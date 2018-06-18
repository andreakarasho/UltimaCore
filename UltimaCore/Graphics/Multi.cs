using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UltimaCore.Graphics
{
    public static class Multi
    {
        private static UOFileMul _file;

        public static void Load()
        {
            string path = Path.Combine(FileManager.UoFolderPath, "Multi.mul");
            string pathidx = Path.Combine(FileManager.UoFolderPath, "Multi.idx");

            if (!File.Exists(path) || !File.Exists(pathidx))
                throw new FileNotFoundException();

            _file = new UOFileMul(path, pathidx, 0x2000, 14);
        }

        public static MultiComponentList GetMulti(int index)
        {
            index &= FileManager.GraphicMask;
            if (index >= 0 && index < 0x2000)
            {
                (int length, int extra, bool patcher) = _file.SeekByEntryIndex(index);
                return new MultiComponentList(_file, FileManager.ClientVersion >= ClientVersions.CV_7090 ? length / 16 : length / 12);
            }
            return MultiComponentList.Empty;
        }
    }

    public sealed class MultiComponentList
    {
        public static readonly MultiComponentList Empty = new MultiComponentList();

        private int _xMin, _yMin, _xMax, _yMax, _xCenter, _yCenter;
        private int _width, _height, _maxHeight;
        private MTile[][][] _tiles;
        private MultiItem[] _sortedTiles;
        private int _surface;

        public MultiComponentList(UOFileMul file, int count)
        {
            _xMin = _yMin = _xMax = _yMax = 0;
            _sortedTiles = new MultiItem[count];

            for (int i = 0; i < count; i ++)
            {
                _sortedTiles[i].Graphic = file.ReadUShort();
                _sortedTiles[i].OffsetX = file.ReadShort();
                _sortedTiles[i].OffsetY = file.ReadShort();
                _sortedTiles[i].OffsetZ = file.ReadShort();
                _sortedTiles[i].Flags = file.ReadInt();

                if (FileManager.ClientVersion >= ClientVersions.CV_7090)
                    file.Skip(4);

                if (_sortedTiles[i].OffsetX < _xMin)
                    _xMin = _sortedTiles[i].OffsetX;
                if (_sortedTiles[i].OffsetY < _yMin)
                    _yMin = _sortedTiles[i].OffsetY;
                if (_sortedTiles[i].OffsetX > _xMax)
                    _xMax = _sortedTiles[i].OffsetX;
                if (_sortedTiles[i].OffsetY > _yMax)
                    _yMax = _sortedTiles[i].OffsetY;
                if (_sortedTiles[i].OffsetZ > _maxHeight)
                    _maxHeight = _sortedTiles[i].OffsetZ;
            }

            _xCenter = -_xMin;
            _yCenter = -_yMin;
            _width = (_xMax - _xMin) + 1;
            _height = (_yMax - _yMin) + 1;

            MTileList[][] tiles = new MTileList[_width][];
            _tiles = new MTile[_width][][];

            for (int x = 0; x < _width; x++)
            {
                tiles[x] = new MTileList[_height];
                _tiles[x] = new MTile[_height][];

                for (int y = 0; y < _height; y++)
                    tiles[x][y] = new MTileList();
            }

            for (int i = 0; i < _sortedTiles.Length; i++)
            {
                int offsetX = _sortedTiles[i].OffsetX + _xCenter;
                int offsetY = _sortedTiles[i].OffsetY + _yCenter;

                tiles[offsetX][offsetY].Add(_sortedTiles[i].Graphic, (sbyte)_sortedTiles[i].OffsetZ, (sbyte)_sortedTiles[i].Flags);
            }

            _surface = 0;

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _tiles[x][y] = tiles[x][y].ToArray();
                    for (int i = 0; i < _tiles[x][y].Length; i++)
                        _tiles[x][y][i].Solver = i;
                    if (_tiles[x][y].Length > 1)
                        Array.Sort(_tiles[x][y]);
                    if (_tiles[x][y].Length > 0)
                        _surface++;
                }
            }
        }

        private MultiComponentList()
        {
            _sortedTiles = new MultiItem[0];
            _tiles = new MTile[0][][];
        }
    }

    public struct MultiItem
    {
        public ushort Graphic;
        public short OffsetX, OffsetY, OffsetZ;
        public int Flags;
    }
}
