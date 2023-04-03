using EspionSpotify.AudioSessions;
using EspionSpotify.AudioSessions.NAudio;
using Moq;
using NAudio.Wave;
using System;
using System.Threading;
using Xunit;

namespace EspionSpotify.Tests
{
    public class AudioLoopbackTests
    {
        private const int SAMPLE_RATE = 48_000;
        private const int CHANNELS = 2;
        private WaveFormat _defaultWaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(SAMPLE_RATE, CHANNELS);

        private Mock<IAudioLoopbackCapture> _audioLoopbackCaptureMock;
        private Mock<IAudioWaveOut> _audioWaveOutMock;

        public AudioLoopbackTests()
        {
            _audioLoopbackCaptureMock = new Mock<IAudioLoopbackCapture>();
            _audioWaveOutMock = new Mock<IAudioWaveOut>();

            _audioLoopbackCaptureMock = new Mock<IAudioLoopbackCapture>();
            _audioLoopbackCaptureMock.Setup(x => x.WaveFormat).Returns(_defaultWaveFormat);
            _audioLoopbackCaptureMock.Setup(x => x.CaptureState).Returns(NAudio.CoreAudioApi.CaptureState.Capturing);
        }

        [Fact]
        internal void Running_Falsy()
        {
            var audioLoopback = CreateAudioLoopback();

            Assert.False(audioLoopback.Running);
        }

        [Fact]
        internal void Running_Truthy()
        {
            var audioLoopback = CreateAudioLoopback();

            audioLoopback.Run(new CancellationTokenSource());
            
            Assert.True(audioLoopback.Running);
        }

        [Fact]
        internal void Dispose_WhenRunning()
        {
            var audioLoopback = CreateAudioLoopback();

            audioLoopback.Run(new CancellationTokenSource());
            audioLoopback.Dispose();

            Assert.False(audioLoopback.Running);
            _audioLoopbackCaptureMock.Verify(x => x.Dispose(), Times.Once());
            _audioWaveOutMock.Verify(x => x.Dispose(), Times.Once());
        }

        [Fact]
        internal void Dispose_BeforeRunning()
        {
            var audioLoopback = CreateAudioLoopback();
            audioLoopback.Dispose();
        }

        private IAudioLoopback CreateAudioLoopback(bool sameDevice = false)
        {
            var identifier = Guid.NewGuid();
            return new AudioLoopback(
                identifier.ToString(),
                sameDevice ? identifier.ToString() : Guid.NewGuid().ToString(),
                _audioLoopbackCaptureMock.Object,
                _audioWaveOutMock.Object);
        }
    }
}
