using System;
using System.Collections.Generic;

namespace BookToVoice.Core.TextToVoice.Converters
{
    public interface IFormatConverter : IDisposable
    {
        void ConvertAsyn(byte[] waveData);
    }
}
