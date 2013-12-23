using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OpusWrapper.Ogg;
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
            var header = new OpusHeader(chan, SamplingRate.Sampling48000, 0);

            Assert.IsTrue(header.CapturePattern.SequenceEqual("OpusHead".Select(c => (byte)c).ToArray()));
            Assert.IsTrue(header.Version == 1);
            Assert.IsTrue(header.Channels == chan);
            Assert.IsTrue(header.PreSkip == 0);
            Assert.IsTrue(header.InputSampleRate == (UInt32)SamplingRate.Sampling48000);
            Assert.IsTrue(header.Gain == 0);
            Assert.IsTrue(header.ChannelMapping == (byte)MappingFamily.Vorbis);
            var teplate = ChanelTemplateCollection.GetTemplate(header.Channels - 1);
            Assert.IsTrue(header.StreamsCount == teplate.StreamsCount);
            Assert.IsTrue(header.CoupledStreamsCount == teplate.CoupledStreamsCount);
            Assert.IsTrue(header.StreamMap.SequenceEqual(teplate.Mapping));
        }

        [Test]
        public void SaveToFileTest()
        {
            using (var fs = new FileStream("Header.opus", FileMode.Create))
            {
                var os = new StreamState(123456);

                SetHeader(os, fs);
                SetComment(os, fs);
                SetData(os, fs);
            }
        }

        private void SetHeader(StreamState os, Stream stream)
        {
            byte chan = 1;
            int coding_rate = 48000;
            byte preskip = 90;// inopt.skip * (48000 / coding_rate);

            var header = new OpusHeader(chan, SamplingRate.Sampling48000, preskip);
            var op = new Packet
            {
                PacketData = header.Packet,
                Bytes = header.Packet.Length,
                Bos = 1,
                Eos = 0,
                Granulepos = 0,
                PacketNo = 0
            };

            os.Packetin(op);
            os.Flush(stream);
        }

        private void SetComment(StreamState os, Stream stream)
        {
            var opusTags = new OpusTags();
            opusTags.Add("ENCODER", "opusenc from libopus 1.0.1-21-gff16ab0");
            opusTags.Pad();

            var opComment = new Packet
            {
                PacketData = opusTags.GetPacked(),
                Bytes = opusTags.GetPackedLength(),
                Bos = 0,
                Eos = 0,
                Granulepos = 0,
                PacketNo = 1
            };
            os.Packetin(opComment);
            os.Flush(stream);
        }

        private void SetData(StreamState os, Stream stream)
        {
            //int id = 0;
            //int original_samples = 0;
            //int rate = 48000;
            //int coding_rate = 48000;
            //Int64 enc_granulepos = 0;

            //int cur_frame_size = frame_size;

            //enc_granulepos += cur_frame_size * 48000 / coding_rate;

            //var opData = new Packet
            //{
            //    PacketData = packet,
            //    Bytes = packet.Length,
            //    Bos = 0,
            //    Granulepos = enc_granulepos,
            //    PacketNo = 2 + id
            //};
            //if (opData.Eos != 0)
            //{
            //    //We compute the final GP as ceil(len*48k/input_rate). When a resampling
            //    //decoder does the matching floor(len*input/48k) conversion the length will
            //    //be exactly the same as the input.
            //    opData.Granulepos = ((original_samples*48000 + rate - 1)/rate) + 90; // header.PreSkip;
            //}
            //os.Packetin(opData);
        }

    }
}