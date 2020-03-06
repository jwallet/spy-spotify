using Xunit;
using EspionSpotify.Models;

namespace EspionSpotify.Tests
{
    public class UserSettingTests
    {
        [Fact]
        internal void DefaultUserSettings_ReturnsDefault()
        {
            var userSettings = new UserSettings();

            Assert.Equal(1, userSettings.InternalOrderNumber);
            Assert.False(userSettings.HasRecordingTimerEnabled);
            Assert.Equal(0.0, userSettings.RecordingTimerMilliseconds);
            Assert.Null(userSettings.OrderNumber);
            Assert.False(userSettings.IsSpotifyAPISet);
        }

        [Fact]
        internal void RecordingTimer_ReturnsRecordingTimerEnabled()
        {
            Assert.False(new UserSettings { RecordingTimer = "" }.HasRecordingTimerEnabled);
            Assert.False(new UserSettings { RecordingTimer = "1337" }.HasRecordingTimerEnabled);
            Assert.False(new UserSettings { RecordingTimer = "000000" }.HasRecordingTimerEnabled);

            Assert.True(new UserSettings { RecordingTimer = "001337" }.HasRecordingTimerEnabled);
        }

        [Fact]
        internal void RecordingTimer_ReturnsRecordingTimerMilliseconds()
        {
            Assert.Equal(0, new UserSettings { RecordingTimer = "000000" }.RecordingTimerMilliseconds);
            Assert.Equal(1000, new UserSettings { RecordingTimer = "000001" }.RecordingTimerMilliseconds);
            Assert.Equal(817000, new UserSettings { RecordingTimer = "001337" }.RecordingTimerMilliseconds);
            Assert.Equal(43200000, new UserSettings { RecordingTimer = "120000" }.RecordingTimerMilliseconds);
            // invalid value to passed
            Assert.Equal(362439000, new UserSettings { RecordingTimer = "999999" }.RecordingTimerMilliseconds);
        }

        [Fact]
        internal void InternalOrderNumber_ReturnsOrderNumberValue()
        {
            Assert.Equal(1, new UserSettings { OrderNumberInfrontOfFileEnabled = true }.OrderNumber);
            Assert.Equal(1, new UserSettings { OrderNumberInMediaTagEnabled = true }.OrderNumber);
        }

        [Fact]
        internal void SpotifyAPISecrets_ReturnsIsSpotifyAPISet()
        {
            Assert.False(new UserSettings { SpotifyAPIClientId = "", SpotifyAPISecretId = "" }.IsSpotifyAPISet);
            Assert.False(new UserSettings { SpotifyAPIClientId = "abc123", SpotifyAPISecretId = "" }.IsSpotifyAPISet);
            Assert.False(new UserSettings { SpotifyAPIClientId = "", SpotifyAPISecretId = "abc123" }.IsSpotifyAPISet);
            Assert.True(new UserSettings { SpotifyAPIClientId = "abc123", SpotifyAPISecretId = "abc123" }.IsSpotifyAPISet);
        }
    }
}
