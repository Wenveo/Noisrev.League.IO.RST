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

namespace Noisrev.League.IO.RST.Internal;

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
        var length = new ReadOnlySpan<byte>(_byRef + _position, _length - _position).IndexOf(Empty);
        if (length > 0)
        {
            _position += length;
            return _encoding.GetString(_byRef + _position, length);
        }

        return string.Empty;
    }

    public string ReadString(int count)
    {
        var newPos = checked(_position + count);
        if (newPos >= _length)
            throw new ArgumentOutOfRangeException(nameof(count));

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        var value = Marshal.PtrToStringUTF8((nint)(_byRef + _position));
#else
        var value = count > 0 ? _encoding.GetString(_byRef + _position, count) : null;
#endif
        if (value is not null)
        {
            _position += count;
            return value;
        }

        return string.Empty;
    }

    public string ReadStringByOffset(int offset)
    {
#if NET6_0
        return Marshal.PtrToStringUTF8((nint)(_byRef + offset)) ?? string.Empty;
#else
        var length = new ReadOnlySpan<byte>(_byRef + offset, _length - offset).IndexOf(Empty);
        if (length > 0)
            return _encoding.GetString(_byRef + offset, length);

        return string.Empty;
#endif
    }

    public void Dispose()
    {
        var managedObject = _managedObject;
        if (managedObject is null)
            return;

        var disposeAction = _disposeAction;
        if (disposeAction is null)
            return;

        _managedObject = null;
        _disposeAction = null;

        disposeAction.Invoke(managedObject);
    }

    public static BytesReader Create(Stream stream, Encoding encoding)
    {
        return stream switch
        {
#if NET6_0_OR_GREATER
            FileStream fileStream => CreateByFileStreamDotNet6(fileStream, encoding),
#endif
            MemoryStream memoryStream => CreateByMemoryStream(memoryStream, encoding),
            UnmanagedMemoryStream unmanagedMemoryStream => CreateByUnmanagedMemoryStream(unmanagedMemoryStream, encoding),
            _ => CreateByNormalStream(stream, encoding)
        };
    }

    private static BytesReader CreateByMemoryStream(MemoryStream memoryStream, Encoding encoding)
    {
        var offset = memoryStream.Position;
        var length = memoryStream.Length - offset;

        return new BytesReader((byte*)Marshal.UnsafeAddrOfPinnedArrayElement(memoryStream.GetBuffer(), (int)offset), (int)length, encoding);
    }

    private static BytesReader CreateByUnmanagedMemoryStream(UnmanagedMemoryStream unmanagedMemoryStream, Encoding encoding)
    {
        var length = unmanagedMemoryStream.Length - unmanagedMemoryStream.Position;
        return new BytesReader(unmanagedMemoryStream.PositionPointer, (int)length, encoding);
    }

#if NET6_0_OR_GREATER
    private static BytesReader CreateByFileStreamDotNet6(FileStream fileStream, Encoding encoding)
    {
        var offset = fileStream.Position;
        var length = (int)(fileStream.Length - offset);

        var bufferPtr = Marshal.AllocHGlobal(length);
        var buffer = new Span<byte>((byte*)bufferPtr, length);

        try
        {
            RandomAccess.Read(fileStream.SafeFileHandle, buffer, offset);
        }
        catch
        {
            Marshal.FreeHGlobal(bufferPtr);
            throw;
        }

        return new BytesReader((byte*)bufferPtr, length, encoding, bufferPtr, (obj) =>
        {
            if (obj is nint ptr) Marshal.FreeHGlobal(ptr);
        });
    }
#endif

    private static BytesReader CreateByNormalStream(Stream stream, Encoding encoding)
    {
        var offset = stream.Position;
        var length = (int)(stream.Length - offset);

        var bufferPtr = Marshal.AllocHGlobal(length);
        var buffer = new Span<byte>((byte*)bufferPtr, length);

        var tempBuffer = ArrayPool<byte>.Shared.Rent(4096);
        try
        {
            var position = 0;
            while (position < length)
            {
                var read = stream.Read(tempBuffer, 0, Math.Min(4096, length - position));
                if (read == 0)
                    break;

                new Span<byte>(tempBuffer, 0, read).CopyTo(buffer.Slice(position, read));
                position += read;
            }
        }
        catch
        {
            Marshal.FreeHGlobal(bufferPtr);
            throw;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(tempBuffer);
        }

        return new BytesReader((byte*)bufferPtr, length, encoding, bufferPtr, (obj) =>
        {
            if (obj is nint ptr) Marshal.FreeHGlobal(ptr);
        });
    }
}
