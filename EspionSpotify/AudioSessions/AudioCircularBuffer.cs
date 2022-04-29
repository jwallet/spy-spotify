using System;

namespace EspionSpotify.AudioSessions
{
    public class AudioCircularBuffer: IAudioCircularBuffer
    {
        private readonly byte[] _buffer;
        private readonly object _lockObject;
        private int _writePosition;
        private int _readPosition;
        private int _byteCount;

        /// <summary>
        /// Maximum length of this circular buffer
        /// </summary>
        public int MaxLength => _buffer.Length;

        /// <summary>
        /// Number of bytes currently stored in the circular buffer
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lockObject)
                {
                    return _byteCount;
                }
            }
        }
        
        /// <summary>
        /// Create a new circular buffer
        /// </summary>
        /// <param name="size">Max buffer size in bytes</param>
        public AudioCircularBuffer(int size)
        {
            _buffer = new byte[size];
            _lockObject = new object();
        }

        /// <summary>
        /// Write data to the buffer
        /// </summary>
        /// <param name="data">Data to write</param>
        /// <param name="offset">Offset into data</param>
        /// <param name="count">Number of bytes to write</param>
        /// <returns>number of bytes written</returns>
        public int Write(byte[] data, int offset, int count)
        {
            lock (_lockObject)
            {
                var bytesWritten = 0;
                if (count > _buffer.Length - _byteCount)
                {
                    count = _buffer.Length - _byteCount;
                }
                // write to end
                int writeToEnd = Math.Min(_buffer.Length - _writePosition, count);
                Array.Copy(data, offset, _buffer, _writePosition, writeToEnd);
                _writePosition += writeToEnd;
                _writePosition %= _buffer.Length; // if reaches end, resolves to position 0
                bytesWritten += writeToEnd;
                if (bytesWritten < count)
                {
                    // must have wrapped round. Write to start
                    Array.Copy(data, offset + bytesWritten, _buffer, _writePosition, count - bytesWritten);
                    _writePosition += (count - bytesWritten);
                    bytesWritten = count;
                }
                _byteCount += bytesWritten;
                return bytesWritten;
            }
        }

        /// <summary>
        /// Read from the buffer
        /// </summary>
        /// <param name="data">Buffer to read into</param>
        /// <param name="offset">Offset into read buffer</param>
        /// <param name="count">Bytes to read</param>
        /// <returns>Number of bytes actually read</returns>
        public int Read(out byte[] data, int offset, int count)
        {
            data = new byte[MaxLength];
            
            lock (_lockObject)
            {
                if (count > _byteCount)
                {
                    count = _byteCount;
                }
                var bytesRead = 0;
                var readToEnd = Math.Min(_buffer.Length - _readPosition, count);
                Array.Copy(_buffer, _readPosition, data, offset, readToEnd);
                bytesRead += readToEnd;
                _readPosition += readToEnd;
                _readPosition %= _buffer.Length;

                if (bytesRead < count)
                {
                    // must have wrapped round. Read from start
                    Array.Copy(_buffer, _readPosition, data, offset + bytesRead, count - bytesRead);
                    _readPosition += (count - bytesRead);
                    bytesRead = count;
                }

                _byteCount -= bytesRead;
                return bytesRead;
            }
        }
        
        /// <summary>
        /// Peek from the buffer
        /// </summary>
        /// <param name="data">Buffer to read into</param>
        /// <param name="offset">Offset into read buffer</param>
        /// <param name="count">Bytes to read</param>
        /// <returns>Number of bytes actually read</returns>
        public int Peek(out byte[] data, int offset, int count)
        {
            data = new byte[MaxLength];
            var temporaryReadPosition = _readPosition;
            var temporaryBytesRead = _byteCount;
            lock (_lockObject)
            {
                if (count > temporaryBytesRead)
                {
                    count = temporaryBytesRead;
                }
                var bytesRead = 0;
                var readToEnd = Math.Min(_buffer.Length - temporaryReadPosition, count);
                Array.Copy(_buffer, temporaryReadPosition, data, offset, readToEnd);
                bytesRead += readToEnd;
                temporaryReadPosition += readToEnd;
                temporaryReadPosition %= _buffer.Length;

                if (bytesRead < count)
                {
                    // must have wrapped round. Read from start
                    Array.Copy(_buffer, temporaryReadPosition, data, offset + bytesRead, count - bytesRead);
                    bytesRead = count;
                }

                return bytesRead;
            }
        }

        /// <summary>
        /// Advances the buffer, discarding bytes
        /// </summary>
        /// <param name="count">Bytes to advance</param>
        public void Advance(int count)
        {
            lock (_lockObject)
            {
                if (count >= _byteCount)
                {
                    ResetInner();
                }
                else
                {
                    _byteCount -= count;
                    _readPosition += count;
                    _readPosition %= MaxLength;
                }
            }
        }
        
        /// <summary>
        /// Resets the buffer
        /// </summary>
        public void Reset()
        {
            lock (_lockObject)
            {
                ResetInner();
            }
        }

        private void ResetInner()
        {
            _byteCount = 0;
            _readPosition = 0;
            _writePosition = 0;
        }
    }
}