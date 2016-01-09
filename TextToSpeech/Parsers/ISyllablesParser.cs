namespace TextToSpeech.Parsers
{
    using System.Collections.Generic;

    internal interface ISyllablesParser
    {
        IEnumerable<string> GetSyllables(string word);
    }
}