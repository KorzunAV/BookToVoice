using System.Collections.Generic;

namespace OpusWrapper.Opus.Presets
{
    /// <summary>
    /// Opus supports frame sizes from 2.5 ms to 60 ms
    /// </summary>
    public class FrameSize : BaseOption<ushort>
    {
        private readonly ushort _value;

        public enum Template : ushort
        {
            Ms2P5 = 120,
            Ms5 = 240,
            Ms10 = 480,
            Ms20 = 960,
            Ms40 = 1920,
            Ms60 = 2880
        }

        public static Dictionary<ushort, string> Templates = new Dictionary<ushort, string>
            {
                {120, "2.5"},
                {240, "5"},
                {480, "10"},
                {960, "20"},
                {1920, "40"},
                {2880, "60"}
            };

        public override string OptionName
        {
            get { return "framesize"; }
        }

        public override string InfoValue
        {
            get { return Templates[_value]; }
        }

        public override ushort Value
        {
            get { return _value; }
        }


        private FrameSize(Template frameSize)
        {
            _value = (ushort)frameSize;
        }


        public static FrameSize Default()
        {
            return Create(Template.Ms20);
        }

        public static FrameSize Create(Template frameSize)
        {
            return new FrameSize(frameSize);
        }
    }
}
