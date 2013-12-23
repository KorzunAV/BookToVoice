using System;

namespace BookToVoice.Core.TextToVoice.Converters
{
    interface IFormatConverter : IDisposable
    {
        void ConvertAsyn(byte[] waveData);
    }
}
