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
".Replace("{{Logs}}", string.Join(@"\n\r", Settings.Default.Logs.Split(';'))).Replace("{{Settings}}", $@"**Output Directory**:   {Settings.Default.Directory}
**Bitrate**:   {(LAMEPreset)Settings.Default.Bitrate}
**Media Format**:   {(Enums.MediaFormat)Settings.Default.MediaFormat}
**Min. Media Length**:   {Settings.Default.MinimumRecordedLengthSeconds}
**Group Media in Folder**:   {Settings.Default.GroupByFoldersEnabled}
**Track Title Separator Underscore**:   {Settings.Default.TrackTitleSeparatorEnabled}
**Counter In File Name**:   {Settings.Default.OrderNumberInfrontOfFileEnabled}
**Counter In Media Tag**:   {Settings.Default.OrderNumberInMediaTagEnabled}
**Language**:   {(Enums.LanguageType)Settings.Default.Language}
**Tab No**:   {Settings.Default.TabNo}
**Delay When Track Ends**:   {Settings.Default.EndingSongDelayEnabled}
**Mute Ads**:   {Settings.Default.MuteAdsEnabled}
**Record Everything**:   {Settings.Default.RecordEverythingEnabled}
**Analytics CID**:   {Settings.Default.AnalyticsCID}
**Record Over Recordings**:   {Settings.Default.RecordOverRecordingsEnabled}
**Last Version Prompted**:   {Settings.Default.LastVersionPrompted}
**External API**:   {(Enums.ExternalAPIType)Settings.Default.ExternalAPI}
**Audio Endpoint Device ID**:   {Settings.Default.AudioEndPointDeviceID}
**Record Duplicate Recordings**:   {Settings.Default.RecordDuplicateRecordingsEnabled}
**Counter Mask**:   {Settings.Default.OrderNumberMask}
**Spotify API IDs**:   {!string.IsNullOrWhiteSpace(Settings.Default.SpotifyAPISecretId) && !string.IsNullOrWhiteSpace(Settings.Default.SpotifyAPIClientId)}
**Extra Title To Subtitle**:   {Settings.Default.ExtraTitleToSubtitleEnabled}
**Record Ads**:   {Settings.Default.RecordAdsEnabled}");
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
