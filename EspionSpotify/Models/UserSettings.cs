using EspionSpotify.AudioSessions;
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
        public bool RecordUnknownTrackTypeEnabled { get; set; }
        public ISpotifyAudioSession SpotifyAudioSession { get; set; }
        public int? InternalOrderNumber { get; set; }
        public bool DuplicateAlreadyRecordedTrack { get; set; }
        public int? AudioEndPointDeviceIndex { get; set; }
        public string RecordingTimer { get; set; }

        public bool HasRecordingTimerEnabled
        {
            get => RecordingTimer != null && RecordingTimer != "000000";
        }

        public double RecordingTimerMilliseconds
        {
            get => new TimeSpan(int.Parse(RecordingTimer.Substring(0, 2)), int.Parse(RecordingTimer.Substring(2, 2)), int.Parse(RecordingTimer.Substring(4, 2))).TotalMilliseconds;
        }

        public int? OrderNumber
        {
            get => OrderNumberInfrontOfFileEnabled || OrderNumberInMediaTagEnabled ? InternalOrderNumber : null;
        }
    }
}
