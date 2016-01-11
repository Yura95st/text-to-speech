namespace TextToSpeech.Tests.Services.SyllablesService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using TextToSpeech.Services.SyllablesService;

    [TestFixture]
    public class SyllablesServiceTests
    {
        private SyllablesService _syllablesService;

        [Test]
        public void GetSyllables_FifthRuleWords_ReturnsValidSyllables()
        {
            // Rule: Якщо поряд стоять два сонорні звуки, то вони належать до різних складів
            string[] words = { "зем-ляк", "бар-ві-нок", "чо-тир-ма", "сум-но", "ча-рів-ни-ця", "чер-во-ний", "пій-ло" };

            foreach (string word in words)
            {
                IEnumerable<string> syllables = this._syllablesService.GetSyllables(word.Replace("-", ""))
                    .ToList();

                Assert.AreEqual(word.Split('-'), syllables);
            }
        }

        [Test]
        public void GetSyllables_FirstRuleWords_ReturnsValidSyllables()
        {
            // Rule: Якщо між голосними стоїть один приголос­ний, то він завжди належить до наступного складу
            string[] words = { "мо-ло-ти-ти", "го-во-ри-ти", "мо-ле-ку-ла", "ве-че-ря", "су-ми" };

            foreach (string word in words)
            {
                IEnumerable<string> syllables = this._syllablesService.GetSyllables(word.Replace("-", ""))
                    .ToList();

                Assert.AreEqual(word.Split('-'), syllables);
            }
        }

        [Test]
        public void GetSyllables_FourthRuleWords_ReturnsValidSyllables()
        {
            // Rule: Якщо з двох приголосних перший глухий або дзвінкий, а другий сонорний, то обидва належать до наступного складу
            string[] words = { "ко-смос", "ху-до-жник", "те-хні-ка", "о-брій", "мі-дний", "до-бре", "по-трі-бно" };

            foreach (string word in words)
            {
                IEnumerable<string> syllables = this._syllablesService.GetSyllables(word.Replace("-", ""))
                    .ToList();

                Assert.AreEqual(word.Split('-'), syllables);
            }
        }

        [Test]
        public void GetSyllables_InvalidWords_ThrowsArgumentException()
        {
            string[] words = { "ьм", "м'", "'м", "м'ят'я", "ёлка", "рыба", "объявление", "электрик" };

            foreach (string word in words)
            {
                Assert.Throws<ArgumentException>(() => this._syllablesService.GetSyllables(word.Replace("-", "")));
            }
        }

        [Test]
        public void GetSyllables_SecondRuleWords_ReturnsValidSyllables()
        {
            // Rule: Якщо поряд стоять два дзвінкі або два глухі звуки, то обидва належать до наступного складу
            string[] words = { "ща-стя", "ка-штан", "рі-чка", "пі-джак", "не-сти", "го-спо-дар", "мі-сткість" };

            foreach (string word in words)
            {
                IEnumerable<string> syllables = this._syllablesService.GetSyllables(word.Replace("-", ""))
                    .ToList();

                Assert.AreEqual(word.Split('-'), syllables);
            }
        }

        [Test]
        public void GetSyllables_SixthRuleWords_ReturnsValidSyllables()
        {
            // Rule: Якщо в слові є подовжені приголосні, то вони належать до наступного складу
            string[] words = { "по-чу-ття", "ста-ття" };

            foreach (string word in words)
            {
                IEnumerable<string> syllables = this._syllablesService.GetSyllables(word.Replace("-", ""))
                    .ToList();

                Assert.AreEqual(word.Split('-'), syllables);
            }
        }

        [Test]
        public void GetSyllables_ThirdRuleWords_ReturnsValidSyllables()
        {
            // Rule: Якщо з двох приголосних перший дзвінкий, а другий глухий, то вони належать до різних складів
            string[] words = { "бе-різ-ка", "буз-ко-вий", "швид-ко", "стеж-ка", "глиб-ше", "їдь-те" };

            foreach (string word in words)
            {
                IEnumerable<string> syllables = this._syllablesService.GetSyllables(word.Replace("-", ""))
                    .ToList();

                Assert.AreEqual(word.Split('-'), syllables);
            }
        }

        [Test]
        public void GetSyllables_WordIsNullOrEmptyOrWhiteSpace_ReturnsNoSyllables()
        {
            Assert.IsEmpty(this._syllablesService.GetSyllables(null));
            Assert.IsEmpty(this._syllablesService.GetSyllables(string.Empty));
            Assert.IsEmpty(this._syllablesService.GetSyllables("  "));
        }

        [Test]
        public void GetSyllables_WordsWithApostrophe_ReturnsValidSyllables()
        {
            string[] words = { "де-м'ян", "м'я-та", "тор-ф'я-ний", "здо-ро-в'я", "з'їзд", "зв'я-зок", "сі-м'я" };

            foreach (string word in words)
            {
                IEnumerable<string> syllables = this._syllablesService.GetSyllables(word.Replace("-", ""))
                    .ToList();

                Assert.AreEqual(word.Split('-'), syllables);
            }
        }

        [Test]
        public void GetSyllables_WordsWithSoftSign_ReturnsValidSyllables()
        {
            string[] words = { "мідь", "гедзь", "кіль-це", "у-кра-їн-ська", "луцьк", "буль-йон", "тьо-хка-ти" };

            foreach (string word in words)
            {
                IEnumerable<string> syllables = this._syllablesService.GetSyllables(word.Replace("-", ""))
                    .ToList();

                Assert.AreEqual(word.Split('-'), syllables);
            }
        }

        [SetUp]
        public void Init()
        {
            this._syllablesService = new SyllablesService();
        }
    }
}