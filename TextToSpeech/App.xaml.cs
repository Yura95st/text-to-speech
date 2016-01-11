namespace TextToSpeech
{
    using System;
    using System.Text;
    using System.Windows;
    using System.Windows.Threading;

    using TextToSpeech.Services.PlaybackService;
    using TextToSpeech.Services.SyllablesService;
    using TextToSpeech.Services.TranscriptionService;
    using TextToSpeech.ViewModels;
    using TextToSpeech.ViewModels.BackgroundWorkerWrapper;
    using TextToSpeech.Views;

    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ApplicationViewModel _viewModel;

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            StringBuilder messageBuilder = new StringBuilder();

            messageBuilder.AppendLine("Wow ... we didn't expect You to do that.");
            messageBuilder.AppendLine(
                "Please send us the exception, with information on what You were doing at the time. Since this was unexpected, the application is now in an inconsistent state.");
            messageBuilder.AppendLine();
            messageBuilder.AppendLine();
            messageBuilder.AppendLine("The error details are:");

            Exception exception = e.Exception;

            while (exception != null)
            {
                messageBuilder.AppendLine(exception.Message);

                exception = exception.InnerException;
            }

            messageBuilder.AppendLine();
            messageBuilder.AppendLine();
            messageBuilder.AppendLine("The stack trace:");
            messageBuilder.AppendLine(e.Exception.StackTrace);

            string message = messageBuilder.ToString();

            MessageBoxResult result = MessageBox.Show(message, "Unhandled Exception", MessageBoxButton.OK,
                MessageBoxImage.Error);

            if (result == MessageBoxResult.OK)
            {
                this.Shutdown();
            }
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            this._viewModel = new ApplicationViewModel(new SyllablesService(), new TranscriptionService(),
                new PlaybackService(new AsyncBackgroundWorker()));

            ApplicationView view = new ApplicationView { DataContext = this._viewModel };

            view.Show();
        }
    }
}