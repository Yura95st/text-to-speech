namespace TextToSpeech.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using GalaSoft.MvvmLight;

    using TextToSpeech.Services.SyllablesService;
    using TextToSpeech.Services.TranscriptionService;
    using TextToSpeech.Services.WordVoiceService;
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

        private readonly IWordVoiceService _wordVoiceService;

        private string _inputText;

        private ISyllablesService _syllablesService;

        private string _transcriptionText;

        private IList<string> _words;

        public ApplicationViewModel(ISyllablesService syllablesService, ITranscriptionService transcriptionService,
                                    IWordVoiceService wordVoiceService)
        {
            Guard.NotNull(syllablesService, "syllablesService");
            Guard.NotNull(transcriptionService, "transcriptionService");
            Guard.NotNull(wordVoiceService, "wordVoiceService");

            this._syllablesService = syllablesService;
            this._transcriptionService = transcriptionService;
            this._wordVoiceService = wordVoiceService;

            this._inputText = "";
            this._transcriptionText = "";
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
                    this.UpdateTranscriptionText();
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

        public bool CanPlay()
        {
            return this._words.Any();
        }

        public bool CanStop()
        {
            return false;
        }

        public void Play()
        {
            foreach (string word in this._words)
            {
                this._wordVoiceService.PlayWord(this._transcriptionService.GetTranscription(word));
            }
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }

        private IEnumerable<string> GetWordsFromText(string text)
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

            foreach (string word in this._words)
            {
                sb.Append(this._transcriptionService.GetTranscription(word.ToLower()));
                sb.Append(" ");
            }

            this._transcriptionText = sb.ToString();
            this.RaisePropertyChanged(() => this.TranscriptionText);
        }

        private void UpdateWords()
        {
            this._words = this.GetWordsFromText(this._inputText)
                .ToList();

            this.RaisePropertyChanged(() => this.WordsCount);
        }

        public int WordsCount
        {
            get
            {
                return this._words.Count;
            }
        }
    }
}