using System;
using System.IO;
using NUnit.Framework;
using OpusWrapper.Opus;
using OpusWrapper.Opus.Enums;
using OpusWrapper.Opus.Presets;

namespace OpusWrapper.Test
{

    [TestFixture]
    public class OpusEncoderTest
    {
        [Test]
        public void EncodeTest()
        {
            const string filename = "test.stream";
            int segmentFrames = 960;
            var encoder = OpusEncoder.Create(SamplingRate.Create(SamplingRate.Template.Sampling48000), (byte)Channels.Template.Mono, ApplicationType.Voip);
            encoder.Bitrate = 8192;
            int bytesPerSegment = encoder.FrameByteCount(segmentFrames);

            using (var fs = new FileStream(filename, FileMode.Open))
            {
                var l = fs.Length;

                var max = (int)(l);
                var buf = new byte[max];
                int len;

                do
                {
                    len = fs.Read(buf, 0, max);
                    if (len > 0)
                    {
                        Encode(encoder, buf, len, bytesPerSegment);
                    }
                } while (len != 0);
            }
        }

        byte[] _notEncodedBuffer = new byte[0];
        private void Encode(OpusEncoder encoder, byte[] waveData, int count, int bytesPerSegment)
        {
            var soundBuffer = new byte[count + _notEncodedBuffer.Length];
            for (int i = 0; i < _notEncodedBuffer.Length; i++)
            {
                soundBuffer[i] = _notEncodedBuffer[i];
            }
            for (int i = 0; i < count; i++)
            {
                soundBuffer[i + _notEncodedBuffer.Length] = waveData[i];
            }

            int byteCap = bytesPerSegment;
            int segmentCount = (int)Math.Floor((decimal)soundBuffer.Length / byteCap);
            int segmentsEnd = segmentCount * byteCap;
            int notEncodedCount = soundBuffer.Length - segmentsEnd;
            _notEncodedBuffer = new byte[notEncodedCount];
            for (int i = 0; i < notEncodedCount; i++)
            {
                _notEncodedBuffer[i] = soundBuffer[segmentsEnd + i];
            }

            for (int i = 0; i < segmentCount; i++)
            {
                var segment = new byte[byteCap];
                for (int j = 0; j < segment.Length; j++)
                {
                    segment[j] = soundBuffer[(i * byteCap) + j];
                }
                byte[] buff = encoder.Encode(segment, segment.Length);
            }
        }

        [Test]
        public void Test()
        {
            byte[] intBytes2 = new byte[] { 0x34, 0x68, 0x00, 0x00 };

            var rez = BitConverter.ToInt32(intBytes2, 0);

            int intValue = 4;
            byte[] intBytes = BitConverter.GetBytes(intValue);
            var rez2 = BitConverter.ToInt32(intBytes, 0);
            Array.Reverse(intBytes);
        }
    }
}
