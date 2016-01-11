namespace TextToSpeech.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Input;

    using GalaSoft.MvvmLight;

    using TextToSpeech.Services.PlaybackService;
    using TextToSpeech.Services.SyllablesService;
    using TextToSpeech.Services.TranscriptionService;
    using TextToSpeech.Utils;
    using TextToSpeech.ViewModels.Commands;

    internal class ApplicationViewModel : ObservableObject
    {
        private static readonly char[] delimiters =
        {
            ' ', '-', ',', '.', '!', '?', ':', ';', '(', ')', '[', ']', '{', '}',
            '\"'
        };

        private readonly ITranscriptionService _transcriptionService;

        private readonly IPlaybackService _playbackService;

        private string _inputText;

        private bool _isInputTextBoxEnabled;

        private ISyllablesService _syllablesService;

        private string _transcriptionText;

        private IList<string> _words;

        public ApplicationViewModel(ISyllablesService syllablesService, ITranscriptionService transcriptionService,
                                    IPlaybackService playbackService)
        {
            Guard.NotNull(syllablesService, "syllablesService");
            Guard.NotNull(transcriptionService, "transcriptionService");
            Guard.NotNull(playbackService, "playbackService");

            this._syllablesService = syllablesService;
            this._transcriptionService = transcriptionService;
            this._playbackService = playbackService;

            this._playbackService.PlaybackCompleted += this.PlaybackServiceOnPlaybackCompleted;

            this._inputText = "";
            this._transcriptionText = "";
            this._words = new List<string>();
            this._isInputTextBoxEnabled = true;

            this.Commands = new ApplicationViewModelCommands(this);
        }

        public ApplicationViewModelCommands Commands
        {
            get;
            private set;
        }

        public string InputText
        {
            get
            {
                return this._inputText;
            }
            set
            {
                if (value != this._inputText)
                {
                    this._inputText = value;
                    this.RaisePropertyChanged(() => this.InputText);

                    this.UpdateWords();
                    this.UpdateTranscriptionText();
                }
            }
        }

        public bool IsInputTextBoxEnabled
        {
            get
            {
                return this._isInputTextBoxEnabled;
            }
            private set
            {
                if (value != this._isInputTextBoxEnabled)
                {
                    this._isInputTextBoxEnabled = value;

                    this.RaisePropertyChanged(() => this.IsInputTextBoxEnabled);
                }
            }
        }

        public string TranscriptionText
        {
            get
            {
                return this._transcriptionText;
            }
        }

        public int WordsCount
        {
            get
            {
                return this._words.Count;
            }
        }

        public bool CanPlay()
        {
            return this._playbackService.CanPlay();
        }

        public bool CanStop()
        {
            return this._playbackService.CanStop();
        }

        public void Play()
        {
            this.IsInputTextBoxEnabled = false;

            this._playbackService.Play();
        }

        public void Stop()
        {
            this._playbackService.Stop();
        }

        private IEnumerable<string> GetTranscriptedWords()
        {
            return this._words.Select(word => this._transcriptionService.GetTranscription(word.ToLower()));
        }

        private static IEnumerable<string> GetWordsFromText(string text)
        {
            foreach (string word in text.Trim()
                .Split(ApplicationViewModel.delimiters))
            {
                string trimmedWord = word.Trim();

                if (trimmedWord.Length > 0)
                {
                    yield return trimmedWord;
                }
            }
        }

        private void UpdateTranscriptionText()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string word in this.GetTranscriptedWords())
            {
                sb.Append(string.Format("{0} ", word));
            }

            this._transcriptionText = sb.ToString();
            this.RaisePropertyChanged(() => this.TranscriptionText);
        }

        private void UpdateWords()
        {
            this._words = ApplicationViewModel.GetWordsFromText(this._inputText)
                .ToList();

            this._playbackService.Init(this.GetTranscriptedWords());

            this.RaisePropertyChanged(() => this.WordsCount);
        }

        private void PlaybackServiceOnPlaybackCompleted(object sender, EventArgs eventArgs)
        {
            this.IsInputTextBoxEnabled = true;

            CommandManager.InvalidateRequerySuggested();
        }
    }
}