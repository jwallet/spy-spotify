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
        void StopWorker(Guid identifier);
        void RemoveWorker(Guid identifier);
        bool StopWorkerExist(Guid identifier);

        Task<AudioWaveBuffer> GetData(Guid identifier);
        Task<AudioWaveBuffer> GetDataStart(Guid identifier);
        Task<AudioWaveBuffer> GetDataEnd(Guid identifier);

        void TrimEndBufferForSilence(ref byte[] buffer);

        Task Run(CancellationTokenSource cancellationTokenSource);

        void Dispose();
    }
}