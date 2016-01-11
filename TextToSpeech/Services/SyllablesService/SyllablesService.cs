namespace TextToSpeech.Services.SyllablesService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using TextToSpeech.Enums;

    public class SyllablesService : ISyllablesService
    {
        private static readonly char[] obstruents = { 'к', 'п', 'с', 'т', 'ф', 'х', 'ц', 'ч', 'ш', 'щ' };

        private static readonly char[] sibilants = { 'б', 'г', 'ґ', 'д', 'ж', 'з' };

        private static readonly char[] sonorouses = { 'в', 'й', 'л', 'м', 'н', 'р' };

        private static readonly char[] vowels = { 'а', 'е', 'є', 'и', 'і', 'ї', 'о', 'у', 'ю', 'я' };

        #region ISyllablesService Members

        public IEnumerable<string> GetSyllables(string word)
        {
            List<string> syllables = new List<string>();

            if (string.IsNullOrWhiteSpace(word))
            {
                return syllables;
            }

            word = word.Trim()
                .ToLower();
            if (!SyllablesService.IsValidWord(word))
            {
                throw new ArgumentException(string.Format("Word \"{0}\" is invalid.", word), "word");
            }

            int start = 0;
            int end = 1;

            while (end < word.Length)
            {
                switch (SyllablesService.GetCuttingMode(word, end))
                {
                    case SyllableCuttingMode.Current:
                    {
                        if (end - start - 1 > 0
                            && SyllablesService.DoesWordContainVowel(word.Substring(start, end - start - 1)))
                        {
                            syllables.Add(word.Substring(start, end - start - 1));
                            start = end - 1;
                        }
                        break;
                    }

                    case SyllableCuttingMode.Next:
                    {
                        if (SyllablesService.DoesWordContainVowel(word.Substring(start, end - start)))
                        {
                            syllables.Add(word.Substring(start, end - start));
                            start = end;
                        }
                        break;
                    }
                }

                end++;
            }

            if (!SyllablesService.DoesWordContainVowel(word.Substring(start)) && syllables.Count - 1 >= 0)
            {
                syllables[syllables.Count - 1] += word.Substring(start);
            }
            else
            {
                syllables.Add(word.Substring(start));
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

            return word.Any(SyllablesService.IsVowel);
        }

        private static ConsonantType GetConsonantType(char c)
        {
            if (!SyllablesService.IsConsonant(c))
            {
                throw new ArgumentException(string.Format("Char '{0}' is not a consonant.", c), "c");
            }

            if (SyllablesService.sonorouses.Contains(c))
            {
                return ConsonantType.Sonorous;
            }

            return SyllablesService.sibilants.Contains(c) ? ConsonantType.Sibilant : ConsonantType.Obstruents;
        }

        private static SyllableCuttingMode GetCuttingMode(string word, int index)
        {
            char firstChar = word[index - 1] != 'ь' ? word[index - 1] : word[index - 2];
            char secondChar = word[index];

            // Rule #1
            if (index + 1 < word.Length)
            {
                char thirdChar = word[index + 1] != '\'' ? word[index + 1] : word[index + 2];

                if (SyllablesService.IsVowel(firstChar) && SyllablesService.IsConsonant(secondChar)
                    && SyllablesService.IsVowel(thirdChar))
                {
                    return SyllableCuttingMode.Next;
                }
            }

            if (SyllablesService.IsConsonant(firstChar) && SyllablesService.IsConsonant(secondChar))
            {
                // Rule #6
                if (firstChar == secondChar)
                {
                    return SyllableCuttingMode.Current;
                }
                if (SyllablesService.GetConsonantType(firstChar) == SyllablesService.GetConsonantType(secondChar))
                {
                    // Rule #5
                    if (SyllablesService.GetConsonantType(firstChar) == ConsonantType.Sonorous)
                    {
                        return SyllableCuttingMode.Next;
                    }
                    return SyllableCuttingMode.Current;
                }
                if (SyllablesService.GetConsonantType(secondChar) == ConsonantType.Obstruents) // Rule #3
                {
                    return SyllableCuttingMode.Next;
                }
                if (SyllablesService.GetConsonantType(secondChar) == ConsonantType.Sonorous) // Rule #4
                {
                    return SyllableCuttingMode.Current;
                }
            }
            else if (SyllablesService.IsVowel(firstChar) && SyllablesService.IsVowel(secondChar))
            {
                return SyllableCuttingMode.Next;
            }

            return SyllableCuttingMode.Skip;
        }

        private static bool IsConsonant(char c)
        {
            return SyllablesService.sibilants.Contains(c) || SyllablesService.sonorouses.Contains(c)
                || SyllablesService.obstruents.Contains(c);
        }

        private static bool IsValidWord(string word)
        {
            return Regex.IsMatch(word, "^[а-яєіїґ-[ьёъыэ]]+(\'[яюєї])?[а-яєіїґ-[ёъыэ]]*$");
        }

        private static bool IsVowel(char c)
        {
            return SyllablesService.vowels.Contains(c);
        }
    }
}