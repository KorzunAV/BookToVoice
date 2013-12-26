namespace OpusWrapper.Opus.Presets
{
    public class BitRateEncoding : BaseOption<byte>
    {
        private readonly byte _value;

        private readonly string[] _names = new[] { "vbr", "cvbr", "hard-cbr" };
        public enum Template
        {
            Vbr,
            Cvbr,
            HardCbr
        }


        public override string OptionName
        {
            get { return _names[_value]; }
        }

        public override string InfoValue
        {
            get { return string.Empty; }
        }

        public override byte Value
        {
            get { return _value; }
        }


        private BitRateEncoding(Template encode)
        {
            _value = (byte)encode;
        }


        public static BitRateEncoding Default()
        {
            return Create(Template.Vbr);
        }

        public static BitRateEncoding Create(Template encode)
        {
            return new BitRateEncoding(encode);
        }
    }
}
