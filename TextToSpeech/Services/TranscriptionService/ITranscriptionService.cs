namespace TextToSpeech.Services.TranscriptionService
{
    public interface ITranscriptionService
    {
        string GetTranscription(string word);
    }
}