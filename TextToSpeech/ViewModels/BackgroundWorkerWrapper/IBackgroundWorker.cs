namespace TextToSpeech.ViewModels.BackgroundWorkerWrapper
{
    using System;
    using System.ComponentModel;

    public interface IBackgroundWorker : IDisposable
    {
        event DoWorkEventHandler DoWork;

        event RunWorkerCompletedEventHandler RunWorkerCompleted;

        #region Properties and Indexers

        bool CancellationPending
        {
            get;
        }

        bool IsBusy
        {
            get;
        }

        #endregion

        #region Public Methods

        void CancelAsync();

        void RunWorkerAsync();

        void RunWorkerAsync(object arguments);

        #endregion
    }
}