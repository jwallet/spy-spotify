using System;
using System.Threading;
using System.Threading.Tasks;
using EspionSpotify.Enums;
using EspionSpotify.Models;
using NAudio.Wave;

namespace EspionSpotify.AudioSessions
{
    public interface IAudioThrottler
    {
        bool Running { get; set; }
        WaveFormat WaveFormat { get; }
        //bool BufferIsReady { get; }

        Task<bool> WaitForWorkerPositionReady(Guid identifier, int timeout);

        void AddWorker(Guid identifier);
        int? GetWorkerPosition(Guid identifier);
        void RemoveWorker(Guid identifier);

        Task<AudioWaveBuffer> GetData(Guid identifier);

        void TrimEndBufferForSilence(ref byte[] buffer);

        Task Run(CancellationTokenSource cancellationTokenSource);

        void Dispose();
    }
}