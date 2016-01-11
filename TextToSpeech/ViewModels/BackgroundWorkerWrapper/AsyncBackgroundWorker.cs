namespace TextToSpeech.ViewModels.BackgroundWorkerWrapper
{
    using System.ComponentModel;

    public class AsyncBackgroundWorker : IBackgroundWorker
    {
        private readonly BackgroundWorker _worker = new BackgroundWorker { WorkerSupportsCancellation = true };

        #region IBackgroundWorker Members

        public event DoWorkEventHandler DoWork
        {
            add
            {
                this._worker.DoWork += value;
            }
            remove
            {
                this._worker.DoWork -= value;
            }
        }

        public event RunWorkerCompletedEventHandler RunWorkerCompleted
        {
            add
            {
                this._worker.RunWorkerCompleted += value;
            }
            remove
            {
                this._worker.RunWorkerCompleted -= value;
            }
        }

        public void CancelAsync()
        {
            this._worker.CancelAsync();
        }

        public void RunWorkerAsync()
        {
            this._worker.RunWorkerAsync();
        }

        public void RunWorkerAsync(object args)
        {
            this._worker.RunWorkerAsync(args);
        }

        public bool CancellationPending
        {
            get
            {
                return this._worker.CancellationPending;
            }
        }

        public bool IsBusy
        {
            get
            {
                return this._worker.IsBusy;
            }
        }

        public void Dispose()
        {
            if (this._worker != null)
            {
                this._worker.Dispose();
            }
        }

        #endregion
    }
}