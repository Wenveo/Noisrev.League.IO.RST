/*
 * Copyright (c) Noisrev, GZSkins, Inc.
 * All rights reserved.
 *
 * This source code is distributed under an MIT license. 
 * LICENSE file in the root directory of this source tree.
 */

using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Text;

namespace Noisrev.League.IO.RST.Unsafe
{
    internal unsafe class BytesWriter
    {
        private byte[] _buffer;
        private int _position;
        private int _length;

        public byte[] Buffer => _buffer;
        public int Position => _position;
        public int Length => _length;
        public int Capacity => _buffer.Length;

        private BytesWriter()
        {
            _buffer = ArrayPool<byte>.Shared.Rent(4);
            ArrayPool<byte>.Shared.Return(_buffer);
        }

        private BytesWriter(int capacity)
        {
            _buffer = ArrayPool<byte>.Shared.Rent(capacity);
            ArrayPool<byte>.Shared.Return(_buffer);
        }

        public void Write(bool value)
        {
            Contract.Ensures(Contract.Result<BytesWriter>() != null);
            WriteByte((byte)(value ? 1 : 0));
        }

        public void Write(byte value)
        {
            Contract.Ensures(Contract.Result<BytesWriter>() != null);
            WriteByte(value);
        }

        public void Write(sbyte value)
        {
            Contract.Ensures(Contract.Result<BytesWriter>() != null);
            WriteByte((byte)value);
        }

        public void Write(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            CoreceWrite(bytes, 0, bytes.Length);
        }

        public void Write(byte[] bytes, int index, int count)
        {
            CoreceWrite(bytes, index, count);
        }

        public void Write(char[] ch)
        {
            Write(ch, Encoding.ASCII);
        }

        public void Write(char[] ch, Encoding encoding)
        {
            var rented = ArrayPool<byte>.Shared.Rent(encoding.GetByteCount(ch));
            var actualByteCount = encoding.GetBytes(ch, 0, ch.Length, rented, 0);

            CoreceWrite(rented, 0, actualByteCount);
            ArrayPool<byte>.Shared.Return(rented);
        }

        public void Write(double value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(double)];
            MemoryMarshal.Write(buffer, ref value);
            CoreceWrite(buffer);
        }

        public void Write(short value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(short)];
            MemoryMarshal.Write(buffer, ref value);
            CoreceWrite(buffer);
        }

        public void Write(ushort value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(ushort)];
            MemoryMarshal.Write(buffer, ref value);
            CoreceWrite(buffer);
        }

        public void Write(int value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(int)];
            MemoryMarshal.Write(buffer, ref value);
            CoreceWrite(buffer);
        }
        public void Write(uint value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(uint)];
            MemoryMarshal.Write(buffer, ref value);
            CoreceWrite(buffer);
        }

        public void Write(long value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(long)];
            MemoryMarshal.Write(buffer, ref value);
            CoreceWrite(buffer);
        }

        public void Write(ulong value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(ulong)];
            MemoryMarshal.Write(buffer, ref value);
            CoreceWrite(buffer);
        }

        public void Write(float value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(float)];
            MemoryMarshal.Write(buffer, ref value);
            CoreceWrite(buffer);
        }

        public void Write(string value, bool writeEndChar = false)
        {
            Write(value, Encoding.Default, writeEndChar);
        }

        public void Write(string value, Encoding encoding, bool writeEndChar = false)
        {
            var rented = ArrayPool<byte>.Shared.Rent(encoding.GetByteCount(value));
            var actualByteCount = encoding.GetBytes(value, 0, value.Length, rented, 0);

            if (writeEndChar)
                CoreceWriteWithEndByte(rented, 0, actualByteCount);
            else
                CoreceWrite(rented, 0, actualByteCount);
            ArrayPool<byte>.Shared.Return(rented);
        }

        public void WriteByte(byte value)
        {
            CheckOrResize(1);

            _buffer[_position++] = value;
            _length++;
        }

        private void CoreceWrite(Span<byte> buffer)
        {
            var size = buffer.Length;
            CheckOrResize(size);

            var sharedBuffer = ArrayPool<byte>.Shared.Rent(size);
            try
            {
                buffer.CopyTo(sharedBuffer);

                var offsetPtr = (IntPtr)((byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _buffer[0]) + (uint)Position);
                Marshal.Copy(sharedBuffer, 0, offsetPtr, size);

                _position += size;
                _length += size;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }

        private void CoreceWrite(byte[] buffer, int index, int count)
        {
            var size = count;
            CheckOrResize(size);

            var offsetPtr = (IntPtr)((byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _buffer[0]) + (uint)Position);
            Marshal.Copy(buffer, index, offsetPtr, count);

            _position += size;
            _length += size;
        }

        private void CoreceWriteWithEndByte(byte[] buffer, int index, int count)
        {
            var size = count + 1;
            CheckOrResize(size);

            var offsetPtr = (IntPtr)((byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _buffer[0]) + (uint)Position);
            Marshal.Copy(buffer, index, offsetPtr, count);

            _position += size;
            _length += size;
        }

        private void CheckOrResize(int bufferSize)
        {
            if (bufferSize > _buffer.Length - _length)
            {
                var capacity = _buffer.Length;
                var maxValue = Math.Max(bufferSize, capacity);
                var rentedSize = checked(capacity + maxValue);

                var buffer = ArrayPool<byte>.Shared.Rent(rentedSize);
                Array.Copy(_buffer, buffer, _length);

                ArrayPool<byte>.Shared.Return(buffer);
                _buffer = buffer;
            }
        }

        public static BytesWriter Create()
        {
            return new BytesWriter();
        }

        public static BytesWriter Create(int capacity)
        {
            return new BytesWriter(capacity);
        }
    }
}
