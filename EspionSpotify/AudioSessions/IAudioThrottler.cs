using System;
using System.Threading;
using System.Threading.Tasks;
using EspionSpotify.Models;
using NAudio.Wave;

namespace EspionSpotify.AudioSessions
{
    public interface IAudioThrottler
    {
        bool Running { get; set; }
        WaveFormat WaveFormat { get; }

        // Task<bool> WaitForWorkerReadPositionReadiness(Guid identifier, int timeoutInSeconds);
        // Task<bool> WaitForWorkerStopPositionReadiness(Guid identifier, int timeoutInSeconds);

        void AddWorker(Guid identifier);
        int? GetWorkerPosition(Guid identifier);
        void StopWorker(Guid identifier);
        void RemoveWorker(Guid identifier);
        bool StopWorkerExist(Guid identifier);

        Task<AudioWaveBuffer> GetData(Guid identifier);
        Task<AudioWaveBuffer> GetDataStart(Guid identifier, bool detectSilence);
        Task<AudioWaveBuffer> GetDataEnd(Guid identifier, bool detectSilence);

        Task Run(CancellationTokenSource cancellationTokenSource);

        void Dispose();
    }
}