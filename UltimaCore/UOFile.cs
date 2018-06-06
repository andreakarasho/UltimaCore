using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;

namespace UltimaCore
{
    public abstract class UOFile
    {
        private BinaryReader _reader;

        public UOFile(string filepath)
        {
            FileName = filepath;
            Path = System.IO.Path.GetDirectoryName(FileName);
        }

        public string FileName { get; }
        public string Path { get; }
        public long Length => _reader.BaseStream.Length;
        public UOFileIndex3D[] Entries3D { get; set; }

        protected virtual void Load()
        {
            FileInfo fileInfo = new FileInfo(FileName);
            if (!fileInfo.Exists)
                throw new UOFileException(FileName + " not exists.");            
            long size = fileInfo.Length;
            if (size > 0)
            {
                var file = MemoryMappedFile.CreateFromFile(fileInfo.FullName, FileMode.Open);
                if (file == null)
                    throw new UOFileException("Something goes wrong with file mapping creation '" + FileName + "'");
                _reader = new BinaryReader(file.CreateViewStream(0, size, MemoryMappedFileAccess.Read));
            }
            else
                throw new UOFileException($"{FileName} size must has > 0");
        }

        internal byte ReadByte() => _reader.ReadByte();
        internal sbyte ReadSByte() => _reader.ReadSByte();
        internal short ReadShort() => _reader.ReadInt16();
        internal ushort ReadUShort() => _reader.ReadUInt16();
        internal int ReadInt() => _reader.ReadInt32();
        internal uint ReadUInt() => _reader.ReadUInt32();
        internal long ReadLong() => _reader.ReadInt64();
        internal ulong ReadULong() => _reader.ReadUInt64();
        internal byte[] ReadArray(int count)
        {
            byte[] buffer = new byte[count];
            _reader.Read(buffer, 0, count);
            return buffer;
        }

        internal void Skip(int count) => _reader.BaseStream.Seek(count, SeekOrigin.Current);
        internal long Seek(int count) => _reader.BaseStream.Seek(count, SeekOrigin.Begin);
        internal long Seek(long count) => _reader.BaseStream.Seek(count, SeekOrigin.Begin);

    }

    public class UOFileException : Exception
    {
        public UOFileException(string text) : base(text) { }
    }
}
