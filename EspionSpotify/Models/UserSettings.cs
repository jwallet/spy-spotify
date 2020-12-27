using EspionSpotify.Enums;
using NAudio.Lame;
using System;

namespace EspionSpotify.Models
{
    public class UserSettings
    {
        public string OutputPath { get; set; }
        public LAMEPreset Bitrate { get; set; }
        public MediaFormat MediaFormat { get; set; }
        public int MinimumRecordedLengthSeconds { get; set; }
        public bool GroupByFoldersEnabled { get; set; }
        public string TrackTitleSeparator { get; set; }
        public bool OrderNumberInMediaTagEnabled { get; set; }
        public bool OrderNumberInfrontOfFileEnabled { get; set; }
        public bool EndingTrackDelayEnabled { get; set; }
        public bool MuteAdsEnabled { get; set; }
        public bool RecordEverythingEnabled { get; set; }
        public bool RecordAdsEnabled { get; set; }
        public bool ExtraTitleToSubtitleEnabled { get; set; }
        public int InternalOrderNumber { get; set; } = 1;
        public RecordRecordingsStatus RecordRecordingsStatus { get; set; }
        public string AudioEndPointDeviceID { get; set; }
        public string RecordingTimer { get; set; }
        public string SpotifyAPIClientId { get; set; }
        public string SpotifyAPISecretId { get; set; }
        public string OrderNumberMask { get; set; } = "000";
        public int OrderNumberMax
        {
            get
            {
                return Convert.ToInt32(OrderNumberMask.Replace('0', '9').ToString());
            }
        }

        public bool HasRecordingTimerEnabled
        {
            get => !string.IsNullOrEmpty(RecordingTimer) && RecordingTimer.Length == 6 && RecordingTimer != "000000";
        }

        public double RecordingTimerMilliseconds
        {
            get => HasRecordingTimerEnabled
                ? new TimeSpan(
                    int.Parse(RecordingTimer.Substring(0, 2)),
                    int.Parse(RecordingTimer.Substring(2, 2)),
                    int.Parse(RecordingTimer.Substring(4, 2))).TotalMilliseconds
                : 0.0;
        }

        public bool HasOrderNumberEnabled
        {
            get => OrderNumberInfrontOfFileEnabled || OrderNumberInMediaTagEnabled;
        }

        public int? OrderNumberAsTag
        {
            get => OrderNumberInMediaTagEnabled ? (int?)InternalOrderNumber : null;
        }

        public int? OrderNumberAsFile
        {
            get => OrderNumberInfrontOfFileEnabled ? (int?)Math.Min(InternalOrderNumber, OrderNumberMax) : null;
        }

        public bool IsSpotifyAPISet
        {
            get => !string.IsNullOrWhiteSpace(SpotifyAPIClientId) && !string.IsNullOrWhiteSpace(SpotifyAPISecretId);
        }
    }
}
