using System;

namespace OpusWrapper.Opus.Presets
{
    public class SamplingRate : BaseOption<uint>
    {
        private readonly uint _value;

        public enum Template : ushort
        {
            Sampling08000 = 8000,
            Sampling12000 = 12000,
            Sampling16000 = 16000,
            Sampling24000 = 24000,
            Sampling48000 = 48000
        }


        public override string OptionName
        {
            get { throw new NotImplementedException(); }
        }

        public override string InfoValue
        {
            get { throw new NotImplementedException(); }
        }

        public override uint Value
        {
            get { return _value; }
        }
        
        private SamplingRate(uint samplingRate)
        {
            _value = samplingRate;
        }

        public static SamplingRate Default()
        {
            return Create((uint)Template.Sampling48000);
        }

        public static SamplingRate Create(Template samplingRate)
        {
            return Create((uint)samplingRate);
        }

        public static SamplingRate Create(uint samplingRate)
        {
            return new SamplingRate(samplingRate);
        }
    }
}
