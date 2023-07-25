// Copyright (c) 2021 - 2023 Noisrev
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Noisrev.League.IO.RST.Unsafe;

internal unsafe class BytesWriter : IDisposable
{
    private readonly ArrayPool<byte> _bufferPool;
    private readonly Encoding _encoding;

    private byte[] _buffer;
    private int _position;
    private int _length;

    private bool _isDisposed;

    public byte[] Buffer => _buffer;

    public int Position => _position;

    public int Length => _length;

    public int Capacity => _buffer.Length;

    private BytesWriter(int capacity, Encoding encoding)
    {
        _bufferPool = ArrayPool<byte>.Shared;
        _buffer = _bufferPool.Rent(capacity);

        _encoding = encoding;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(bool value) => WriteByte((byte)(value ? 1 : 0));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(byte value) => WriteByte(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(sbyte value) => WriteByte((byte)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(char[] ch) => CoreceWrite(_encoding.GetBytes(ch));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(byte[] bytes) => CoreceWrite(bytes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(byte[] bytes, int index, int count) => CoreceWrite(bytes, index, count);

#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public void Write(double value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(double)];
        MemoryMarshal.Write(buffer, ref value);
        CoreceWrite(buffer);
    }

#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public void Write(short value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(short)];
        MemoryMarshal.Write(buffer, ref value);
        CoreceWrite(buffer);
    }

#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public void Write(ushort value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(ushort)];
        MemoryMarshal.Write(buffer, ref value);
        CoreceWrite(buffer);
    }

#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public void Write(int value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        MemoryMarshal.Write(buffer, ref value);
        CoreceWrite(buffer);
    }

#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public void Write(uint value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(uint)];
        MemoryMarshal.Write(buffer, ref value);
        CoreceWrite(buffer);
    }

#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public void Write(long value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(long)];
        MemoryMarshal.Write(buffer, ref value);
        CoreceWrite(buffer);
    }

#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public void Write(ulong value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(ulong)];
        MemoryMarshal.Write(buffer, ref value);
        CoreceWrite(buffer);
    }

#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public void Write(float value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(float)];
        MemoryMarshal.Write(buffer, ref value);
        CoreceWrite(buffer);
    }

#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public void Write(string value)
    {
        fixed (char* ch = value)
        {
            var length = value.Length;
            var byteCount = _encoding.GetByteCount(ch, length);

            // Max Stack Limit 
            Debug.Assert(byteCount <= 1024);
            var tempBuffer = stackalloc byte[byteCount];
            var n = _encoding.GetBytes(ch, length, tempBuffer, byteCount);
            Debug.Assert(n <= byteCount);

            CoreceWrite(new ReadOnlySpan<byte>(tempBuffer, byteCount));
        }
    }

    public void WriteStringWithEndByte(string value)
    {
#if NET5_0_OR_GREATER
        [SkipLocalsInit]
#endif
        void WriteStringViaStackAlloc(char* ch, int length, int byteCount)
        {
            var tempBuffer = stackalloc byte[byteCount];
            var n = _encoding.GetBytes(ch, length, tempBuffer, byteCount);
            Debug.Assert(n <= byteCount);

            tempBuffer[n] = 0;
            CoreceWrite(new ReadOnlySpan<byte>(tempBuffer, byteCount));
        }

#if NET5_0_OR_GREATER
        [SkipLocalsInit]
#endif
        void WriteStringViaArrayPool(char* ch, int length, int byteCount)
        {
            var tempBuffer = ArrayPool<byte>.Shared.Rent(byteCount);
            try
            {
                fixed (byte* bytes = tempBuffer)
                {
                    var n = _encoding.GetBytes(ch, length, bytes, byteCount);
                    Debug.Assert(n <= byteCount);

                    bytes[n] = 0;
                    CoreceWrite(new ReadOnlySpan<byte>(bytes, byteCount));
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(tempBuffer);
            }
        }

        fixed (char* ch = value)
        {
            var length = value.Length;
            var byteCount = _encoding.GetByteCount(ch, length) + 1;

            // Max Stack Limit
            if (byteCount <= 1024)
            {
                WriteStringViaStackAlloc(ch, length, byteCount);
            }
            else
            {
                WriteStringViaArrayPool(ch, length, byteCount);
            }
        }
    }

    public void WriteByte(byte value)
    {
        EnsureCapacity(1);

        _buffer[_position++] = value;
        _length++;
    }

    private void CoreceWrite(ReadOnlySpan<byte> buffer)
    {
        var size = buffer.Length;
        EnsureCapacity(size);

        var span = new Span<byte>(_buffer);
        buffer.CopyTo(span.Slice(_position, size));

        _position += size;
        _length += size;
    }

    private void CoreceWrite(ReadOnlySpan<byte> buffer, int index, int count)
    {
        EnsureCapacity(count);

#if NET6_0_OR_GREATER
        buffer.Slice(index, count).CopyTo(
            new((byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(
                ref MemoryMarshal.GetArrayDataReference(_buffer)) + _position, count));
#else
        buffer.Slice(index, count).CopyTo(new Span<byte>(_buffer, _position, count));
#endif

        _position += count;
        _length += count;
    }

    private void EnsureCapacity(int bufferSize)
    {
        if (bufferSize > _buffer.Length - _length)
        {
            var capacity = _buffer.Length;
            var maxValue = Math.Max(bufferSize, capacity);
            var rentedSize = checked(capacity + maxValue);

            var buffer = _bufferPool.Rent(rentedSize);
            Array.Copy(_buffer, buffer, _length);

            _bufferPool.Return(_buffer);
            _buffer = buffer;
        }
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _bufferPool.Return(_buffer);
    }

    public static BytesWriter Create(int capacity, Encoding encoding)
    {
        return new BytesWriter(capacity, encoding);
    }
}
