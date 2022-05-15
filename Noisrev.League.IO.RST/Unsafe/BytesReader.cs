/*
 * Copyright (c) Noisrev, GZSkins, Inc.
 * All rights reserved.
 *
 * This source code is distributed under an MIT license. 
 * LICENSE file in the root directory of this source tree.
 */

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Noisrev.League.IO.RST.Unsafe
{
    internal unsafe class BytesReader
    {
        private const byte Empty = 0;
        private readonly byte[] _buffer;
        private readonly Encoding _encoding;
        private int _position;

        public byte[] Buffer => _buffer;
        public int Position => _position;
        public int Length => _buffer.Length;

        private BytesReader(byte[] buffer)
        {
            _buffer = buffer;
            _encoding = Encoding.Default;
        }
        private BytesReader(byte[] buffer, Encoding encoding)
        {
            _buffer = buffer;
            _encoding = encoding;
        }

        public ReadOnlySpan<byte> Read(int count)
        {
            int newPos = checked(_position + count);
            if (newPos >= _buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(newPos));

            var span = new ReadOnlySpan<byte>(_buffer, _position, count);

            _position = newPos;
            return span;
        }

        public ReadOnlySpan<byte> Read(int offset, int count)
        {
            int newPos = checked(offset + count);
            if (newPos >= _buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(newPos));

            var span = new ReadOnlySpan<byte>(_buffer, offset, count);

            _position = newPos;
            return span;
        }

        public byte ReadByte()
        {
            return _buffer[_position++];
        }

        public sbyte ReadSByte()
        {
            return (sbyte)ReadByte();
        }

        public bool ReadBoolean()
        {
            return ReadByte() != 0;
        }

        public char ReadChar()
        {
            var value = (char)ReadByte();
            if (value == -1)
                throw new EndOfStreamException();

            return value;
        }

        public short ReadInt16()
        {
            return MemoryMarshal.Read<short>(Read(2));
        }

        public ushort ReadUInt16()
        {
            return MemoryMarshal.Read<ushort>(Read(2));
        }

        public int ReadInt32()
        {
            return MemoryMarshal.Read<int>(Read(4));
        }

        public uint ReadUInt32()
        {
            return MemoryMarshal.Read<uint>(Read(4));
        }

        public long ReadInt64()
        {
            return MemoryMarshal.Read<long>(Read(8));
        }

        public ulong ReadUInt64()
        {
            return MemoryMarshal.Read<ulong>(Read(8));
        }

        public string ReadString()
        {
            var span = new ReadOnlySpan<byte>(_buffer, _position, Length - _position);
            var length = span.IndexOf(Empty);

            if (length == 0) return string.Empty;

            _position += length;
            return _encoding.GetString(_buffer, _position, length);
        }
        public string ReadString(int count)
        {
            return _encoding.GetString(Read(count).ToArray());
        }

        public string ReadStringWithOffset(int offset)
        {
            var span = new ReadOnlySpan<byte>(_buffer, offset, Length - offset);
            var length = span.IndexOf(Empty);

            if (length == 0) return string.Empty;

            _position = offset + length;
            return _encoding.GetString(_buffer, offset, length);
        }

        public static BytesReader Create(byte[] buffer)
        {
            return new BytesReader(buffer);
        }
        public static BytesReader Create(byte[] buffer, Encoding encoding)
        {
            return new BytesReader(buffer, encoding);
        }
    }
}
