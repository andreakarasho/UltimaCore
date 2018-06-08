using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;

namespace UltimaCore
{
    public unsafe abstract class UOFile
    {
        //private BinaryReader _reader;

        public UOFile(string filepath)
        {
            FileName = filepath;
            Path = System.IO.Path.GetDirectoryName(FileName);
        }

        public string FileName { get; }
        public string Path { get; }
        public long Length => _length;  //_reader.BaseStream.Length;
        public UOFileIndex3D[] Entries { get; protected set; }
        public int Position { get => _position; set => _position = value; }

        protected byte* _ptr;
        protected int _position;
        protected long _length;

        public IntPtr StartAddress => (IntPtr)_ptr;

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
                var stream = file.CreateViewStream(0, size, MemoryMappedFileAccess.Read);
                //_reader = new BinaryReader(stream);
                _position = 0;
                _length = stream.Length;
                stream.SafeMemoryMappedViewHandle.AcquirePointer(ref _ptr);
            }
            else
                throw new UOFileException($"{FileName} size must has > 0");
        }

        /*internal byte ReadByte() => _reader.ReadByte();
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
        internal long Seek(long count) => _reader.BaseStream.Seek(count, SeekOrigin.Begin);*/

        internal byte ReadByte() => _ptr[_position++];
        internal sbyte ReadSByte() => (sbyte)ReadByte();
        internal short ReadShort() => (short)(ReadByte() | (ReadByte() << 8));
        internal ushort ReadUShort() => (ushort)ReadShort();
        internal int ReadInt() => (ReadByte() | (ReadByte() << 8) | (ReadByte() << 16) | (ReadByte() << 24));
        internal uint ReadUInt() => (uint)ReadInt();
        internal long ReadLong() => (ReadByte() | ((long)ReadByte() << 8) | ((long)ReadByte() << 16) | ((long)ReadByte() << 24) | ((long)ReadByte() << 32) | ((long)ReadByte() << 40) | ((long)ReadByte() << 48) | ((long)ReadByte() << 56));
        internal ulong ReadULong() => (ulong)ReadLong();
        internal byte[] ReadArray(int count)
        {
            byte[] buffer = new byte[count];

            for (int i = 0; i < count; i++)
                buffer[i] = ReadByte();
            return buffer;
        }
        internal void Skip(int count) => _position += count;
        internal void Seek(int count) => _position = count;
        internal void Seek(long count) => _position = (int)count;
    }

    public class UOFileException : Exception
    {
        public UOFileException(string text) : base(text) { }
    }
}
