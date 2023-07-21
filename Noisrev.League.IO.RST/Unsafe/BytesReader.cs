// Copyright (c) 2021 - 2023 Noisrev
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Noisrev.League.IO.RST.Unsafe;

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
        var newPos = checked(_position + count);
        if (newPos >= _buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(count));

        var span = new ReadOnlySpan<byte>(_buffer, _position, count);

        _position = newPos;
        return span;
    }

    public ReadOnlySpan<byte> Read(int offset, int count)
    {
        var newPos = checked(offset + count);
        if (newPos >= _buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(count));

        var span = new ReadOnlySpan<byte>(_buffer, offset, count);

        _position = newPos;
        return span;
    }

    public byte ReadByte()
    {
        return _buffer[_position++];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte ReadSByte()
    => (sbyte)ReadByte();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBoolean()
    => ReadByte() != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char ReadChar()
    => (char)ReadByte();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadInt16()
    => MemoryMarshal.Read<short>(Read(2));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUInt16()
    => MemoryMarshal.Read<ushort>(Read(2));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt32()
    => MemoryMarshal.Read<int>(Read(4));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt32()
    => MemoryMarshal.Read<uint>(Read(4));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadInt64()
    => MemoryMarshal.Read<long>(Read(8));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadUInt64()
    => MemoryMarshal.Read<ulong>(Read(8));

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
