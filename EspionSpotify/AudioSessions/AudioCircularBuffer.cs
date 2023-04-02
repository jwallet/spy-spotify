using NAudio.Utils;
using System;

namespace EspionSpotify.AudioSessions
{
    public class AudioCircularBuffer: IAudioCircularBuffer
    {
        private readonly byte[] _buffer;
        private readonly object _lockObject;
        private int _writePosition;
        private int _totalBytesWritten;

        private bool DidLoopOnce => _totalBytesWritten >= _buffer.Length;

        /// <summary>
        /// Maximum length of this circular buffer
        /// </summary>
        public int MaxLength => _buffer.Length;

        /// <summary>
        /// Number of bytes left on the circular buffer before it needs to loop
        /// </summary>
        public int BytesAvailable
        {
            get
            {
                lock (_lockObject)
                {
                    return _buffer.Length - WritePosition;
                }
            }
        }

        /// <summary>
        /// Number of bytes currently stored in the circular buffer on its current loop
        /// </summary>
        public int BytesWritten
        {
            get
            {
                lock (_lockObject)
                {
                    if (_totalBytesWritten == 0) return 0;
                    var bytesOver = _totalBytesWritten % _buffer.Length;
                    return bytesOver == 0 ? _buffer.Length : bytesOver;
                }
            }
        }

        /// <summary>
        /// Number of bytes written in the circular buffer
        /// </summary>
        public int TotalBytesWritten
        {
            get
            {
                lock (_lockObject)
                {
                    return _totalBytesWritten;
                }
            }
        }
        
        /// <summary>
        /// Write position of the circular buffer
        /// </summary>
        public int WritePosition
        {
            get
            {
                lock (_lockObject)
                {
                    return _writePosition;
                }
            }
        }


        /// <summary>
        /// Default read position of the circular buffer based on an offset of the write position
        /// </summary>
        /// <param name="offset">Offset of the read position</param>
        public int GetDefaultReadPosition(int offset = 0)
        {
            lock (_lockObject)
            {
                if (offset == 0) return WritePosition;
                if (!DidLoopOnce) return Math.Max(0, WritePosition - (offset % MaxLength));

                var readOffset = WritePosition - offset;
                return readOffset < 0 ? Math.Max(0, MaxLength + readOffset) : Math.Min(MaxLength, readOffset);
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
        /// <param name="position">Write position into the buffer destination</param>
        /// <param name="count">Number of bytes to write</param>
        /// <returns>number of bytes written</returns>
        public int Write(byte[] data, int position, int count)
        {
            var bytesToWrite = count > data.Length ? data.Length : count;
            var cursor = position;
            var previousTotalBytesWritten = _totalBytesWritten;

            lock (_lockObject)
            {
                while (bytesToWrite > 0)
                {
                    int bytesWritten = Math.Min(BytesAvailable, bytesToWrite);

                    Array.Copy(data, cursor % MaxLength, _buffer, _writePosition, bytesWritten);

                    _totalBytesWritten += bytesWritten;
                    bytesToWrite -= bytesWritten;
                    cursor += bytesWritten;
                    _writePosition = _totalBytesWritten % MaxLength;
                }
            }

            return _totalBytesWritten - previousTotalBytesWritten;
        }

        /// <summary>
        /// Read from the buffer
        /// </summary>
        /// <param name="data">Data read from the buffer</param>
        /// <param name="position">Read position into the buffer based on total bytes written</param>
        /// <param name="count">Bytes to read</param>
        /// <returns>Number of bytes actually read</returns>
        public int Read(out byte[] data, int position, int count)
        {
            var totalBuffer = Math.Min(_totalBytesWritten, MaxLength);

            data = new byte[totalBuffer];
            var bytesToRead = count;
            var cursor = position;
            var previousTotalBytesWritten = position;
            var bytesRead = 0;
            
            lock (_lockObject)
            {
                while (bytesToRead > 0)
                {
                    var writePositionFromTotalBytes = TotalBytesWritten - previousTotalBytesWritten;
                    var currentPosition = cursor % MaxLength;
                    var readToEnd = Math.Min(Math.Min(totalBuffer - currentPosition, bytesToRead), writePositionFromTotalBytes);

                    if (readToEnd > 0)
                    {
                        Array.Copy(_buffer, currentPosition, data, bytesRead, readToEnd);
                        bytesToRead -= readToEnd;
                        position += readToEnd;
                        bytesRead += readToEnd;
                        cursor += readToEnd;
                    }
                    else
                    {
                        // prevent to continue if end is reached, cannot read any bytes
                        bytesToRead = 0;
                    }
                }
              
                return bytesRead;
            }
        }

        /// <summary>
        /// Resets the buffer
        /// </summary>
        public void Reset()
        {
            lock (_lockObject)
            {
                _totalBytesWritten = 0;
                _writePosition = 0;
            }
        }
    }
}