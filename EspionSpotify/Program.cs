using EspionSpotify.Properties;
using ExceptionReporting;
using NAudio.Lame;
using System;
using System.Threading;
using System.Windows.Forms;

namespace EspionSpotify
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            // Add the event handler for handling UI thread exceptions to the event.
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

            // Set the unhandled exception mode to force all Windows Forms errors to go through our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event. 
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmEspionSpotify());
        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether
        // or not they wish to abort execution.
        internal static void Application_ThreadException(object sender, ThreadExceptionEventArgs t)
        {
            ReportException(t.Exception);
        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether
        // or not they wish to abort execution.
        // NOTE: This exception cannot be kept from terminating the application - it can only 
        // log the event, and inform the user about it. 
        internal static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var thread = new Thread(() =>
            {
                ReportException((Exception)e.ExceptionObject);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
            thread.Join();
        }

        // Creates the error message and displays it.
        internal static void ReportException(Exception ex)
        {
            string template = null;
            try
            {
                template = @"# {{App.Title}}

**Version**:     {{App.Version}}  
**Region**:      {{App.Region}}  
{{#if App.User}}
**User**:        {{App.User}}  
{{/if}}    
**Date**: {{Error.Date}}  
**Time**: {{Error.Time}}  
{{#if Error.Explanation}}
**User Explanation**: {{Error.Explanation}}  
{{/if}}

**Error Message**: {{Error.Message}}
 
## Stack Traces
```shell
{{Error.FullStackTrace}} 
```

## Logs
```console
{{Logs}}
```

## Settings
{{Settings}}
 
## Assembly References
{{#App.AssemblyRefs}}
 - {{Name}}, Version={{Version}}  
{{/App.AssemblyRefs}}

## System Info  
```console
{{SystemInfo}}
```
".Replace("{{Logs}}", string.Join(@"\n\r", Settings.Default.app_console_logs.Split(';'))).Replace("{{Settings}}", $@"**Output Directory**:   {Settings.Default.settings_output_path}
**Bitrate**:   {(LAMEPreset)Settings.Default.settings_media_bitrate_quality}
**Media Format**:   {(Enums.MediaFormat)Settings.Default.settings_media_audio_format}
**Min. Media Length**:   {Settings.Default.settings_media_minimum_recorded_length_in_seconds}
**Group Media in Folder**:   {Settings.Default.advanced_file_group_media_in_folders_enabled}
**Track Title Separator Underscore**:   {Settings.Default.advanced_file_replace_space_by_underscore_enabled}
**Counter In File Name**:   {Settings.Default.advanced_file_counter_number_prefix_enabled}
**Counter In Media Tag**:   {Settings.Default.advanced_id3_counter_number_as_track_number_enabled}
**Language**:   {(Enums.LanguageType)Settings.Default.settings_language}
**Tab No**:   {Settings.Default.app_tab_number_selected}
**Delay When Track Ends**:   {Settings.Default.advanced_watcher_delay_next_recording_until_silent_enabled}
**Mute Ads**:   {Settings.Default.settings_mute_ads_enabled}
**Record Everything**:   {Settings.Default.advanced_record_everything}
**Analytics CID**:   {Settings.Default.app_analytics_cid}
**Record Over Recordings**:   {Settings.Default.advanced_record_over_recordings_enabled}
**Last Version Prompted**:   {Settings.Default.app_last_version_prompt}
**External API**:   {(Enums.ExternalAPIType)Settings.Default.app_selected_external_api_id}
**Audio Endpoint Device ID**:   {Settings.Default.app_selected_audio_device_id}
**Record Duplicate Recordings**:   {Settings.Default.advanced_record_over_recordings_and_duplicate_enabled}
**Counter Mask**:   {Settings.Default.app_counter_number_mask}
**Spotify API IDs**:   {!string.IsNullOrWhiteSpace(Settings.Default.app_spotify_api_client_secret) && !string.IsNullOrWhiteSpace(Settings.Default.app_spotify_api_client_id)}
**Extra Title To Subtitle**:   {Settings.Default.advanced_id3_extra_title_as_subtitle_enabled}
**Record Ads**:   {Settings.Default.advanced_record_everything_and_ads_enabled}");
            }
            catch { };

            ExceptionReporter er = new ExceptionReporter()
            {
                Config =
                {
                    AppName = Application.ProductName,
                    CompanyName = Application.CompanyName,
                    WebServiceUrl = "https://exception-mailer.herokuapp.com/send",
                    TitleText = "Exception Report",
                    TakeScreenshot = true,
                    SendMethod = ReportSendMethod.WebService,
                    TopMost = true,
                    ShowFlatButtons = true,
                    ShowLessDetailButton = true,
                    ReportCustomTemplate = !string.IsNullOrWhiteSpace(template) ? template : null,
                    ReportTemplateFormat = TemplateFormat.Markdown,
                },
            };
            er.Show(ex);
        }
    }
}
