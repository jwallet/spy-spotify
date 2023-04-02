using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EspionSpotify.AudioSessions.NAudio
{
    public class AudioLoopbackCapture : IAudioLoopbackCapture, IDisposable
    {
        private bool _disposed;
        private WasapiLoopbackCapture _wasapiLoopbackCapture;
        private bool _stopped;
        private int _timeoutDispose = 1000;

        public AudioLoopbackCapture(MMDevice device)
        {
            _wasapiLoopbackCapture = new WasapiLoopbackCapture(device);
            _wasapiLoopbackCapture.DataAvailable += RaisedDataAvailable;
            _wasapiLoopbackCapture.RecordingStopped += RaisedRecordingStopped;
        }

        public CaptureState CaptureState => _wasapiLoopbackCapture.CaptureState;

        public WaveFormat WaveFormat => _wasapiLoopbackCapture.WaveFormat;

        public event EventHandler<WaveInEventArgs> DataAvailable;
        public event EventHandler<StoppedEventArgs> RecordingStopped;

        public void StartRecording()
        {
            _wasapiLoopbackCapture.StartRecording();
        }

        public void StopRecording()
        {
            _wasapiLoopbackCapture.StopRecording();
        }

        private void RaisedDataAvailable(object sender, WaveInEventArgs e)
        {
            DataAvailable.Invoke(this, e);
        }

        private void RaisedRecordingStopped(object sender, StoppedEventArgs e)
        {
            RecordingStopped.Invoke(this, e);
            _stopped = true;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private async void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _wasapiLoopbackCapture.StopRecording();
                while (!_stopped && _timeoutDispose > 0)
                {
                    await Task.Delay(100);
                    _timeoutDispose -= 100;
                }
                _wasapiLoopbackCapture.DataAvailable -= RaisedDataAvailable;
                _wasapiLoopbackCapture.RecordingStopped -= RaisedRecordingStopped;
                _wasapiLoopbackCapture.Dispose();
            }

            _disposed = true;
        }
    }
}
