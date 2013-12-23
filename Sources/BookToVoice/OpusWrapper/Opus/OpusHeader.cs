using System;
using System.Linq;
using OpusWrapper.Opus.Enums;
using OpusWrapper.Opus.Presets;

namespace OpusWrapper.Opus
{
    public class OpusHeader
    {
        public byte[] Packet;

        /// <summary>1.[0-7] capture_pattern:
        /// a 8 Byte field that signifies the beginning of a page. It contains "OpusHead"
        ///  </summary>
        public byte[] CapturePattern
        {
            get { return Packet.Take(8).ToArray(); }
        }

        /// <summary> 2.[8] version:
        /// 1 Byte signifying the version number of the Opus file format used in this stream (this document specifies version 1).
        /// </summary>
        public byte Version
        {
            get { return Packet[8]; }
            private set { Packet[8] = value; }
        }

        /// <summary>3.[9] Channels count:
        /// 1 Byte signifying the Number of channels: 1..255
        /// </summary>
        public byte Channels
        {
            get { return Packet[9]; }
            private set { Packet[9] = value; }
        }

        /// <summary>4.[10-11] PreSkip 
        /// a 2 Byte field that signifies the ..
        /// </summary>
        public UInt16 PreSkip
        {
            get { return BitConverter.ToUInt16(Packet, 10); }
            private set { Array.Copy(BitConverter.GetBytes(value), 0, Packet, 10, 2); }
        }

        /// <summary>5.[12-15] InputSampleRate 
        /// a 4 Byte field that signifies the ..
        /// </summary>
        public UInt32 InputSampleRate
        {
            get { return BitConverter.ToUInt32(Packet, 12); }
            private set { Array.Copy(BitConverter.GetBytes(value), 0, Packet, 12, 4); }
        }

        /// <summary>6.[16-17] Gain 
        /// a 2 Byte field that signifies the ..
        /// in dB S7.8 should be zero whenever possible
        /// </summary>
        public UInt16 Gain
        {
            get { return BitConverter.ToUInt16(Packet, 16); }
            private set { Array.Copy(BitConverter.GetBytes(value), 0, Packet, 16, 2); }
        }

        /// <summary>7.[18] ChannelMapping:
        /// 1 Byte signifying the ..
        /// </summary>
        public byte ChannelMapping
        {
            get { return Packet[18]; }
            private set { Packet[18] = value; }
        }

        /// <summary>8.[19] StreamsCount:
        /// 1 Byte signifying the ..
        /// used if channel_mapping != 0
        /// </summary>
        public byte StreamsCount
        {
            get { return Packet[19]; }
            private set { Packet[19] = value; }
        }

        /// <summary>9.[20] CoupledStreamsCount:
        /// 1 Byte signifying the ..
        /// used if channel_mapping != 0
        /// </summary>
        public byte CoupledStreamsCount
        {
            get { return Packet[20]; }
            private set { Packet[20] = value; }
        }

        /// <summary>9.[21..*255] CoupledStreamsCount:
        /// 0-255 Byte signifying the ..
        /// </summary>
        public byte[] StreamMap
        {
            get { return Packet.Skip(21).ToArray(); }
            private set { Array.Copy(value, 0, Packet, 21, value.Length); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="samplingRate"></param>
        /// <param name="preskip"></param>
        public OpusHeader(byte channels, SamplingRate samplingRate, byte preskip)
            : this(channels, channels > 8 ? MappingFamily.Multy : MappingFamily.Vorbis, samplingRate, preskip, 0, 1) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="mappingFamily"></param>
        /// <param name="samplingRate"></param>
        /// <param name="preskip"></param>
        /// <param name="gain">0 dB gain is recommended unless you know what you're doing</param>
        /// <param name="version"></param>
        public OpusHeader(byte channels, MappingFamily mappingFamily, SamplingRate samplingRate, byte preskip, UInt16 gain, byte version)
        {
            Packet = new byte[21 + channels];

            const string keyTemplate = "OpusHead";
            for (int index = 0; index < keyTemplate.Length; index++)
            {
                Packet[index] = (byte)keyTemplate[index];
            }

            Version = version;
            Channels = channels;
            PreSkip = preskip;
            InputSampleRate = (UInt32)samplingRate;
            Gain = gain;
            ChannelMapping = (byte)mappingFamily;


            if (mappingFamily == MappingFamily.SingleStream || mappingFamily == MappingFamily.Vorbis)
            {
                var template = ChanelTemplateCollection.GetTemplate(channels - 1);
                StreamsCount = template.StreamsCount;
                CoupledStreamsCount = template.CoupledStreamsCount;
                StreamMap = template.Mapping;
            }
            else
            {
                StreamsCount = channels;
                StreamMap = Enumerable.Range(0, channels).Select(b => (byte)b).ToArray();
            }
        }
    }
}
