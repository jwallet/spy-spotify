namespace EspionSpotify.AudioSessions
{
    public interface IAudioCircularBuffer
    {
        int MaxLength { get; }
        int BytesAvailable { get; }
        int BytesWritten { get; }
        long TotalBytesWritten { get; }
        int WritePosition { get; }

        int GetDefaultReadPosition(int offset);

        int Write(byte[] data, int offset, int count);
        int Read(out byte[] data, long position, int count);

        void Reset();
    }
}