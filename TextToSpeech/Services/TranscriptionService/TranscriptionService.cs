namespace TextToSpeech.Services.TranscriptionService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TranscriptionService : ITranscriptionService
    {
        private static readonly char[] consonants =
        {
            'к', 'п', 'с', 'т', 'ф', 'х', 'ц', 'ч', 'ш', 'б', 'г', 'ґ', 'д', 'ж',
            'з', 'в', 'л', 'м', 'н', 'р'
        };

        private static readonly char[] vowelsAndSpecials = { 'а', 'е', 'и', 'і', 'о', 'у', '\'', 'ь' };

        private readonly IDictionary<string, string> simplificationRules = new Dictionary<string, string>
        {
            { "сш", "ш" }, { "зж", "ж" }, { "здж", "ждж" }, { "жсь", "зьсь" }, { "шсь", "сь" }, { "чсь", "цьсь" },
            { "жць", "зьсь" }, { "шць", "сць" }, { "чць", "ць" }, { "дж", "джж" }, { "дз", "дзз" }, { "дс", "дзс" },
            { "дц", "дзц" }, { "дч", "джч" }, { "дш", "джш" }, { "тс", "ц" }, { "тц", "ц" }, { "тч", "ч" }, { "тш", "чш" },
            { "нстс", "нст" }, { "нтськ", "ньськ" }, { "стськ", "ськ" }, { "стць", "сьць" }, { "стч", "шч" }, { "стд", "зд" },
            { "стс", "с" }, { "здць", "зьць" }
        };

        #region ITranscriptionService Members

        public string GetTranscription(string word)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < word.Length; i++)
            {
                switch (word[i])
                {
                    case 'я':
                    {
                        sb.Append((i == 0 || TranscriptionService.vowelsAndSpecials.Contains(word[i - 1])) ? "йа" : "ьа");
                        break;
                    }

                    case 'ю':
                    {
                        sb.Append((i == 0 || TranscriptionService.vowelsAndSpecials.Contains(word[i - 1])) ? "йу" : "ьу");
                        break;
                    }

                    case 'є':
                    {
                        sb.Append((i == 0 || TranscriptionService.vowelsAndSpecials.Contains(word[i - 1])) ? "йе" : "ье");
                        break;
                    }

                    case 'ї':
                    {
                        sb.Append("йі");
                        break;
                    }

                    case 'щ':
                    {
                        sb.Append("шч");
                        break;
                    }

                    case 'і':
                    {
                        sb.Append((i != 0 && TranscriptionService.consonants.Contains(word[i - 1])) ? "ьі" : "і");
                        break;
                    }

                    case '\'':
                    {
                        break;
                    }

                    default:
                    {
                        sb.Append(word[i]);
                        break;
                    }
                }
            }

            foreach (
                KeyValuePair<string, string> keyValuePair in
                    this.simplificationRules.OrderByDescending(kvp => kvp.Key.Length))
            {
                sb = sb.Replace(keyValuePair.Key, keyValuePair.Value);
            }

            return sb.ToString();
        }

        #endregion
    }
}