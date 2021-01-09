using EspionSpotify.Enums;
using EspionSpotify.Properties;

namespace EspionSpotify.Extensions
{
    internal static class SettingExtensions
    {
        internal static RecordRecordingsStatus GetRecordRecordingsStatus(this Settings settings)
        {
            if (settings.advanced_record_over_recordings_enabled)
            {
                if (settings.advanced_record_over_recordings_and_duplicate_enabled)
                {
                    return RecordRecordingsStatus.Duplicate;
                }
                return RecordRecordingsStatus.Overwrite;
            }
            return RecordRecordingsStatus.Skip;
        }
    }
}
