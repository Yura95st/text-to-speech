namespace TextToSpeech.Services.SyllablesService
{
    using System.Collections.Generic;

    public interface ISyllablesService
    {
        IEnumerable<string> GetSyllables(string word);
    }
}