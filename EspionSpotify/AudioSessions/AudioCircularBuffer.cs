using System;

namespace EspionSpotify.AudioSessions
{
    // https://github.com/naudio/NAudio/blob/master/NAudio.Core/Utils/CircularBuffer.cs
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
                    var bytesOver = (int)(_totalBytesWritten % _buffer.Length);
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
        /// <param name="offset">Write position into the buffer destination to offset</param>
        /// <param name="count">Number of bytes to write</param>
        /// <returns>number of bytes written</returns>
        public int Write(byte[] data, int offset, int count)
        {
            var bytesToWrite = count > data.Length ? data.Length : count;
            var cursor = offset % MaxLength;
            var previousTotalBytesWritten = _totalBytesWritten;

            lock (_lockObject)
            {
                while (bytesToWrite > 0)
                {
                    int bytesWritten = Math.Min(BytesAvailable, bytesToWrite);
      
                    try
                    {
                        Array.Copy(data, cursor, _buffer, _writePosition, bytesWritten);
                    }
                    catch (Exception ex)
                    {
                        if (ex is ArgumentOutOfRangeException)
                        {
                            // _writePosition is based on _totalBytesWritten which can overflow past long.MaxValue
                            // 48khz: it will take 245 years of recording to overflow
                            // 192khz: it wil take 64 years of recording to overflow
                            Console.WriteLine("AudioCircularBuffer: TotalBytesWritten reached long.MaxValue. Lower the audio device default format.");
                            Reset();
                        }

                        Program.ReportException(ex);

                        throw ex;
                    }

                    _totalBytesWritten = (_totalBytesWritten + bytesWritten) % int.MaxValue;
                    bytesToWrite = Math.Max(0, bytesToWrite - bytesWritten);
                    cursor = (cursor + bytesWritten) % MaxLength;
                    _writePosition = _totalBytesWritten % MaxLength;
                }
            }

            return Math.Max(0, _totalBytesWritten - previousTotalBytesWritten);
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
            var cursor = position % MaxLength;
            // var previousTotalBytesWritten = position;
            var bytesRead = 0;
            
            lock (_lockObject)
            {
                while (bytesToRead > 0)
                {
                    var readToEnd = Math.Min(totalBuffer - cursor, bytesToRead);

                    if (readToEnd > 0)
                    {
                        try
                        {
                            Array.Copy(_buffer, cursor, data, bytesRead, readToEnd);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("AudioCircularBuffer: Error reading buffer");
                            Program.ReportException(ex);
                        }
                        bytesToRead = Math.Max(0, bytesToRead - readToEnd);
                        bytesRead += readToEnd;
                        cursor = (cursor + readToEnd) % MaxLength;
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