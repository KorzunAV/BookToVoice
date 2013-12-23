using System;
using System.Collections.Generic;

namespace OpusWrapper.Opus.Presets
{
    /// <summary>
    /// Opus supports frame sizes from 2.5 ms to 60 ms
    /// </summary>
    public class FrameSize : BaseOption<int>
    {
        private static FrameSize _entity;
        private readonly string _key;

        public static Dictionary<string, int> Templates = new Dictionary<string, int>
            {
                {"2.5", 120},
                {"5", 240},
                {"10", 480},
                {"20", 960},
                {"40", 1920},
                {"60", 2880}
            };

        public override string OptionName
        {
            get { return "framesize"; }
        }

        public override string Key
        {
            get { return _key; }
        }

        public override int Value
        {
            get { return Templates[_key]; }
        }


        private FrameSize(string frameSizeInMs)
        {
            _key = frameSizeInMs;
        }


        public static FrameSize Default()
        {
            _entity = new FrameSize("20");
            return _entity;
        }

        public static FrameSize Create(string frameSizeInMs)
        {
            if (Templates.ContainsKey(frameSizeInMs))
            {
                _entity = new FrameSize("20");
                return _entity;
            }
            return Default();
        }
    }
}
