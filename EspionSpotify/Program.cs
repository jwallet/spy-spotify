using ExceptionReporting;
using System;
using System.Configuration;
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
            thread.Start();
            thread.Join();
        }

        // Creates the error message and displays it.
        internal static void ReportException(Exception ex)
        {
            var emailPassword = Environment.GetEnvironmentVariable("spytify_email_support_password", EnvironmentVariableTarget.Machine);
            ExceptionReporter er = new ExceptionReporter()
            {
                Config =
                {
                    AppName = Application.ProductName,
                    CompanyName = Application.CompanyName,
                    TitleText = "Error Report",
                    TakeScreenshot = true,
                    SendMethod = ReportSendMethod.SMTP,
                    SmtpServer = "smtp.gmail.com",
                    SmtpUseDefaultCredentials = false,
                    SmtpPort = 587,
                    TopMost = true,
                    ShowFlatButtons = true,
                    ShowLessDetailButton = true,
                    SmtpUseSsl = true,
                    ReportTemplateFormat = TemplateFormat.Markdown,
                    EmailReportAddress = "report.spytify@gmail.com",
                    SmtpFromAddress = "report.spytify@gmail.com",
                    SmtpUsername = "report.spytify@gmail.com",
                    SmtpPassword = emailPassword,
                },
            };
            er.Show(ex);
        }
    }
}
