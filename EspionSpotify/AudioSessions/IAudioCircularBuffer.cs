namespace EspionSpotify.AudioSessions
{
    public interface IAudioCircularBuffer
    {
        int MaxLength { get; }
        int Count { get; }
        int ReadPosition { get; }
        int WritePosition { get; }

        int Write(byte[] data, int offset, int count);
        int Read(out byte[] data, int offset, int count);
        int Peek(out byte[] data, int offset, int count);
        void Advance(int count);
        void Reset();
    }
}