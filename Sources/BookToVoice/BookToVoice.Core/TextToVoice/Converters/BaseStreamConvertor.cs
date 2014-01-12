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
        readonly EventWaitHandle _eventWaitHandle = new AutoResetEvent(false);
        private bool _disposing;
        private readonly Thread _thread;
        private double _totalConvertTime;
        private UInt64 _totalConvertCount;


        public int DataQueueCount
        {
            get
            {
                if (DataQueue == null)
                {
                    return 0;
                }
                return DataQueue.Count;
            }
        }
    


        protected BaseStreamConvertor()
        {
            _thread = new Thread(Convert);
            _thread.Start();
        }

        public void ConvertAsyn(byte[] waveData)
        {
            lock (Synk)
            {
                DataQueue.Enqueue(waveData);
            }
            _eventWaitHandle.Set();
            if (DataQueueCount > 20)
            {
                _thread.Join((int)(20 * _totalConvertTime / _totalConvertCount));
            }
        }         
          
        public void Pause(int millisecondsTimeout)
        {
            _thread.Join(millisecondsTimeout);
        }

        private void Convert()
        {
            do
            {
            while (DataQueue.Count > 0)
            {
                    _totalConvertCount++;
                    var time1 = DateTime.Now;
                    byte[] waveData;
                lock (Synk)
                {
                    waveData = DataQueue.Dequeue();
                }
                ExecuteConvert(waveData);
                    var time2 = DateTime.Now;
                    _totalConvertTime += (time2 - time1).TotalMilliseconds;
            }

                _eventWaitHandle.WaitOne();
            } while (!_disposing);
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
                _disposing = true;
                _eventWaitHandle.Set();

                if (_thread != null && _thread.IsAlive)
                {
                    _thread.Join();
                }
                Log.Info(string.Format("BaseStreamConvertor total convert time ={0}, total convert count ={1}", _totalConvertTime, _totalConvertCount));
                // Note disposing has been done.
                _disposed = true;
            }
        }
        #endregion IDisposable
    }
}
