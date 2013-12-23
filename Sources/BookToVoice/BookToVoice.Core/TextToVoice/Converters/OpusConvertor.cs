using System;
using System.IO;
using OpusWrapper.Ogg;
using OpusWrapper.Opus;
using OpusWrapper.Opus.Enums;

namespace BookToVoice.Core.TextToVoice.Converters
{
    public class OpusConvertor : IFormatConverter
    {
        private SamplingRate _outSamplingRate;
        private Channels _outChannels;
        private OpusEncoder _encoder;
        int _segmentFrames;

        int _bytesPerSegment;

        byte[] _notEncodedBuffer = new byte[0];
        private StreamState StreamState;
        private Stream _stream;

        public OpusConvertor()
            : this(SamplingRate.Sampling24000, Channels.Mono, SamplingRate.Sampling48000, Channels.Mono, ApplicationType.Voip,
            8192, 960) { }


        public OpusConvertor(SamplingRate inputSamplingRate, Channels inputChannels, SamplingRate outSamplingRate, Channels outChannels, ApplicationType application,
            int bitrate, int segmentFrames)
        {
            _encoder = OpusEncoder.Create(inputSamplingRate, inputChannels, application);
            _encoder.Bitrate = bitrate;

            _segmentFrames = segmentFrames;
            _bytesPerSegment = _encoder.FrameByteCount(_segmentFrames);

            _outSamplingRate = outSamplingRate;
            _outChannels = outChannels;
        }

        public void ConvertAsyn(byte[] waveData)
        {
            var soundBuffer = new byte[waveData.Length + _notEncodedBuffer.Length];
            Array.Copy(_notEncodedBuffer, soundBuffer, _notEncodedBuffer.Length);
            Array.Copy(waveData, 0, soundBuffer, _notEncodedBuffer.Length, waveData.Length);
            
            int byteCap = _bytesPerSegment;
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
                int len;
                byte[] buff = _encoder.Encode(segment, segment.Length, out len);

                SetData(StreamState, _stream, buff, len, i);
                //outStream.Write(buff, 0, len);
                //var buff2 = new byte[len];
                //for (int j = 0; j < len; j++)
                //{
                //    buff2[j] = buff[j];
                //}
                //var buff3 = _decoder.Decode(buff2, len, out len);
                //_playBuffer.AddSamples(buff, 0, len);
            }
            StreamState.Flush(_stream);
        }

        public void SetOutStream(Stream stream)
        {
            _stream = stream;
            var random = new Random(DateTime.Now.Minute + DateTime.Now.Second);
            StreamState = new StreamState((uint)random.Next(0, int.MaxValue));
            SetHeader(StreamState, stream);
            SetComment(StreamState, stream);
        }

        private void SetHeader(StreamState os, Stream stream)
        {
            byte preskip = 90;// inopt.skip * (48000 / coding_rate);

            var header = new OpusHeader((byte)_outChannels, _outSamplingRate, preskip);
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

        private void SetData(StreamState os, Stream stream, byte[] packet, int packetLen, int idPacket)
        {
            int original_samples = 0;
            int rate = 48000;
            int coding_rate = 48000;
            Int64 enc_granulepos = 0;

            int cur_frame_size = 48000;

            enc_granulepos += cur_frame_size * 48000 / coding_rate;

            var opData = new Packet
            {
                PacketData = packet,
                Bytes = packetLen,
                Bos = 0,
                Granulepos = enc_granulepos,
                PacketNo = 2 + idPacket
            };
            if (opData.Eos != 0)
            {
                //We compute the final GP as ceil(len*48k/input_rate). When a resampling
                //decoder does the matching floor(len*input/48k) conversion the length will
                //be exactly the same as the input.
                opData.Granulepos = ((original_samples * 48000 + rate - 1) / rate) + 90; // header.PreSkip;
            }
            os.Packetin(opData);
        }

        #region IDisposable
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.

                }
                // Note disposing has been done.
                disposed = true;
            }
        }
        #endregion IDisposable
    }
}
