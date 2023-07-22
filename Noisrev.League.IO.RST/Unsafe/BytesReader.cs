// Copyright (c) 2021 - 2023 Noisrev
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Noisrev.League.IO.RST.Unsafe;

internal unsafe class BytesReader : IDisposable
{
    private const byte Empty = 0;

    private readonly byte* _byRef;
    private readonly int _length;
    private readonly Encoding _encoding;
    private object? _managedObject;
    private Action<object?>? _disposeAction;

    private int _position;

    public int Position => _position;

    public int Length => _length;

    private BytesReader(byte* pointer, int length, Encoding encoding, object? managedObject = null, Action<object?>? disposeAction = null)
    {
        _byRef = pointer;
        _length = length;
        _encoding = encoding;
        _managedObject = managedObject;
        _disposeAction = disposeAction;
    }

    public ReadOnlySpan<byte> Read(int count)
    {
        var newPos = checked(_position + count);
        if (newPos >= _length)
            throw new ArgumentOutOfRangeException(nameof(count));

        var span = new ReadOnlySpan<byte>(_byRef + _position, count);

        _position = newPos;
        return span;
    }

    public ReadOnlySpan<byte> Read(int offset, int count)
    {
        var newPos = checked(offset + count);
        if (newPos >= _length)
            throw new ArgumentOutOfRangeException(nameof(count));

        var span = new ReadOnlySpan<byte>(_byRef + offset, count);

        _position = newPos;
        return span;
    }

    public byte ReadByte() => _byRef[_position++];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte ReadSByte() => (sbyte)ReadByte();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBoolean() => ReadByte() != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char ReadChar() => (char)ReadByte();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadInt16() => MemoryMarshal.Read<short>(Read(2));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUInt16() => MemoryMarshal.Read<ushort>(Read(2));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt32() => MemoryMarshal.Read<int>(Read(4));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt32() => MemoryMarshal.Read<uint>(Read(4));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadInt64() => MemoryMarshal.Read<long>(Read(8));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadUInt64() => MemoryMarshal.Read<ulong>(Read(8));

    public string ReadString()
    {
        var span = new ReadOnlySpan<byte>(_byRef + _position, Length - _position);
        var length = span.IndexOf(Empty);

        if (length == 0) return string.Empty;

        _position += length;
        return _encoding.GetString(_byRef + _position, length);
    }

    public string ReadString(int count)
    {
        return _encoding.GetString(Read(count).ToArray());
    }

    public string ReadStringWithOffset(int offset)
    {
        var span = new ReadOnlySpan<byte>(_byRef + offset, Length - offset);
        var length = span.IndexOf(Empty);

        if (length == 0) return string.Empty;

        _position = offset + length;
        return _encoding.GetString(_byRef + offset, length);
    }

    public void Dispose()
    {
        var managedObject = _managedObject;
        if (managedObject is null)
        {
            return;
        }

        var disposeAction = _disposeAction;
        if (disposeAction is null)
        {
            return;
        }

        _managedObject = null;
        _disposeAction = null;

        disposeAction.Invoke(managedObject);
    }

    public static BytesReader Create(Stream stream, Encoding encoding)
    {
        var position = stream.Position;
        var rentSize = (int)(stream.Length - position);

        if (stream is MemoryStream memoryStream)
        {
            return new BytesReader((byte*)Marshal.UnsafeAddrOfPinnedArrayElement(memoryStream.GetBuffer(), (int)position), rentSize, encoding);
        }
#if NET6_0_OR_GREATER
        else if (stream is FileStream fileStream)
        {
            Exception? excp = null;
            var bufferPtr = Marshal.AllocHGlobal(rentSize);
            try
            {
                var buffer = new Span<byte>((byte*)bufferPtr.ToPointer(), rentSize);
                RandomAccess.Read(fileStream.SafeFileHandle, buffer, position);
            }
            catch (Exception sourceException)
            {
                Marshal.FreeHGlobal(bufferPtr);
                excp = sourceException;
            }

            if (excp is not null)
            {
                throw excp;
            }

            return new BytesReader((byte*)bufferPtr.ToPointer(), rentSize, encoding, bufferPtr, (obj) =>
            {
                if (obj is IntPtr ptr) Marshal.FreeHGlobal(ptr);
            });
        }
#endif
        else if (stream is UnmanagedMemoryStream unmanagedMemoryStream)
        {
            return new BytesReader(unmanagedMemoryStream.PositionPointer, rentSize, encoding);
        }
        else
        {
            var buffer = ArrayPool<byte>.Shared.Rent(rentSize);

            var totalRead = 0;
            while (totalRead < rentSize)
            {
                var read = stream.Read(buffer, totalRead, rentSize - totalRead);
                if (read == 0)
                {
                    break;
                }

                totalRead += read;
            }

            return new BytesReader((byte*)Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0), rentSize, Encoding.UTF8, buffer, (obj) =>
            {
                if (obj is byte[] array) ArrayPool<byte>.Shared.Return(array);
            });
        }

    }
}
