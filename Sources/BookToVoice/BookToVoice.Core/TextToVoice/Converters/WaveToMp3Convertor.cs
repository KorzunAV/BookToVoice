using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NAudio.Lame;
using NAudio.Wave;
using NLog;

namespace BookToVoice.Core.TextToVoice.Converters
{
    public class WaveToMp3Convertor : IFormatConverter
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static readonly object Synk = new object();
        private static readonly Queue<byte[]> DataQueue = new Queue<byte[]>();
        private static int _bitRate;
        private static WaveFormat _waveFormat;
        private static Stream _stream;
        private Thread _thread;
        private double t1 = 0;
        private double t11 = 0;
        private static double t2 = 0;

        private bool _disposed;

        public WaveToMp3Convertor(int bitRate, WaveFormat waveFormat, Stream stream)
        {
            _bitRate = bitRate;
            _waveFormat = waveFormat;
            _stream = stream;
        }

        public void ConvertAsyn(byte[] waveData)
        {
            var time1 = DateTime.Now;
            lock (Synk)
            {
                DataQueue.Enqueue(waveData);

                if (_thread == null || !_thread.IsAlive)
                {
                    _thread = new Thread(Convert);
                    _thread.Start();
                }
            }
            var time2 = DateTime.Now;
            t1 += (time2 - time1).TotalMilliseconds;

            var time11 = DateTime.Now;
            //TODO:Find another approach for saving memory
            if (DataQueue.Count > 10)
            {
                _thread.Join(TimeSpan.FromSeconds(10));
            }
            var time21 = DateTime.Now;
            t11 += (time21 - time11).TotalMilliseconds;
        }

        private static void Convert()
        {
            var time1 = DateTime.Now;
            using (var mp3Writer = new LameMP3FileWriter(_stream, _waveFormat, _bitRate))
            {
                while (DataQueue.Count > 0)
                {
                    byte[] waveData;
                    lock (Synk)
                    {
                        waveData = DataQueue.Dequeue();
                    }
                    mp3Writer.Write(waveData, 0, waveData.Length);
                }
            }
            var time2 = DateTime.Now;
            t2 += (time2 - time1).TotalMilliseconds;
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.

                    if (_thread != null && _thread.IsAlive)
                    {
                        _thread.Join();
                    }
                    Log.Info(string.Format("t1={0}", t1));
                    Log.Info(string.Format("t11={0}", t11));
                    Log.Info(string.Format("t2={0}", t2));
                }
                // Note disposing has been done.
                _disposed = true;
            }
        }
        #endregion IDisposable
    }
}