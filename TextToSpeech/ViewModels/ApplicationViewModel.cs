namespace TextToSpeech.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Input;

    using GalaSoft.MvvmLight;

    using TextToSpeech.Enums;
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

        private readonly IPlaybackService _playbackService;

        private readonly ISyllablesService _syllablesService;

        private readonly ITranscriptionService _transcriptionService;

        private string _inputText;

        private bool _isInputTextBoxEnabled;

        private OutputMode _outputMode;

        private IDictionary<OutputMode, string> _outputModeProperties;

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
            this._isInputTextBoxEnabled = true;
            this._outputMode = OutputMode.Transcription;
            this.OutputText = "";
            this._words = new List<string>();

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
                    this.UpdateOutputText();
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

        public OutputMode OutputMode
        {
            get
            {
                return this._outputMode;
            }
            set
            {
                if (value != this._outputMode)
                {
                    this._outputMode = value;
                    this.RaisePropertyChanged(() => this.OutputMode);

                    this.UpdateOutputText();
                }
            }
        }

        public IDictionary<OutputMode, string> OutputModeProperties
        {
            get
            {
                if (this._outputModeProperties == null)
                {
                    this._outputModeProperties = new Dictionary<OutputMode, string>
                    { { OutputMode.Syllables, "Syllables" }, { OutputMode.Transcription, "Transcription" } };
                }

                return this._outputModeProperties;
            }
        }

        public string OutputText
        {
            get;
            private set;
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

        private string GetSyllablesText()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string word in this._words.Select(w => w.ToLower()))
            {
                bool isFirstSyllable = true;

                try
                {
                    foreach (string syllable in this._syllablesService.GetSyllables(word))
                    {
                        if (!isFirstSyllable)
                        {
                            sb.Append("-");
                        }

                        sb.Append(syllable);

                        isFirstSyllable = false;
                    }
                }
                catch (ArgumentException)
                {
                    sb.Append(word);
                }

                sb.Append(" ");
            }

            return sb.ToString();
        }

        private IEnumerable<string> GetTranscriptedWords()
        {
            return this._words.Select(word => this._transcriptionService.GetTranscription(word.ToLower()));
        }

        private string GetTranscriptionsText()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string word in this.GetTranscriptedWords())
            {
                sb.Append(string.Format("{0} ", word));
            }

            return sb.ToString();
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

        private void PlaybackServiceOnPlaybackCompleted(object sender, EventArgs eventArgs)
        {
            this.IsInputTextBoxEnabled = true;

            CommandManager.InvalidateRequerySuggested();
        }

        private void UpdateOutputText()
        {
            this.OutputText = (this._outputMode == OutputMode.Syllables)
                ? this.GetSyllablesText() : this.GetTranscriptionsText();

            this.RaisePropertyChanged(() => this.OutputText);
        }

        private void UpdateWords()
        {
            this._words = ApplicationViewModel.GetWordsFromText(this._inputText)
                .ToList();

            this._playbackService.Init(this.GetTranscriptedWords());

            this.RaisePropertyChanged(() => this.WordsCount);
        }
    }
}