namespace TextToSpeech.Services.PlaybackService
{
    using System;
    using System.Collections.Generic;

    internal interface IPlaybackService
    {
        event EventHandler PlaybackCompleted;

        bool CanInit();

        bool CanPlay();

        bool CanStop();

        void Init(IEnumerable<string> words);

        void Play();

        void Stop();
    }
}