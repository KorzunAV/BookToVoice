using OpusWrapper.Opus.Presets;

namespace BookToVoice.Core.TextToVoice.Converters
{
    public class ConvertorFactory
    {
        public enum SupportedType
        {
            Opus,
            Mp3
        }

        public static BaseStreamConvertor GetConverter(string fileName, Options options, SupportedType type)
        {
            switch (type)
            {
                case SupportedType.Opus :
                    {
                        return new OpusStreamConvertor(fileName, options);
                    }
                case SupportedType.Mp3:
                    {
                        return new WaveToMp3Convertor(fileName, options);
                    }
            }
            return null;
        }
    }
}
