namespace TextToSpeech.ViewModels.Commands
{
    using System.Windows.Input;

    using GalaSoft.MvvmLight.CommandWpf;

    using TextToSpeech.Utils;

    internal class ApplicationViewModelCommands
    {
        private readonly ApplicationViewModel _viewModel;

        private ICommand _playCommand;

        private ICommand _stopCommand;

        public ApplicationViewModelCommands(ApplicationViewModel viewModel)
        {
            Guard.NotNull(viewModel, "viewModel");

            this._viewModel = viewModel;
        }

        public ICommand PlayCommand
        {
            get
            {
                if (this._playCommand == null)
                {
                    this._playCommand = new RelayCommand(this._viewModel.Play, this._viewModel.CanPlay);
                }

                return this._playCommand;
            }
        }

        public ICommand StopCommand
        {
            get
            {
                if (this._stopCommand == null)
                {
                    this._stopCommand = new RelayCommand(this._viewModel.Stop, this._viewModel.CanStop);
                }

                return this._stopCommand;
            }
        }
    }
}