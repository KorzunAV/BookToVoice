using System.Globalization;

namespace OpusWrapper.Opus.Presets
{
    public class Channels : BaseOption<byte>
    {
        public enum Template : byte
        {
            /// <summary>
            /// 1 Channel
            /// </summary>
            Mono = 1,
            /// <summary>
            /// 2 Channels
            /// </summary>
            Stereo = 2
        }

        private readonly byte _value;

        public override string OptionName
        {
            get { return "raw-chan"; }
        }

        public override string InfoValue
        {
            get { return _value.ToString(CultureInfo.InvariantCulture); }
        }

        public override byte Value
        {
            get { return _value; }
        }


        private Channels(byte chanelsCount)
        {
            _value = chanelsCount;
        }
        

        public static Channels Default()
        {
            return Create(2);
        }

        public static Channels Create(byte chanelsCount)
        {
            return new Channels(chanelsCount);
        }

        public static Channels Create(Template chanelsCount)
        {
            return new Channels((byte)chanelsCount);
        }
    }
}
