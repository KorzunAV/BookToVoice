using System;
using System.Collections.Generic;

namespace BookToVoice.Core.TextToVoice.Strategies
{
    interface ITextToVoiceStrategy : IDisposable
    {
        void Execute(TextToVoiceModel model, string voiceName);
        IEnumerable<string> GetVoiceNames();
    }
}
