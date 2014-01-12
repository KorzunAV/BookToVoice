using System;
using OpusWrapper.Opus.Enums;
using OpusWrapper.Opus.Presets;

namespace OpusWrapper.Opus
{
    /// <summary>
    /// Opus codec wrapper.
    /// </summary>
    public class OpusEncoder : IDisposable
    {
        private readonly uint _maxDataBytes;
        private readonly byte _inputChannels;
        private IntPtr _encoder;

        #region properties

        /// <summary>
        /// Gets or sets the bitrate setting of the encoding.
        /// </summary>
        public int Bitrate
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("OpusEncoder");
                int bitrate;
                var ret = Api.opus_encoder_ctl(_encoder, CtlGetRequest.Bitrate, out bitrate);
                if (ret < 0)
                    throw new Exception("Encoder error - " + ((ErrorCode)ret).ToString());
                return bitrate;
            }
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("OpusEncoder");
                var ret = Api.opus_encoder_ctl(_encoder, CtlSetRequest.Bitrate, value);
                if (ret < 0)
                    throw new Exception("Encoder error - " + ((ErrorCode)ret).ToString());
            }
        }

        /// <summary>
        /// Gets or sets whether Forward Error Correction is enabled.
        /// </summary>
        public bool ForwardErrorCorrection
        {
            get
            {
                if (_encoder == IntPtr.Zero)
                    throw new ObjectDisposedException("OpusEncoder");

                int fec;
                int ret = Api.opus_encoder_ctl(_encoder, CtlGetRequest.InbandFec, out fec);
                if (ret < 0)
                    throw new Exception("Encoder error - " + ((ErrorCode)ret).ToString());

                return fec > 0;
            }

            set
            {
                if (_encoder == IntPtr.Zero)
                    throw new ObjectDisposedException("OpusEncoder");

                var ret = Api.opus_encoder_ctl(_encoder, CtlSetRequest.InbandFec, value ? 1 : 0);
                if (ret < 0)
                    throw new Exception("Encoder error - " + ((ErrorCode)ret).ToString());
            }
        }

        #endregion properties

        private OpusEncoder(IntPtr encoder, byte inputChannels, uint maxDataBytes)
        {
            _encoder = encoder;
            _maxDataBytes = maxDataBytes;
            _inputChannels = inputChannels;
        }

        /// <summary>
        /// Creates a new Opus encoder.
        /// </summary>
        /// <param name="inputSamplingRate">Sampling rate of the input signal (Hz). This must be one of 8000, 12000, 16000, 24000, or 48000.</param>
        /// <param name="inputChannels">Number of channels (1 or 2) in input signal.</param>
        /// <param name="application">Coding mode.</param>
        /// <returns>A new <c>OpusEncoder</c></returns>
        public static OpusEncoder Create(SamplingRate inputSamplingRate, byte inputChannels, ApplicationType application)
        {
            IntPtr error;
            IntPtr encoder = Api.opus_encoder_create((int)inputSamplingRate.Value, inputChannels, (int)application, out error);
            if ((ErrorCode)error != ErrorCode.OK)
            {
                throw new Exception("Exception occured while creating encoder");
            }
            return new OpusEncoder(encoder, inputChannels, 4000);
        }

        /// <summary>
        /// Produces Opus encoded audio from PCM samples.
        /// </summary>
        /// <param name="inputPcmSamples">PCM samples to encode.</param>
        /// <param name="sampleLength">How many bytes to encode.</param>
        /// <param name="encoded">Opus encoded audio buffer.</param>
        /// <returns>Opus encoded audio buffer length.</returns>
        public unsafe int Encode(byte[] inputPcmSamples, uint sampleLength, out byte[] encoded)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("OpusEncoder");
            }

            int frames = FrameCount(inputPcmSamples);
            encoded = new byte[_maxDataBytes];
            int length;
            fixed (byte* benc = encoded)
            {
                var encodedPtr = new IntPtr(benc);
                length = Api.opus_encode(_encoder, inputPcmSamples, frames, encodedPtr, sampleLength);
            }
            if (length < 0)
            {
                throw new Exception(String.Format("Encoding failed - {0}", ((ErrorCode)length).ToString()));
            }
            return length;
        }

        /// <summary>
        /// Determines the number of frames in the PCM samples.
        /// </summary>
        /// <param name="pcmSamples"></param>
        /// <returns></returns>
        public int FrameCount(byte[] pcmSamples)
        {
            //  seems like bitrate should be required
            int bitrate = 16;
            int bytesPerSample = (bitrate / 8) * _inputChannels;
            return pcmSamples.Length / bytesPerSample;
        }

        /// <summary>
        /// Helper method to determine how many bytes are required for encoding to work.
        /// </summary>
        /// <param name="frameCount">Target frame size.</param>
        /// <returns></returns>
        public uint FrameByteCount(uint frameCount)
        {
            uint bitrate = 16;
            uint bytesPerSample = (bitrate / 8) * _inputChannels;
            return frameCount * bytesPerSample;
        }


        #region IDisposable

        private bool _disposed;

        ~OpusEncoder()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            GC.SuppressFinalize(this);

            if (_encoder != IntPtr.Zero)
            {
                Api.opus_encoder_destroy(_encoder);
                _encoder = IntPtr.Zero;
            }

            _disposed = true;
        }

        #endregion IDisposable
    }
}
