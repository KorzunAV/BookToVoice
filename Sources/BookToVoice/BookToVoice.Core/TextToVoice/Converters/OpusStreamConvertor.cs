using System;
using System.IO;
using OpusWrapper.Ogg;
using OpusWrapper.Opus;
using OpusWrapper.Opus.Presets;

namespace BookToVoice.Core.TextToVoice.Converters
{
    public class OpusStreamConvertor : BaseStreamConvertor
    {
        private readonly StreamState _streamState;
        private readonly OpusEncoder _encoder;
        private readonly Stream _stream;
        private readonly Options _options;
        readonly int _bytesPerSegment;
        private byte[] _notEncodedBuffer = new byte[0];

        public OpusStreamConvertor(string fileName, Options options)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = string.Format("{0:d_M_yyy_HH_mm_ss}.opus", DateTime.Now);
            }
            else if (!fileName.EndsWith(".opus"))
            {
                fileName += ".opus";
            }

            _options = options;

            var fi = new FileInfo(fileName);

            var random = new Random(DateTime.Now.Minute + DateTime.Now.Second);
            _streamState = new StreamState((uint)random.Next(0, int.MaxValue));

            _encoder = OpusEncoder.Create(options.InputSamplingRate, options.InputChannels.Value, options.ApplicationType);
            _encoder.Bitrate = _options.BitRate.Value;

            _bytesPerSegment = _encoder.FrameByteCount(options.FrameSize.Value);

            if (!fi.Exists)
            {
                _stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                SetHeader(_streamState, _stream);
                SetComment(_streamState, _stream);
            }
            else
            {
                _stream = new FileStream(fileName, FileMode.Open, FileAccess.Write, FileShare.Read);
            }
        }


        protected override void ExecuteConvert(byte[] waveData)
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

            for (uint i = 0; i < segmentCount; i++)
            {
                var segment = new byte[byteCap];
                for (int j = 0; j < segment.Length; j++)
                {
                    segment[j] = soundBuffer[(i * byteCap) + j];
                }
                byte[] buff = _encoder.Encode(segment, segment.Length);

                SetData(_streamState, buff, i);
            }
        }

        private void SetHeader(StreamState os, Stream stream)
        {
            byte preskip = 144;// inopt.skip * (48000 / coding_rate);

            var header = new OpusHeader(_options.OutChannels.Value, _options.OutSamplingRate, preskip);
            var op = new Packet
            {
                PacketData = header.Packet,
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
            opusTags.AddOption(_options.BitRate);
            opusTags.AddOption(_options.BitRateEncoding);
            opusTags.AddOption("ignorelength", string.Empty);

            opusTags.Pad();

            var opComment = new Packet
            {
                PacketData = opusTags.GetPacked(),
                Bos = 0,
                Eos = 0,
                Granulepos = 0,
                PacketNo = 1
            };
            os.Packetin(opComment);
            os.Flush(stream);
        }

        private void SetData(StreamState os, byte[] packet, UInt64 idPacket)
        {
            //UInt64 original_samples = 0;
            //UInt64 rate = 48000;
            //UInt64 coding_rate = 48000;
            //UInt64 enc_granulepos = 0;

            //UInt64 cur_frame_size = 48000;

            //enc_granulepos += cur_frame_size * 48000 / coding_rate;

            var opData = new Packet
            {
                PacketData = packet,
                Bos = 0,
                Granulepos = idPacket * (ushort)_options.OutSamplingRate.Value,
                PacketNo = 2 + idPacket
            };
            os.Packetin(opData);

            var pageCount = os.TotalPages();
            if (pageCount > 1)
            {
                os.Flush(_stream);
            }
        }

        #region IDisposable
        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                base.Dispose(disposing);
                // Dispose managed resources.
                _streamState.Eos = true;
                _streamState.Flush(_stream);
                _stream.Flush();
                _stream.Dispose();
                // Note disposing has been done.
                _disposed = true;
            }
        }
        #endregion IDisposable

    }
}
