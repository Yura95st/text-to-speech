namespace TextToSpeech
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using TextToSpeech.Services.SyllablesService;
    using TextToSpeech.Services.TranscriptionService;
    using TextToSpeech.Services.WordVoiceService;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            //Assembly assembly = Assembly.GetExecutingAssembly();
            //var list = assembly.GetManifestResourceNames().ToList();
            //list.Sort();

            //var sb =new StringBuilder(200);
            //foreach (string manifestResourceName in list)
            //{
            //    var nameParts = manifestResourceName.Split('.');

            //    sb.Append(string.Format("\"{0}\", ", nameParts[nameParts.Length - 2]));
            //}

            ISyllablesService syllablesService = new SyllablesService();
            ITranscriptionService transcriptionService = new TranscriptionService();
            IWordVoiceService wordVoiceService = new WordVoiceService();

            string str =
                "річці принісши зшиток вивізши зжовклий зчепити безчинство джемом намажся вивчишся озвучся доріжці чашці підживити підземний"
                    + "відспівати відцвісти відчеканити підшити братство учіться коритце вотчина багатшати студентство  кореспондентський туристський "
                    + "артистці невістчин шістдесят шістсот поїздці ";

            foreach (string word in str.Split(' '))
            {
                IEnumerable<string> syllables = syllablesService.GetSyllables(word)
                    .ToList();

                string wordTranscription = transcriptionService.GetTranscription(str);
                wordVoiceService.PlayWord(wordTranscription);
            }
        }
    }
}