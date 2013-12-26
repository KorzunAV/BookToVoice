using OpusWrapper.Opus.Enums;

namespace OpusWrapper.Opus.Presets
{
    public class Options
    {
        private FrameSize _frameSize;
        private BitRate _bitRate;
        private BitRateEncoding _bitRateEncoding;
        private SamplingRate _inputSamplingRate;
        private SamplingRate _outSamplingRate;
        private Channels _inputChannels;
        private Channels _outChannels;
        private ApplicationType _applicationType = ApplicationType.Voip;

        public FrameSize FrameSize
        {
            get { return _frameSize ?? (_frameSize = FrameSize.Create(FrameSize.Template.Ms20)); }
            set { _frameSize = value; }
        }

        public BitRate BitRate
        {
            get { return _bitRate ?? (_bitRate = BitRate.Create(24000)); }
            set { _bitRate = value; }
        }

        public BitRateEncoding BitRateEncoding
        {
            get { return _bitRateEncoding ?? (_bitRateEncoding = BitRateEncoding.Create(BitRateEncoding.Template.Vbr)); }
            set { _bitRateEncoding = value; }
        }

        public SamplingRate InputSamplingRate
        {
            get
            {
                return _inputSamplingRate ??
                       (_inputSamplingRate = SamplingRate.Create(SamplingRate.Template.Sampling24000));
            }
            set { _inputSamplingRate = value; }
        }

        public SamplingRate OutSamplingRate
        {
            get { return _outSamplingRate ?? (_outSamplingRate = SamplingRate.Create(22050)); }
            set { _outSamplingRate = value; }
        }

        public Channels InputChannels
        {
            get { return _inputChannels ?? (_inputChannels = Channels.Create(Channels.Template.Mono)); }
            set { _inputChannels = value; }
        }

        public Channels OutChannels
        {
            get { return _outChannels ?? (_outChannels = Channels.Create(Channels.Template.Mono)); }
            set { _outChannels = value; }
        }

        public ApplicationType ApplicationType
        {
            get { return _applicationType; }
            set { _applicationType = value; }
        }
    }
}