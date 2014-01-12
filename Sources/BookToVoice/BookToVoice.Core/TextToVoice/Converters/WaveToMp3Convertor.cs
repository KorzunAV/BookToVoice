using System.IO;
using System.Linq;
using NAudio.Lame;
using NAudio.Wave;
using NLog;
using OpusWrapper.Opus.Presets;

namespace BookToVoice.Core.TextToVoice.Converters
{
    public class WaveToMp3Convertor : BaseStreamConvertor
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly LameMP3FileWriter _mp3Writer;
        private static Stream _stream;

        public WaveToMp3Convertor(string fileName, Options options)
        {
            _stream = new FileStream(fileName + ".mp3", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            var waveFormat = new WaveFormat((int)options.InputSamplingRate.Value, options.InputeBits, options.InputChannels.Value);
            _mp3Writer = new LameMP3FileWriter(_stream, waveFormat, options.BitRate.Value);
        }


        protected override void ExecuteConvert(byte[] waveData)
        {
            _mp3Writer.Write(waveData.ToArray(), 0, waveData.Length);
        }

        #region IDisposable
        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                base.Dispose(disposing);
                // Dispose managed resources.
                _mp3Writer.Dispose();
                _stream.Flush();
                _stream.Dispose();
                // Note disposing has been done.
                _disposed = true;
            }
        }
        #endregion IDisposable
    }
}