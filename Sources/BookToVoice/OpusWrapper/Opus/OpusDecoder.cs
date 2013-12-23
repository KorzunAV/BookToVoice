using System;
using OpusWrapper.Opus.Enums;

namespace OpusWrapper.Opus
{
    /// <summary>
    /// Opus audio decoder.
    /// </summary>
    public class OpusDecoder : IDisposable
    {
        private IntPtr _decoder;

        #region properties
        
        /// <summary>
        /// Gets the output sampling rate of the decoder.
        /// </summary>
        public SamplingRate OutputSamplingRate { get; private set; }

        /// <summary>
        /// Gets the number of channels of the decoder.
        /// </summary>
        public Channels OutputChannels { get; private set; }

        /// <summary>
        /// Gets or sets the size of memory allocated for decoding data.
        /// </summary>
        public int MaxDataBytes { get; set; }

        /// <summary>
        /// Gets or sets whether forward error correction is enabled or not.
        /// </summary>
        public bool ForwardErrorCorrection { get; set; }
        #endregion properties


        private OpusDecoder(IntPtr decoder, SamplingRate outputSamplingRateHz, Channels outputChannels)
        {
            _decoder = decoder;
            OutputSamplingRate = outputSamplingRateHz;
            OutputChannels = outputChannels;
            MaxDataBytes = 4000;
        }


        /// <summary>
        /// Creates a new Opus decoder.
        /// </summary>
        /// <param name="outputSamplingRateHz">Sample rate to decode at (Hz). This must be one of 8000, 12000, 16000, 24000, or 48000.</param>
        /// <param name="outputChannels">Number of channels to decode.</param>
        /// <returns>A new <c>OpusDecoder</c>.</returns>
        public static OpusDecoder Create(SamplingRate outputSamplingRateHz, Channels outputChannels)
        {
            IntPtr error;
            IntPtr decoder = Api.opus_decoder_create((int)outputSamplingRateHz, (int)outputChannels, out error);
            if ((ErrorCode)error != ErrorCode.OK)
            {
                throw new Exception("Exception occured while creating decoder");
            }
            return new OpusDecoder(decoder, outputSamplingRateHz, outputChannels);
        }



        /// <summary>
        /// Produces PCM samples from Opus encoded data.
        /// </summary>
        /// <param name="inputOpusData">Opus encoded data to decode, null for dropped packet.</param>
        /// <param name="dataLength">Length of data to decode.</param>
        /// <param name="decodedLength">Set to the length of the decoded sample data.</param>
        /// <returns>PCM audio samples.</returns>
        public unsafe byte[] Decode(byte[] inputOpusData, int dataLength, out int decodedLength)
        {
            if (disposed)
                throw new ObjectDisposedException("OpusDecoder");

            IntPtr decodedPtr;
            byte[] decoded = new byte[MaxDataBytes];
            int frameCount = FrameCount(MaxDataBytes);
            int length = 0;
            fixed (byte* bdec = decoded)
            {
                decodedPtr = new IntPtr((void*)bdec);

                if (inputOpusData != null)
                    length = Api.opus_decode(_decoder, inputOpusData, dataLength, decodedPtr, frameCount, 0);
                else
                    length = Api.opus_decode(_decoder, null, 0, decodedPtr, frameCount, (ForwardErrorCorrection) ? 1 : 0);
            }
            decodedLength = length * 2;
            if (length < 0)
                throw new Exception("Decoding failed - " + ((ErrorCode)length).ToString());

            return decoded;
        }

        /// <summary>
        /// Determines the number of frames that can fit into a buffer of the given size.
        /// </summary>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public int FrameCount(int bufferSize)
        {
            //  seems like bitrate should be required
            int bitrate = 16;
            int bytesPerSample = (bitrate / 8) * (int)OutputChannels;
            return bufferSize / bytesPerSample;
        }

        #region IDisposable

        private bool disposed;

        ~OpusDecoder()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (disposed)
                return;

            GC.SuppressFinalize(this);

            if (_decoder != IntPtr.Zero)
            {
                Api.opus_decoder_destroy(_decoder);
                _decoder = IntPtr.Zero;
            }

            disposed = true;
        }

        #endregion IDisposable
    }
}
