namespace TextToSpeech
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using TextToSpeech.Parsers;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            string word = "дем'ян";

            ISyllablesParser syllablesParser = new SyllablesParser();

            IEnumerable<string> syllables = syllablesParser.GetSyllables(word)
                .ToList();
        }
    }
}