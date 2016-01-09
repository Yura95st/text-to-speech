namespace TextToSpeech.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ConsoleApplication1.Enums;

    using TextToSpeech.Enums;

    internal class SyllablesParser : ISyllablesParser
    {
        private static readonly char[] obstruents = { 'к', 'п', 'с', 'т', 'ф', 'х', 'ц', 'ч', 'ш', 'щ' };

        private static readonly char[] sibilants = { 'б', 'г', 'ґ', 'д', 'ж', 'з' };

        private static readonly char[] sonorouses = { 'в', 'й', 'л', 'м', 'н', 'р' };

        private static readonly char[] vowels = { 'а', 'е', 'є', 'и', 'і', 'ї', 'о', 'у', 'ю', 'я' };

        #region ISyllablesParser Members

        public IEnumerable<string> GetSyllables(string word)
        {
            List<string> syllables = new List<string>();

            if (string.IsNullOrWhiteSpace(word))
            {
                return syllables;
            }

            word = word.Trim()
                .ToLower();
            if (!SyllablesParser.IsValidWord(word))
            {
                throw new ArgumentException(string.Format("Word \"{0}\" is invalid.", word), "word");
            }

            int start = 0;
            int end = 1;

            while (end < word.Length)
            {
                switch (SyllablesParser.GetCuttingMode(word, end))
                {
                    case SyllableCuttingMode.Current:
                    {
                        if (end - start - 1 > 0
                            && SyllablesParser.DoesWordContainVowel(word.Substring(start, end - start - 1)))
                        {
                            syllables.Add(word.Substring(start, end - start - 1));
                            start = end - 1;
                        }
                        break;
                    }

                    case SyllableCuttingMode.Next:
                    {
                        if (SyllablesParser.DoesWordContainVowel(word.Substring(start, end - start)))
                        {
                            syllables.Add(word.Substring(start, end - start));
                            start = end;
                        }
                        break;
                    }
                }

                end++;
            }

            if (SyllablesParser.DoesWordContainVowel(word.Substring(start)))
            {
                syllables.Add(word.Substring(start));
            }
            else
            {
                syllables[syllables.Count - 1] += word.Substring(start);
            }

            return syllables;
        }

        #endregion

        private static bool DoesWordContainVowel(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return false;
            }

            return word.Any(SyllablesParser.IsVowel);
        }

        private static ConsonantType GetConsonantType(char c)
        {
            if (!SyllablesParser.IsConsonant(c))
            {
                throw new ArgumentException(string.Format("Char '{0}' is not a consonant.", c), "c");
            }

            if (SyllablesParser.sonorouses.Contains(c))
            {
                return ConsonantType.Sonorous;
            }

            return SyllablesParser.sibilants.Contains(c) ? ConsonantType.Sibilant : ConsonantType.Obstruents;
        }

        private static SyllableCuttingMode GetCuttingMode(string word, int index)
        {
            char firstChar = word[index - 1] != 'ь' ? word[index - 1] : word[index - 2];
            char secondChar = word[index];

            // Rule #1
            if (index + 1 < word.Length)
            {
                char thirdChar = word[index + 1] != '\'' ? word[index + 1] : word[index + 2];

                if (SyllablesParser.IsVowel(firstChar) && SyllablesParser.IsConsonant(secondChar)
                    && SyllablesParser.IsVowel(thirdChar))
                {
                    return SyllableCuttingMode.Next;
                }
            }

            if (SyllablesParser.IsConsonant(firstChar) && SyllablesParser.IsConsonant(secondChar))
            {
                // Rule #6
                if (firstChar == secondChar)
                {
                    return SyllableCuttingMode.Current;
                }
                if (SyllablesParser.GetConsonantType(firstChar) == SyllablesParser.GetConsonantType(secondChar))
                {
                    // Rule #5
                    if (SyllablesParser.GetConsonantType(firstChar) == ConsonantType.Sonorous)
                    {
                        return SyllableCuttingMode.Next;
                    }
                    return SyllableCuttingMode.Current;
                }
                if (SyllablesParser.GetConsonantType(secondChar) == ConsonantType.Obstruents) // Rule #3
                {
                    return SyllableCuttingMode.Next;
                }
                if (SyllablesParser.GetConsonantType(secondChar) == ConsonantType.Sonorous) // Rule #4
                {
                    return SyllableCuttingMode.Current;
                }
            }
            else if (SyllablesParser.IsVowel(firstChar) && SyllablesParser.IsVowel(secondChar))
            {
                return SyllableCuttingMode.Next;
            }

            return SyllableCuttingMode.Skip;
        }

        private static bool IsConsonant(char c)
        {
            return SyllablesParser.sibilants.Contains(c) || SyllablesParser.sonorouses.Contains(c)
                || SyllablesParser.obstruents.Contains(c);
        }

        private static bool IsValidWord(string word)
        {
            return word.All(c => SyllablesParser.IsConsonant(c) || SyllablesParser.IsVowel(c) || c == 'ь' || c == '\'');
        }

        private static bool IsVowel(char c)
        {
            return SyllablesParser.vowels.Contains(c);
        }
    }
}