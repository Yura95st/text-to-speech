namespace TextToSpeech.Services.WordVoiceService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Media;
    using System.Reflection;

    internal class WordVoiceService : IWordVoiceService
    {
        private readonly Assembly _assembly = Assembly.GetExecutingAssembly();

        private readonly string _soundPart = "TextToSpeech.Themes.sounds.{0}.wav";

        private readonly string[] _soundPartNames =
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

        #region IWordVoiceService Members

        public void PlayWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return;
            }

            using (SoundPlayer player = new SoundPlayer())
            {
                foreach (string soundPartName in this.GetSoundPartNamesFromWord(word))
                {
                    using (
                        Stream stream =
                            this._assembly.GetManifestResourceStream(string.Format(this._soundPart, soundPartName)))
                    {
                        player.Stream = stream;
                        player.PlaySync();
                    }
                }
            }
        }

        #endregion

        private IEnumerable<string> GetSoundPartNamesFromWord(string word)
        {
            while (word.Length != 0)
            {
                bool partFound = false;

                foreach (string soundPartName in this._soundPartNames.Where(p => p.Length <= word.Length)
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
    }
}