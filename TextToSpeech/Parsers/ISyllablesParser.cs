namespace TextToSpeech.Parsers
{
    using System.Collections.Generic;

    public interface ISyllablesParser
    {
        IEnumerable<string> GetSyllables(string word);
    }
}