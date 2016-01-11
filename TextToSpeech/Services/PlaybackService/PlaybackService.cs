namespace TextToSpeech.Services.PlaybackService
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Media;
    using System.Reflection;
    using System.Threading;

    using TextToSpeech.Enums;
    using TextToSpeech.Utils;
    using TextToSpeech.ViewModels.BackgroundWorkerWrapper;

    internal class PlaybackService : IPlaybackService
    {
        private static readonly string soundPart = "TextToSpeech.Themes.sounds.{0}.wav";

        private static readonly string[] soundPartNames =
        {
            //"а", "б", "в", "г", "ґ", "д", "е", "є", "ж", "з", "и", "і", "ї", "й", "к", "л", "м", "н", "о", "п", "р", "с", "т", "у",
            //"ф", "х", "ц", "ч", "ш", "щ", "ю", "я",
            "а", "б", "ба", "бе", "би", "бо", "бу", "бь", "бьа", "бье", "бьі", "бьо", "бьу", "в", "ва", "ве", "ви", "во",
            "ву", "вь", "вьа", "вье", "вьі", "вьо", "вьу", "г", "га", "ге", "ги", "го", "гу", "гь", "гьа", "гье", "гьі",
            "гьо", "гьу", "ґ", "ґа", "ґе", "ґи", "ґо", "ґу", "ґь", "ґьа", "ґье", "ґьі", "ґьо", "ґьу", "д", "да", "де", "дж",
            "джа", "дже", "джи", "джо", "джу", "джь", "джьа", "джье", "джьі", "джьо", "джьу", "дз", "дза", "дзе", "дзи",
            "дзо", "дзу", "дзь", "дзьа", "дзье", "дзьі", "дзьо", "дзьу", "ди", "до", "ду", "дь", "дьа", "дье", "дьі", "дьо",
            "дьу", "е", "ж", "жа", "же", "жи", "жо", "жу", "жь", "жьа", "жье", "жьі", "жьо", "жьу", "з", "за", "зе", "зи",
            "зо", "зу", "зь", "зьа", "зье", "зьі", "зьо", "зьу", "и", "й", "йа", "йе", "йі", "йо", "йу", "і", "к", "ка", "ке",
            "ки", "ко", "ку", "кь", "кьа", "кье", "кьі", "кьо", "кьу", "л", "ла", "ле", "ли", "ло", "лу", "ль", "льа", "лье",
            "льі", "льо", "льу", "м", "ма", "ме", "ми", "мо", "му", "мь", "мьа", "мье", "мьі", "мьо", "мьу", "н", "на", "не",
            "ни", "но", "ну", "нь", "ньа", "нье", "ньі", "ньо", "ньу", "о", "п", "па", "пе", "пи", "по", "пу", "пь", "пьа",
            "пье", "пьі", "пьо", "пьу", "р", "ра", "ре", "ри", "ро", "ру", "рь", "рьа", "рье", "рьі", "рьо", "рьу", "с", "са",
            "се", "си", "со", "су", "сь", "сьа", "сье", "сьі", "сьо", "сьу", "т", "та", "те", "ти", "то", "ту", "ть", "тьа",
            "тье", "тьі", "тьо", "тьу", "у", "ф", "фа", "фе", "фи", "фо", "фу", "фь", "фьа", "фье", "фьі", "фьо", "фьу", "х",
            "ха", "хе", "хи", "хо", "ху", "хь", "хьа", "хье", "хьі", "хьо", "хьу", "ц", "ца", "це", "ци", "цо", "цу", "ць",
            "цьа", "цье", "цьі", "цьо", "цьу", "ч", "ча", "че", "чи", "чо", "чу", "чь", "чьа", "чье", "чьі", "чьо", "чьу",
            "ш", "ша", "ше", "ши", "шо", "шу", "шь", "шьа", "шье", "шьі", "шьу"
        };

        private static readonly int wordBreakTimeSpan = 200;

        private readonly Assembly _assembly;

        private readonly IBackgroundWorker _backgroundWorker;

        private PlayerState _currentPlayerState;

        private IList<string> _words;

        public PlaybackService(IBackgroundWorker backgroundWorker)
        {
            Guard.NotNull(backgroundWorker, "backgroundWorker");

            this._backgroundWorker = backgroundWorker;
            this._backgroundWorker.DoWork += this.BackgroundWorker_DoWork;
            this._backgroundWorker.RunWorkerCompleted += this.BackgroundWorker_RunWorkerCompleted;

            this._assembly = Assembly.GetExecutingAssembly();
            this._currentPlayerState = PlayerState.NotInitialized;
            this._words = new List<string>();
        }

        #region IPlaybackService Members

        public event EventHandler PlaybackCompleted;

        public bool CanInit()
        {
            return this._currentPlayerState == PlayerState.NotInitialized || this._currentPlayerState == PlayerState.Ready;
        }

        public bool CanPlay()
        {
            return this._currentPlayerState == PlayerState.Ready && this._words.Any();
        }

        public bool CanStop()
        {
            return this._currentPlayerState == PlayerState.Playing;
        }

        public void Init(IEnumerable<string> words)
        {
            Guard.NotNull(words, "words");

            this._words = words.ToList();

            this._currentPlayerState = PlayerState.Ready;
        }

        public void Play()
        {
            if (!this.CanPlay())
            {
                throw new InvalidOperationException("\"Play\" operation is forbidden.");
            }

            this._currentPlayerState = PlayerState.Playing;

            this.MoveToWordWithIndex(0);
        }

        public void Stop()
        {
            if (!this.CanStop())
            {
                throw new InvalidOperationException("\"Stop\" operation is forbidden.");
            }

            this._currentPlayerState = PlayerState.StoppingPending;
        }

        #endregion

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            int wordIndex = doWorkEventArgs.Argument as int? ?? 0;

            doWorkEventArgs.Result = wordIndex;

            string word = this._words[wordIndex];

            using (SoundPlayer player = new SoundPlayer())
            {
                IList<string> soundPartNames = this.GetSoundPartNamesFromWord(word)
                    .ToList();

                int i = 0;
                while (this._currentPlayerState == PlayerState.Playing && i < soundPartNames.Count)
                {
                    using (
                        Stream stream =
                            this._assembly.GetManifestResourceStream(string.Format(PlaybackService.soundPart,
                                soundPartNames[i])))
                    {
                        player.Stream = stream;
                        player.PlaySync();
                    }

                    i++;
                }
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender,
                                                         RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            if (runWorkerCompletedEventArgs.Error != null)
            {
                this.FinishPlayback();
            }
            else
            {
                int wordIndex = runWorkerCompletedEventArgs.Result as int? ?? 0;

                switch (this._currentPlayerState)
                {
                    case PlayerState.Playing:
                    {
                        Thread.Sleep(PlaybackService.wordBreakTimeSpan);

                        this.MoveToWordWithIndex(wordIndex + 1);
                        break;
                    }
                    case PlayerState.StoppingPending:
                    {
                        this.FinishPlayback();
                        break;
                    }
                }
            }
        }

        private void FinishPlayback()
        {
            this._currentPlayerState = PlayerState.Ready;

            if (this.PlaybackCompleted != null)
            {
                this.PlaybackCompleted.Invoke(this, new EventArgs());
            }
        }

        private IEnumerable<string> GetSoundPartNamesFromWord(string word)
        {
            while (word.Length != 0)
            {
                bool partFound = false;

                foreach (string soundPartName in PlaybackService.soundPartNames.Where(p => p.Length <= word.Length)
                    .OrderByDescending(p => p.Length))
                {
                    if (word.IndexOf(soundPartName, StringComparison.Ordinal) == 0)
                    {
                        partFound = true;
                        word = (word.Length - soundPartName.Length > 0) ? word.Substring(soundPartName.Length) : string.Empty;

                        yield return soundPartName;
                        break;
                    }
                }

                if (!partFound)
                {
                    word = (word.Length > 1) ? word.Substring(1) : string.Empty;
                }
            }
        }

        private void MoveToWordWithIndex(int wordIndex)
        {
            if (wordIndex < this._words.Count)
            {
                this.RunBackgroundWorker(wordIndex);
            }
            else
            {
                this.FinishPlayback();
            }
        }

        private void RunBackgroundWorker(int wordIndex)
        {
            if (this._backgroundWorker.IsBusy)
            {
                throw new InvalidOperationException("BackgroundWorker is busy.");
            }

            this._backgroundWorker.RunWorkerAsync(wordIndex);
        }
    }
}