using System;
using System.Linq;
using NUnit.Framework;
using OpusWrapper.Opus;
using OpusWrapper.Opus.Enums;
using OpusWrapper.Opus.Presets;

namespace OpusWrapper.Test.Opus
{
    [TestFixture]
    internal class OpusHeaderTest
    {
        [Test]
        public void Test()
        {
            byte chan = 1;
            var header = new OpusHeader(chan, SamplingRate.Create(SamplingRate.Template.Sampling48000), 0);

            Assert.IsTrue(header.CapturePattern.SequenceEqual("OpusHead".Select(c => (byte)c).ToArray()));
            Assert.IsTrue(header.Version == 1);
            Assert.IsTrue(header.Channels == chan);
            Assert.IsTrue(header.PreSkip == 0);
            Assert.IsTrue(header.InputSampleRate == (UInt32)SamplingRate.Template.Sampling48000);
            Assert.IsTrue(header.Gain == 0);
            Assert.IsTrue(header.ChannelMapping == (byte)MappingFamily.Vorbis);
            var teplate = ChanelTemplateCollection.GetTemplate(header.Channels - 1);
            Assert.IsTrue(header.StreamsCount == teplate.StreamsCount);
            Assert.IsTrue(header.CoupledStreamsCount == teplate.CoupledStreamsCount);
        }
    }
}