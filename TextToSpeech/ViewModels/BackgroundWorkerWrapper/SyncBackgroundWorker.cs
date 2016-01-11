namespace TextToSpeech.ViewModels.BackgroundWorkerWrapper
{
    using System.ComponentModel;

    public class SyncBackgroundWorker : IBackgroundWorker
    {
        #region IBackgroundWorker Members

        public event DoWorkEventHandler DoWork = delegate
        {
        };

        public event RunWorkerCompletedEventHandler RunWorkerCompleted = delegate
        {
        };

        public void CancelAsync()
        {
            //Do nothing;
        }

        public void RunWorkerAsync()
        {
            DoWorkEventArgs args = new DoWorkEventArgs(null);
            this.RunWorkerSync(args);
        }

        public void RunWorkerAsync(object arguments)
        {
            DoWorkEventArgs args = new DoWorkEventArgs(arguments);
            this.RunWorkerSync(args);
        }

        public bool CancellationPending
        {
            get
            {
                return false;
            }
        }

        public bool IsBusy
        {
            get
            {
                return false;
            }
        }

        public void Dispose()
        {
            //Do nothing;
        }

        #endregion

        private void RunWorkerSync(DoWorkEventArgs args)
        {
            this.DoWork.Invoke(this, args);
            this.RunWorkerCompleted.Invoke(this, new RunWorkerCompletedEventArgs(args.Result, null, false));
        }
    }
}