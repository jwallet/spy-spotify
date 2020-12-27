using EspionSpotify.Enums;
using EspionSpotify.Properties;

namespace EspionSpotify.Extensions
{
    internal static class SettingExtensions
    {
        internal static RecordRecordingsStatus GetRecordRecordingsStatus(this Settings settings)
        {
            if (settings.RecordOverRecordingsEnabled)
            {
                if (settings.RecordDuplicateRecordingsEnabled)
                {
                    return RecordRecordingsStatus.Duplicate;
                }
                return RecordRecordingsStatus.Overwrite;
            }
            return RecordRecordingsStatus.Skip;
        }
    }
}
