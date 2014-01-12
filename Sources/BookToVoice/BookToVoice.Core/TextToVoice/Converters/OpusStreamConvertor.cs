using System;
using System.IO;
using OpusWrapper;
using OpusWrapper.Ogg;
using OpusWrapper.Opus;
using OpusWrapper.Opus.Presets;

namespace BookToVoice.Core.TextToVoice.Converters
{
    public class OpusStreamConvertor : BaseStreamConvertor
    {
        private readonly StreamState _streamState;
        private readonly Options _options;
        private readonly string _fileName;


        public OpusStreamConvertor(string fileName, Options options)
        {
            _fileName = fileName + ".opus";
            _options = options;
            _streamState = new StreamState(options);

            if (File.Exists(_fileName))
            {
                SetGranulePos(_streamState);
            }
            else
            {
                using (var fs = new FileStream(_fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    SetHeader(_streamState, fs);
                    SetComment(_streamState, fs);
                }
            }
        }


        protected override void ExecuteConvert(byte[] waveData)
        {
            _streamState.AddWaveData(waveData);
            var pageCount = _streamState.TotalPages();
            if (pageCount > 10)
            {
                using (var fs = new FileStream(_fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    _streamState.Flush(fs);
                }
            }
        }

        private void SetHeader(StreamState os, Stream stream)
        {
            UInt16 preskip = 0;

            var header = new OpusHeader(_options.OutChannels.Value, _options.OutSamplingRate, preskip);
            var op = new Packet
            {
                PacketData = header.Packet,
                PacketDataLength = header.Packet.Length,
                Bos = 1,
                Eos = 0,
                GranulePos = 0
            };
            os.AddWaveData(op);
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
                PacketDataLength = opusTags.GetPackedLength(),
                Bos = 0,
                Eos = 0,
                GranulePos = 0
            };
            os.AddWaveData(opComment);
            os.Flush(stream);
        }

        private void SetGranulePos(StreamState streamState)
        {
            const uint key = 0x5367674F;
            UInt32 buf = 0;
            var granulepos = new byte[8];
            var bitstreamSerialNumber = new byte[4];

            using (var fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                for (var i = fs.Length - 1; i > -1; i--)
                {
                    fs.Position = i;
                    var sByte = fs.ReadByte();
                    buf = (buf << 8) + (byte)sByte;
                    if (buf == key)
                    {
                        fs.Position = i + 6;
                        for (int j = 0; j < 8; j++)
                        {
                            granulepos[j] = (byte)fs.ReadByte();
                        }
                        for (int j = 0; j < 4; j++)
                        {
                            bitstreamSerialNumber[j] = (byte)fs.ReadByte();
                        }
                        break;
                    }
                }
            }
            streamState.GranulePos = BitConverter.ToUInt64(granulepos, 0);
            streamState.SerialNo = BitConverter.ToUInt32(bitstreamSerialNumber, 0);
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
                using (var fs = new FileStream(_fileName, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    _streamState.Flush(fs);
                }
                // Note disposing has been done.
                _disposed = true;
            }
        }
        #endregion IDisposable
    }
}
