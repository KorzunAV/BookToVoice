using System.Globalization;

namespace OpusWrapper.Opus.Presets
{
    /// <summary>
    /// Opus supports Bit-rates from 6 kb/s to 510 kb/s
    /// </summary>
    public class BitRate : BaseOption<int>
    {
        private readonly int _value;
      
        public override string OptionName
        {
            get { return "bitrate";}
        }

        public override string InfoValue
        {
            get { return (_value / 1000).ToString(CultureInfo.InvariantCulture); }
        }
        
        public override int Value
        {
            get { return _value; }
        }


        private BitRate(int bitRate)
        {
            _value = bitRate;
        }


        public static BitRate Default()
        {
            return Create(24000);
        }

        public static BitRate Create(int bitRate)
        {
            if (bitRate >= 6 && bitRate <= 510)
            {
                return new BitRate(bitRate * 1000);
            }

            if (bitRate >= 6000 && bitRate <= 510000)
            {
                return new BitRate(bitRate);
            }
            return Default();
        }
    }
}
