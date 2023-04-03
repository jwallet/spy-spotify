using Castle.Core.Internal;
using EspionSpotify.AudioSessions;
using EspionSpotify.AudioSessions.NAudio;
using Moq;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EspionSpotify.Tests
{
    public class AudioThrottlerTests
    {
        private const int SAMPLE_RATE = 48_000;
        private const int CHANNELS = 2;
        private const int FACTOR_SHORT_TO_BYTES = 4;
        private const int AVERAGE_BYTES_PER_SECOND = SAMPLE_RATE * CHANNELS * FACTOR_SHORT_TO_BYTES;
        private const int BUFFER_LENGTH = AudioThrottler.BUFFER_SIZE_IN_SECONDS * AVERAGE_BYTES_PER_SECOND;
        
        private WaveFormat _defaultWaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(SAMPLE_RATE, CHANNELS);

        private Mock<IMainAudioSession> _audioSessionMock;
        private Mock<IAudioLoopbackCapture> _waveInMock;
        private Mock<IAudioWaveOut> _silencerMock;
        private CancellationTokenSource _token;

        public AudioThrottlerTests()
        {
            _audioSessionMock = new Mock<IMainAudioSession>();
            _audioSessionMock.Setup(x => x.IsAudioEndpointDeviceActive).Returns(true);

            _waveInMock = new Mock<IAudioLoopbackCapture>();
            _waveInMock.Setup(x => x.WaveFormat).Returns(_defaultWaveFormat);
            _waveInMock.Setup(x => x.CaptureState).Returns(NAudio.CoreAudioApi.CaptureState.Capturing);

            _silencerMock = new Mock<IAudioWaveOut>();

            _token = new CancellationTokenSource();
        }

        [Fact]
        internal void WaveFormat_CanSet()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);

            Assert.Equal(_defaultWaveFormat, audioThrottler.WaveFormat);
        }

        [Fact]
        internal async void Run_StartStopDispose()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);

            Random r = new Random();
            var buffer = new byte[BUFFER_LENGTH];
            r.NextBytes(buffer);

            await RunTest(audioThrottler, async () =>
            {
                await Task.CompletedTask;
            });

            audioThrottler.Dispose();

            _silencerMock.Verify(x => x.Play());
            _waveInMock.Verify(x => x.StartRecording());

            _silencerMock.Verify(x => x.Stop());
            _waveInMock.Verify(x => x.StopRecording());
        }

        [Fact]
        internal async void Run_StartDispose()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);

            Random r = new Random();
            var buffer = new byte[BUFFER_LENGTH];
            r.NextBytes(buffer);

            await StartTest(audioThrottler);
            audioThrottler.Dispose();
            await Task.Delay(200);

            _silencerMock.Verify(x => x.Play());
            _waveInMock.Verify(x => x.StartRecording());

            _silencerMock.Verify(x => x.Stop());
            _waveInMock.Verify(x => x.StopRecording());
        }

        [Fact]
        internal void Worker_AddRemove()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);

            var id = Guid.NewGuid();
            audioThrottler.AddWorker(id);
            var workerInitialPosition = audioThrottler.GetWorkerPosition(id);

            audioThrottler.RemoveWorker(id);

            Assert.Equal(0, workerInitialPosition);
            Assert.Null(audioThrottler.GetWorkerPosition(id));
        }

        [Fact]
        internal async void Worker_CanReadIfBufferReady()
        {
            var batch = BUFFER_LENGTH / 4;
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);

            var id = Guid.NewGuid();
            audioThrottler.AddWorker(id);

            await RunTest(audioThrottler, async () =>
            {
                RaiseEvent(batch);

                Assert.Equal(0, (await audioThrottler.GetData(id)).BytesRecordedCount);

                RaiseEvent(batch);

                Assert.Equal(AVERAGE_BYTES_PER_SECOND, (await audioThrottler.GetData(id)).BytesRecordedCount);
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(AVERAGE_BYTES_PER_SECOND / 2)]
        [InlineData(AVERAGE_BYTES_PER_SECOND)]
        // test if it skips reading if we have less data than the WaveAverageBytesPerSecond (threshold)
        internal async void Worker_AddThenReadNothing(int dataAdded)
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);

            await RunTest(audioThrottler, async () =>
            {
                var id = Guid.NewGuid();
                audioThrottler.AddWorker(id);

                RaiseEvent(dataAdded);

                var data = await audioThrottler.GetData(id);

                Assert.Equal(0, data.BytesRecordedCount);
                Assert.Equal(0, audioThrottler.GetWorkerPosition(id));
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(AVERAGE_BYTES_PER_SECOND / 2)]
        [InlineData(AVERAGE_BYTES_PER_SECOND)]
        // test if it skips reading if we have less data than the WaveAverageBytesPerSecond (threshold)
        internal async void Worker_AddWaitThenReadNothing(int dataAdded)
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);
            var workerInitialPosition = AVERAGE_BYTES_PER_SECOND * 2;

            await RunTest(audioThrottler, async () =>
            {
                RaiseEvent(workerInitialPosition);

                var id = Guid.NewGuid();
                audioThrottler.AddWorker(id);

                RaiseEvent(dataAdded);

                var data = await audioThrottler.GetData(id);

                Assert.Equal(0, data.BytesRecordedCount);
                Assert.Equal(workerInitialPosition, audioThrottler.GetWorkerPosition(id));
            });
        }

        [Fact]
        // test if it reads as soon as we get more data than the WaveAverageBytesPerSecond (threshold)
        internal async void Worker_AddThenReadSome()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);

            await RunTest(audioThrottler, async () =>
            {
                var id = Guid.NewGuid();
                audioThrottler.AddWorker(id);

                // it read as soon as it reached WaveAverageBytesPerSecond value;
                RaiseEvent(AVERAGE_BYTES_PER_SECOND + 1);

                var data = await audioThrottler.GetData(id);

                Assert.Equal(AVERAGE_BYTES_PER_SECOND, data.BytesRecordedCount);
                Assert.Equal(AVERAGE_BYTES_PER_SECOND, audioThrottler.GetWorkerPosition(id));
            });
        }

        [Fact]
        // test if it reads as soon as we get more data than the WaveAverageBytesPerSecond (threshold)
        internal async void Worker_AddWaitThenReadSome()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);

            await RunTest(audioThrottler, async () =>
            {
                var initialPosition = AVERAGE_BYTES_PER_SECOND * 2;
                RaiseEvent(initialPosition);

                var id = Guid.NewGuid();
                audioThrottler.AddWorker(id);

                // it read as soon as it reached WaveAverageBytesPerSecond value;
                RaiseEvent(AVERAGE_BYTES_PER_SECOND + 1);

                var data = await audioThrottler.GetData(id);

                Assert.Equal(AVERAGE_BYTES_PER_SECOND, data.BytesRecordedCount);
                Assert.Equal(initialPosition + AVERAGE_BYTES_PER_SECOND, audioThrottler.GetWorkerPosition(id));
            });
        }

        [Fact]
        internal async void Worker_AddThenFullyRead()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);

            var id = Guid.NewGuid();
            audioThrottler.AddWorker(id);

            // read until end reached
            await RunTest(audioThrottler, async () =>
            {
                Assert.Equal(0, (await audioThrottler.GetData(id)).BytesRecordedCount);

                RaiseEvent(BUFFER_LENGTH);

                var data = await audioThrottler.GetData(id);

                Assert.Equal(BUFFER_LENGTH, data.Buffer.Length);
                Assert.Equal(AVERAGE_BYTES_PER_SECOND, data.BytesRecordedCount);
                Assert.Equal(AVERAGE_BYTES_PER_SECOND, (await audioThrottler.GetData(id)).BytesRecordedCount);
                Assert.Equal(AVERAGE_BYTES_PER_SECOND, (await audioThrottler.GetData(id)).BytesRecordedCount);
                Assert.Equal(0, (await audioThrottler.GetData(id)).BytesRecordedCount);
            });
          
        }

        [Fact]
        internal async void Worker_AddThenFullyReadTwice()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);

            await RunTest(audioThrottler, async () =>
            {
                RaiseEvent(BUFFER_LENGTH);

                var id = Guid.NewGuid();
                audioThrottler.AddWorker(id);

                Assert.Equal(0, (await audioThrottler.GetData(id)).BytesRecordedCount);

                RaiseEvent(BUFFER_LENGTH / 2);

                Assert.Equal(AVERAGE_BYTES_PER_SECOND, (await audioThrottler.GetData(id)).BytesRecordedCount);
                Assert.Equal(0, (await audioThrottler.GetData(id)).BytesRecordedCount);

                RaiseEvent(BUFFER_LENGTH / 2);

                Assert.Equal(AVERAGE_BYTES_PER_SECOND, (await audioThrottler.GetData(id)).BytesRecordedCount);
                Assert.Equal(AVERAGE_BYTES_PER_SECOND, (await audioThrottler.GetData(id)).BytesRecordedCount);
                Assert.Equal(0, (await audioThrottler.GetData(id)).BytesRecordedCount);
            });
        }


        [Fact]
        internal async void Worker_MovePositionOnceDataRead()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);

            var id = Guid.NewGuid();
            audioThrottler.AddWorker(id);
            var workerInitialPosition = audioThrottler.GetWorkerPosition(id);

            await RunTest(audioThrottler, async () =>
            {
                RaiseEvent(BUFFER_LENGTH);
                var workerPositionAfterDataAdded = audioThrottler.GetWorkerPosition(id);

                await audioThrottler.GetData(id);

                var workerPositionAfterDataReadOneSecond = audioThrottler.GetWorkerPosition(id);

                await audioThrottler.GetData(id);

                var workerPositionAfterDataReadTwoSeconds = audioThrottler.GetWorkerPosition(id);

                await audioThrottler.GetData(id);

                var workerPositionAfterDataReadThreeSeconds = audioThrottler.GetWorkerPosition(id);

                await audioThrottler.GetData(id); // no more data available

                // four seconds: no more data read; position did not move
                var workerPositionAfterDataReadFourSeconds = audioThrottler.GetWorkerPosition(id);

                Assert.Equal(0, workerInitialPosition.Value);
                Assert.Equal(0, workerPositionAfterDataAdded);
                Assert.Equal(AVERAGE_BYTES_PER_SECOND, workerPositionAfterDataReadOneSecond.Value);
                Assert.Equal(AVERAGE_BYTES_PER_SECOND*2, workerPositionAfterDataReadTwoSeconds.Value);
                Assert.Equal(AVERAGE_BYTES_PER_SECOND*3, workerPositionAfterDataReadThreeSeconds.Value);
                // three seconds = four seconds
                Assert.Equal(workerPositionAfterDataReadThreeSeconds.Value, workerPositionAfterDataReadFourSeconds.Value);
            });
        }

        [Fact]
        internal async void MultipleWorkers_BothRead()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);
           
            var firstWorkerId = Guid.NewGuid();
            audioThrottler.AddWorker(firstWorkerId);

            await RunTest(audioThrottler, async () =>
            {
                RaiseEvent(AVERAGE_BYTES_PER_SECOND);

                var secondWorkerId = Guid.NewGuid();
                audioThrottler.AddWorker(secondWorkerId);

                // none read; none has enough data
                Assert.Equal(0, (await audioThrottler.GetData(firstWorkerId)).BytesRecordedCount);
                Assert.Equal(0, (await audioThrottler.GetData(secondWorkerId)).BytesRecordedCount);

                RaiseEvent(AVERAGE_BYTES_PER_SECOND);

                // only first read; it has enough data
                Assert.Equal(AVERAGE_BYTES_PER_SECOND, (await audioThrottler.GetData(firstWorkerId)).BytesRecordedCount);
                Assert.Equal(0, (await audioThrottler.GetData(secondWorkerId)).BytesRecordedCount);

                RaiseEvent(AVERAGE_BYTES_PER_SECOND);

                // both read
                Assert.Equal(AVERAGE_BYTES_PER_SECOND, (await audioThrottler.GetData(firstWorkerId)).BytesRecordedCount);
                Assert.Equal(AVERAGE_BYTES_PER_SECOND, (await audioThrottler.GetData(secondWorkerId)).BytesRecordedCount);

                RaiseEvent(AVERAGE_BYTES_PER_SECOND);
                audioThrottler.RemoveWorker(firstWorkerId);

                // only last read; first got removed
                Assert.Equal(0, (await audioThrottler.GetData(firstWorkerId)).BytesRecordedCount);
                Assert.Equal(AVERAGE_BYTES_PER_SECOND, (await audioThrottler.GetData(secondWorkerId)).BytesRecordedCount);

                RaiseEvent(AVERAGE_BYTES_PER_SECOND);
                audioThrottler.RemoveWorker(secondWorkerId);

                // none read; all removed
                Assert.Equal(0, (await audioThrottler.GetData(firstWorkerId)).BytesRecordedCount);
                Assert.Equal(0, (await audioThrottler.GetData(secondWorkerId)).BytesRecordedCount);
            });
        }

        [Fact]
        internal async void WaitForBufferReady_Falsy()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);

            await RunTest(audioThrottler, async () =>
            {
                var id = Guid.NewGuid();
                audioThrottler.AddWorker(id);

                var result = await audioThrottler.WaitForWorkerPositionReady(id, 2_000);

                Assert.False(result);
            });
        }

        [Fact]
        internal async void WaitForBufferReady_Truthy()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);

            await RunTest(audioThrottler, async () =>
            {

                var id = Guid.NewGuid();
                audioThrottler.AddWorker(id);

                RaiseEvent();

                var result = await audioThrottler.WaitForWorkerPositionReady(id, 2_000);

                Assert.True(result);
            });
        }

        private async Task StartTest(AudioThrottler audioThrottler)
        {
            audioThrottler.Running = true;
            Task.Run(() => audioThrottler.Run(_token));
            await Task.Delay(50);
        }

        private async Task StopTest(AudioThrottler audioThrottler)
        {
            audioThrottler.Running = false;
            await Task.Delay(50);
            audioThrottler.Dispose();
        }

        private async Task RunTest(AudioThrottler audioThrottler, Func<Task> action)
        {
            await StartTest(audioThrottler);
            await action();
            await StopTest(audioThrottler);
        }

        private void RaiseEvent(int length = BUFFER_LENGTH)
        {
            Random r = new Random();
            var buffer = new byte[length];
            r.NextBytes(buffer);

            _waveInMock.Raise(x => x.DataAvailable += null, new WaveInEventArgs(buffer, buffer.Length));
        }

    }
}
