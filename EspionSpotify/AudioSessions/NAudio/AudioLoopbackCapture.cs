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
            if (DataAvailable != null)
            {
                DataAvailable.Invoke(this, e);
            }
        }

        private void RaisedRecordingStopped(object sender, StoppedEventArgs e)
        {
            if (RecordingStopped != null)
            {
                RecordingStopped.Invoke(this, e);
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _wasapiLoopbackCapture.DataAvailable -= RaisedDataAvailable;
                _wasapiLoopbackCapture.RecordingStopped -= RaisedRecordingStopped;
                _wasapiLoopbackCapture.Dispose();
            }

            _disposed = true;
        }
    }
}
