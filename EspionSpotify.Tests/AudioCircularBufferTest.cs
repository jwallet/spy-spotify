using EspionSpotify.AudioSessions;
using Xunit;

namespace EspionSpotify.Tests
{
    public class AudioCircularBufferTest
    {
        [Fact]
        internal void MaxLength_EqualsSize()
        {
            var size = 4;
            var buffer = new AudioCircularBuffer(size);

            Assert.Equal(size, buffer.MaxLength);
        }

        [Fact]
        internal void ReadPosition_Default()
        {
            var buffer = new AudioCircularBuffer(4);

            var position = buffer.GetDefaultReadPosition();

            Assert.Equal(0, position);
        }

        [Fact]
        internal void ReadPosition_Offset()
        {
            var buffer = new AudioCircularBuffer(4);

            buffer.Write(new byte[] { 1, 2, 3, 4 }, 0, 3);
            var position = buffer.GetDefaultReadPosition(2);

            Assert.Equal(1, position);
        }

        [Fact]
        internal void ReadPosition_OffsetAfterLoop()
        {
            var buffer = new AudioCircularBuffer(4);

            buffer.Write(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 0, 6);
            var position = buffer.GetDefaultReadPosition(4);

            Assert.Equal(2, position);
        }

        [Fact]
        internal void Write_KeepPosition()
        {
            var size = 8;
            var buffer = new AudioCircularBuffer(size);
            var dataToWrite = new byte[] { 1, 2, 3, 4 };
            var count = dataToWrite.Length;

            var bytesWritten = buffer.Write(dataToWrite, 0, count);

            Assert.Equal(count, bytesWritten);
            Assert.Equal(count, buffer.TotalBytesWritten);
            Assert.Equal(count, buffer.BytesWritten);
            Assert.Equal(count, buffer.BytesAvailable);
            Assert.Equal(count, buffer.WritePosition);
        }

        [Fact]
        internal void Write_Partially()
        {
            var size = 8;
            var buffer = new AudioCircularBuffer(size);
            var dataToWrite = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var count = dataToWrite.Length / 2;
            
            var bytesWritten = buffer.Write(dataToWrite, 0, count);

            Assert.Equal(count, bytesWritten);
            Assert.Equal(count, buffer.TotalBytesWritten);
            Assert.Equal(count, buffer.BytesWritten);
            Assert.Equal(size - count, buffer.BytesAvailable);
            Assert.Equal(count, buffer.WritePosition);
        }

        [Fact]
        internal void Write_ReadEarly()
        {
            var size = 8;
            var buffer = new AudioCircularBuffer(size);
            var dataToWrite = new byte[] { 1, 2, 3, 4 };
            var count = dataToWrite.Length;

            var bytesWritten = buffer.Write(dataToWrite, 0, count);
            var bytesRead = buffer.Read(out var dataRead, 0, buffer.TotalBytesWritten);

            Assert.Equal(count, bytesWritten);
            Assert.Equal(count, bytesRead);

            Assert.Equal(dataToWrite, dataRead);
            Assert.Equal(size - count, buffer.BytesAvailable);
            Assert.Equal(count, buffer.WritePosition);
        }

        [Fact]
        internal void Write_ReadFully()
        {
            var size = 4;
            var buffer = new AudioCircularBuffer(size);
            var dataToWrite = new byte[] { 1, 2, 3, 4 };
            var count = dataToWrite.Length;

            var bytesWritten = buffer.Write(dataToWrite, 0, count);
            var bytesRead = buffer.Read(out var dataRead, 0, buffer.TotalBytesWritten);

            Assert.Equal(count, bytesWritten);
            Assert.Equal(count, bytesRead);

            Assert.Equal(dataToWrite, dataRead);
            Assert.Equal(size, buffer.BytesAvailable);
            Assert.Equal(0, buffer.WritePosition);
        }

        [Fact]
        internal void Write_ReadPartially()
        {
            var size = 4;
            var buffer = new AudioCircularBuffer(size);
            var dataToWrite = new byte[] { 1, 2, 3, 4 };
            var count = dataToWrite.Length;

            buffer.Write(dataToWrite, 0, count);
            var readOffset = 1;
            var bytesToRead = buffer.TotalBytesWritten - 2;
            var bytesRead = buffer.Read(out var dataRead, readOffset, bytesToRead);

            Assert.Equal(bytesToRead, bytesRead);

            Assert.Equal(new byte[] { 2, 3, 0, 0 }, dataRead);
            Assert.Equal(count, buffer.BytesWritten);
            Assert.Equal(count, buffer.TotalBytesWritten);
        }

        [Fact]
        internal void DoubleWrite_ReadOverFully()
        {
            var size = 4;
            var buffer = new AudioCircularBuffer(size);
            var dataToWriteFirst = new byte[] { 1, 2, 3, 4 };
            var dataToWriteSecond = new byte[] { 5, 6, 7, 8 };

            var bytesWritten = buffer.Write(dataToWriteFirst, 0, dataToWriteFirst.Length);
            bytesWritten += buffer.Write(dataToWriteSecond, buffer.BytesWritten, dataToWriteSecond.Length);
            var bytesRead = buffer.Read(out var dataRead, buffer.GetDefaultReadPosition(), size);

            Assert.Equal(dataToWriteFirst.Length + dataToWriteSecond.Length, bytesWritten);
            Assert.Equal(size, bytesRead);
            Assert.Equal(dataToWriteSecond, dataRead);
        }

        [Fact]
        internal void DoubleWrite_ReadOverPartially()
        {
            var size = 8;
            var buffer = new AudioCircularBuffer(size);
            var dataToWriteFirst = new byte[] { 1, 2, 3, 4 };
            var dataToWriteSecond = new byte[] { 5, 6, 7, 8, 9, 10, 11, 12 };

            var bytesWritten = buffer.Write(dataToWriteFirst, 0, dataToWriteFirst.Length);
            bytesWritten += buffer.Write(dataToWriteSecond, 0, dataToWriteSecond.Length);
            var offset = buffer.GetDefaultReadPosition(dataToWriteSecond.Length);
            var bytesRead = buffer.Read(out var dataRead, offset, size);

            Assert.Equal(dataToWriteFirst.Length + dataToWriteSecond.Length, bytesWritten);
            Assert.Equal(size, bytesRead);
            Assert.Equal(dataToWriteSecond, dataRead);
        }

        [Fact]
        internal void Write_Reset()
        {
            var size = 8;
            var buffer = new AudioCircularBuffer(size);
            var dataToWrite = new byte[] { 1, 2, 3, 4 };
            var count = dataToWrite.Length;

            var bytesWritten = buffer.Write(dataToWrite, 0, count);
            buffer.Reset();

            Assert.Equal(count, bytesWritten);
            Assert.Equal(0, buffer.WritePosition);
            Assert.Equal(0, buffer.BytesWritten);
            Assert.Equal(0, buffer.TotalBytesWritten);
        }
    }
}
