using System;
using System.Collections.Generic;
using System.Threading;
using NLog;

namespace BookToVoice.Core.TextToVoice.Converters
{
    public abstract class BaseStreamConvertor : IFormatConverter
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static readonly object Synk = new object();
        protected static readonly Queue<byte[]> DataQueue = new Queue<byte[]>();

        private Thread _thread;

        private double t1 = 0;
        private double t11 = 0;
        private static double t2 = 0;
    

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

        private void Convert()
        {
            var time1 = DateTime.Now;

            while (DataQueue.Count > 0)
            {
                byte[] waveData;
                lock (Synk)
                {
                    waveData = DataQueue.Dequeue();
                }
                ExecuteConvert(waveData);
            }

            var time2 = DateTime.Now;
            t2 += (time2 - time1).TotalMilliseconds;
        }

        protected abstract void ExecuteConvert(byte[] waveData);

        #region IDisposable
        private bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_thread != null && _thread.IsAlive)
                {
                    _thread.Join();
                }
                Log.Info(string.Format("t1={0}", t1));
                Log.Info(string.Format("t11={0}", t11));
                Log.Info(string.Format("t2={0}", t2));
                // Note disposing has been done.
                _disposed = true;
            }
        }
        #endregion IDisposable

    }
}
