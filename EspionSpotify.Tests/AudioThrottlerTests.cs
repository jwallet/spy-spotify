using EspionSpotify.AudioSessions;
using EspionSpotify.AudioSessions.Audio;
using Moq;
using NAudio.Wave;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.Swan;
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
        internal void Worker_AddStopRemove()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);

            var id = Guid.NewGuid();
            audioThrottler.AddWorker(id);
            var workerInitialPosition = audioThrottler.GetWorkerPosition(id);

            audioThrottler.StopWorker(id);
            var workerStopPositionExist = audioThrottler.StopWorkerExist(id);

            audioThrottler.RemoveWorker(id);

            Assert.Equal(0, workerInitialPosition);
            Assert.True(workerStopPositionExist);
            Assert.Null(audioThrottler.GetWorkerPosition(id));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(AVERAGE_BYTES_PER_SECOND / 2)]
        [InlineData(AVERAGE_BYTES_PER_SECOND)]
        [InlineData(AVERAGE_BYTES_PER_SECOND * 2)]
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
        [InlineData(AVERAGE_BYTES_PER_SECOND * 2)]
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
                RaiseEvent(AVERAGE_BYTES_PER_SECOND);
                RaiseEvent(AVERAGE_BYTES_PER_SECOND);
                RaiseEvent(AVERAGE_BYTES_PER_SECOND); // needed offset
                RaiseEvent(AVERAGE_BYTES_PER_SECOND);

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
                RaiseEvent(AVERAGE_BYTES_PER_SECOND);
                RaiseEvent(AVERAGE_BYTES_PER_SECOND);
                RaiseEvent(AVERAGE_BYTES_PER_SECOND); // needed offset
                RaiseEvent(AVERAGE_BYTES_PER_SECOND);

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
                // read only once, buffer is x4 the WaveAverageBytesPerSecond, and the needed offset is x3, so x1 available to read
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

                RaiseEvent(AVERAGE_BYTES_PER_SECOND);
                RaiseEvent(AVERAGE_BYTES_PER_SECOND);
                RaiseEvent(AVERAGE_BYTES_PER_SECOND); // needed offset (x3)
                RaiseEvent(AVERAGE_BYTES_PER_SECOND); // will start to read first event afer this event

                Assert.Equal(AVERAGE_BYTES_PER_SECOND, (await audioThrottler.GetData(id)).BytesRecordedCount);
                Assert.Equal(0, (await audioThrottler.GetData(id)).BytesRecordedCount);

                RaiseEvent(AVERAGE_BYTES_PER_SECOND);

                Assert.Equal(AVERAGE_BYTES_PER_SECOND, (await audioThrottler.GetData(id)).BytesRecordedCount);
                Assert.Equal(0, (await audioThrottler.GetData(id)).BytesRecordedCount);
                // there is still x2 AverageBytesPerSecond to read: offset
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

                RaiseEvent(AVERAGE_BYTES_PER_SECOND);

                await audioThrottler.GetData(id);

                var workerPositionAfterDataReadTwoSeconds = audioThrottler.GetWorkerPosition(id);

                await audioThrottler.GetData(id); // no more data available

                // four seconds: no more data read; position did not move
                var workerPositionAfterDataReadThreeSeconds = audioThrottler.GetWorkerPosition(id);

                Assert.Equal(0, workerInitialPosition.Value);
                Assert.Equal(0, workerPositionAfterDataAdded);
                Assert.Equal(AVERAGE_BYTES_PER_SECOND, workerPositionAfterDataReadOneSecond.Value);
                Assert.Equal(AVERAGE_BYTES_PER_SECOND*2, workerPositionAfterDataReadTwoSeconds.Value);
                // three seconds = two seconds
                Assert.Equal(workerPositionAfterDataReadTwoSeconds.Value, workerPositionAfterDataReadThreeSeconds.Value);
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
                RaiseEvent(AVERAGE_BYTES_PER_SECOND); // pre offset x2

                var secondWorkerId = Guid.NewGuid();
                audioThrottler.AddWorker(secondWorkerId);

                RaiseEvent(AVERAGE_BYTES_PER_SECOND); // needed offset reached by first; second only has 1/3

                // none read; none has enough data
                Assert.Equal(0, (await audioThrottler.GetData(firstWorkerId)).BytesRecordedCount);
                Assert.Equal(0, (await audioThrottler.GetData(secondWorkerId)).BytesRecordedCount);

                RaiseEvent(AVERAGE_BYTES_PER_SECOND);

                // only first read; it has enough data
                Assert.Equal(AVERAGE_BYTES_PER_SECOND, (await audioThrottler.GetData(firstWorkerId)).BytesRecordedCount);
                Assert.Equal(0, (await audioThrottler.GetData(secondWorkerId)).BytesRecordedCount);

                RaiseEvent(AVERAGE_BYTES_PER_SECOND);

                // only first read; second has not enough data yet
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
        internal async void Worker_ReadStartDefault()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);
            await RunTest(audioThrottler, async () =>
            {
                var id = Guid.NewGuid();
                audioThrottler.AddWorker(id);
                RaiseEvent(AVERAGE_BYTES_PER_SECOND);
                RaiseEvent(AVERAGE_BYTES_PER_SECOND);
                RaiseEvent(AVERAGE_BYTES_PER_SECOND); // needed offset
                RaiseEvent(AVERAGE_BYTES_PER_SECOND); // will start to read first event afer this event
                var dataStart = await audioThrottler.GetDataStart(id, detectSilence: false);
                Assert.Equal(AVERAGE_BYTES_PER_SECOND, dataStart.BytesRecordedCount);
            });
        }

        [Fact]
        internal async void Worker_ReadStartSilenceBefore()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);
            await RunTest(audioThrottler, async () =>
            {
                var silence = AVERAGE_BYTES_PER_SECOND / 8; // silence of 125 milliseconds: 48_000
                var delayBetweenSilenceAndWorkerAdded = AVERAGE_BYTES_PER_SECOND / 4;  // audio of 250 milliseconds: 96_000
                var id = Guid.NewGuid();
                RaiseEvent(AVERAGE_BYTES_PER_SECOND); // 384_000
                RaiseSilenceEvent(silence);
                RaiseEvent(delayBetweenSilenceAndWorkerAdded); // should be included and up to silence
                audioThrottler.AddWorker(id);
                var workerInitialPosition = audioThrottler.GetWorkerPosition(id); // pos: 528_000
                RaiseEvent(AVERAGE_BYTES_PER_SECOND * 2); // needed offset

                var dataStart = await audioThrottler.GetDataStart(id, detectSilence: true);
                var workerFinalPosition = audioThrottler.GetWorkerPosition(id); // pos: 528_000
                var bytesRecorded = dataStart.BytesRecordedCount;

                var expectedWorkerInitialPosition = AVERAGE_BYTES_PER_SECOND + silence + delayBetweenSilenceAndWorkerAdded;
                Assert.Equal(expectedWorkerInitialPosition, workerInitialPosition);
                var expectedBytesRecorded = (silence /2) + delayBetweenSilenceAndWorkerAdded;
                Assert.Equal(expectedBytesRecorded, bytesRecorded);
                // both positions are equal; data got = only to silence offset, so we go back to initial position
                Assert.Equal(workerInitialPosition, workerFinalPosition);
            });
        }

        [Fact]
        internal async void Worker_ReadStartSilenceAfter()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);
            await RunTest(audioThrottler, async () =>
            {
                var silence = AVERAGE_BYTES_PER_SECOND / 8; // silence of 125 milliseconds: 48_000
                var delayBetweenSilenceAndWorkerAdded = AVERAGE_BYTES_PER_SECOND / 4;  // audio of 250 milliseconds: 96_000
                var id = Guid.NewGuid();
                RaiseEvent(AVERAGE_BYTES_PER_SECOND); // 384_000
                audioThrottler.AddWorker(id);
                var workerInitialPosition = audioThrottler.GetWorkerPosition(id); // pos: 384_000
                RaiseEvent(delayBetweenSilenceAndWorkerAdded); // should be ignored, start at silence and down
                RaiseSilenceEvent(silence);
                RaiseEvent(AVERAGE_BYTES_PER_SECOND * 2); // needed offset

                var dataStart = await audioThrottler.GetDataStart(id, detectSilence: true);
                var workerFinalPosition = audioThrottler.GetWorkerPosition(id); // pos: 528_000
                var bytesRecorded = dataStart.BytesRecordedCount;

                var expectedWorkerInitialPosition = AVERAGE_BYTES_PER_SECOND;
                Assert.Equal(expectedWorkerInitialPosition, workerInitialPosition);
                var expectedBytesRecorded = (silence / 2);
                Assert.Equal(expectedBytesRecorded, bytesRecorded);
                Assert.Equal(workerInitialPosition + expectedBytesRecorded, workerFinalPosition);
            });
        }

        [Fact]
        internal async void Worker_ReadEndDefault()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);
            await RunTest(audioThrottler, async () =>
            {
                var id = Guid.NewGuid();
                audioThrottler.AddWorker(id);
                RaiseEvent(AVERAGE_BYTES_PER_SECOND);
                RaiseEvent(AVERAGE_BYTES_PER_SECOND);
                audioThrottler.StopWorker(id);
                RaiseEvent(AVERAGE_BYTES_PER_SECOND);
                var dataEnd = await audioThrottler.GetDataEnd(id, detectSilence: false);
                Assert.Equal(AVERAGE_BYTES_PER_SECOND * 2, dataEnd.BytesRecordedCount);
            });
        }

        [Fact]
        internal async void Worker_ReadEndSilenceBefore()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);
            await RunTest(audioThrottler, async () =>
            {
                var silence = AVERAGE_BYTES_PER_SECOND / 8; // silence of 125 milliseconds: 48_000
                var delayBetweenSilenceAndWorkerAdded = AVERAGE_BYTES_PER_SECOND / 4;  // audio of 250 milliseconds: 96_000
                var id = Guid.NewGuid();
                RaiseEvent(AVERAGE_BYTES_PER_SECOND); // 384_000
                audioThrottler.AddWorker(id);
                var workerInitialPosition = audioThrottler.GetWorkerPosition(id); // pos: 384_000
                RaiseEvent(AVERAGE_BYTES_PER_SECOND);
                RaiseSilenceEvent(silence); 
                RaiseEvent(delayBetweenSilenceAndWorkerAdded); // should be ignored; get silence and up
                audioThrottler.StopWorker(id);
                RaiseEvent(AVERAGE_BYTES_PER_SECOND);

                var dataStart = await audioThrottler.GetDataEnd(id, detectSilence: true);
                var workerFinalPosition = audioThrottler.GetWorkerPosition(id);
                var bytesRecorded = dataStart.BytesRecordedCount;

                var expectedWorkerInitialPosition = AVERAGE_BYTES_PER_SECOND;
                Assert.Equal(expectedWorkerInitialPosition, workerInitialPosition);
                var expectedBytesRecorded = AVERAGE_BYTES_PER_SECOND + (silence / 2);
                Assert.Equal(expectedBytesRecorded, bytesRecorded);
                var expectedWorkerFinalPosition = workerInitialPosition + bytesRecorded;
                Assert.Equal(expectedWorkerFinalPosition, workerFinalPosition);
            });
        }

        [Fact]
        internal async void Worker_ReadEndSilenceAfter()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);
            await RunTest(audioThrottler, async () =>
            {
                var silence = AVERAGE_BYTES_PER_SECOND / 8; // silence of 125 milliseconds: 48_000
                var delayBetweenSilenceAndWorkerAdded = AVERAGE_BYTES_PER_SECOND / 4;  // audio of 250 milliseconds: 96_000
                var id = Guid.NewGuid();
                RaiseEvent(AVERAGE_BYTES_PER_SECOND); // 384_000
                audioThrottler.AddWorker(id);
                var workerInitialPosition = audioThrottler.GetWorkerPosition(id); // pos: 384_000
                RaiseEvent(AVERAGE_BYTES_PER_SECOND);
                audioThrottler.StopWorker(id);
                RaiseEvent(delayBetweenSilenceAndWorkerAdded); 
                RaiseSilenceEvent(silence);
                RaiseEvent(AVERAGE_BYTES_PER_SECOND); // should be ignored; get silence and up

                var dataStart = await audioThrottler.GetDataEnd(id, detectSilence: true);
                var workerFinalPosition = audioThrottler.GetWorkerPosition(id);
                var bytesRecorded = dataStart.BytesRecordedCount;

                var expectedWorkerInitialPosition = AVERAGE_BYTES_PER_SECOND;
                Assert.Equal(expectedWorkerInitialPosition, workerInitialPosition);
                var expectedBytesRecorded = AVERAGE_BYTES_PER_SECOND + delayBetweenSilenceAndWorkerAdded + (silence / 2);
                Assert.Equal(expectedBytesRecorded, bytesRecorded);
                var expectedWorkerFinalPosition = workerInitialPosition + bytesRecorded;
                Assert.Equal(expectedWorkerFinalPosition, workerFinalPosition);
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

                var result = await audioThrottler.WaitForWorkerReadPositionReadiness(id, 2_000);

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

                var result = await audioThrottler.WaitForWorkerReadPositionReadiness(id, 2_000);

                Assert.True(result);
            });
        }

        private async Task StartTest(AudioThrottler audioThrottler)
        {
            audioThrottler.Running = true;
            _ = Task.Run(async () => await audioThrottler.Run(_token));
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
            var buffer = GenerateSound(_defaultWaveFormat, length);

            _waveInMock.Raise(x => x.DataAvailable += null, new WaveInEventArgs(buffer, buffer.Length));
        }

        private void RaiseSilenceEvent(int length = AVERAGE_BYTES_PER_SECOND)
        {
            var buffer = GenerateSilence(_defaultWaveFormat, length);
            _waveInMock.Raise(x => x.DataAvailable += null, new WaveInEventArgs(buffer, buffer.Length));
        }

        private byte[] GenerateSilenceBuffer(int length)
        {
            var buffer = new byte[length];
            return buffer;
        }


        [Fact]
        internal void TestSilenceBuffer()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);
            var buffer = GenerateSilence(audioThrottler.WaveFormat, audioThrottler.WaveFormat.AverageBytesPerSecond);

            var actual = audioThrottler.DetectSilencePosition(buffer, 0, buffer.Length);

            Assert.Equal(AVERAGE_BYTES_PER_SECOND / 2, actual);
        }

        [Fact]
        internal void TestSoundBuffer()
        {
            var audioThrottler = new AudioThrottler(_audioSessionMock.Object, _waveInMock.Object, _silencerMock.Object);
            var buffer = GenerateSound(audioThrottler.WaveFormat, audioThrottler.WaveFormat.AverageBytesPerSecond);

            var actual = audioThrottler.DetectSilencePosition(buffer, 0, buffer.Length);

            Assert.Equal(AVERAGE_BYTES_PER_SECOND / 2, actual);
        }

        private byte[] GenerateRandomBytes(WaveFormat waveFormat, int length)
        {
            var bytesPerSample = waveFormat.BitsPerSample / 8;
            var byteCount = length / bytesPerSample * bytesPerSample;

            var random = new Random();
            var buffer = new byte[byteCount];

            for (var i = 0; i < byteCount; i += bytesPerSample)
            {
                float sampleValue;

                // Generate random 32-bit sample value
                var lo = (long)random.Next(int.MinValue, int.MaxValue);
                var hi = (long)random.Next(int.MinValue, int.MaxValue);
                sampleValue = BitConverter.ToSingle(BitConverter.GetBytes(lo), 0)
                    + BitConverter.ToSingle(BitConverter.GetBytes(hi), 0);

                // Convert sample value to byte data
                var sampleBytes = BitConverter.GetBytes(sampleValue);

                // Write sample data to buffer
                for (var j = 0; j < bytesPerSample; j++)
                {
                    buffer[i + j] = sampleBytes[j];
                }
            }

            return buffer;
        }

        public byte[] GenerateSound(WaveFormat waveFormat, int lengthInBytes)
        {
            var r = new Random();
            var bytes = new byte[lengthInBytes];
            for (int i = 0; i < lengthInBytes; i+=4)
            {
                var f = r.Next(-4, -1) / 10f;
                var b = f.ToBytes();
                bytes[i] = b[0];
                bytes[i + 1] = b[1];
                bytes[i + 2] = b[2];
                bytes[i + 3] = b[3];
            }
            return bytes;
        }


        public byte[] GenerateSilence(WaveFormat waveFormat, int lengthInBytes)
        {
            var r = new Random();
            var bytes = new byte[lengthInBytes];
            for (int i = 0; i < lengthInBytes; i += 4)
            {
                var f = r.Next(-9, 9) / 100f;
                var b = f.ToBytes();
                bytes[i] = b[0];
                bytes[i + 1] = b[1];
                bytes[i + 2] = b[2];
                bytes[i + 3] = b[3];
            }
            return bytes;
        }

    }
}
