using System;
using System.Runtime.InteropServices;
using OpusWrapper.Opus.Enums;

namespace OpusWrapper.Opus
{
    /// <summary>
    /// Wraps the Opus API.
    /// </summary>
    internal class Api
    {
        #region Opus Decoder

        /// <summary>
        /// Gets the size of an OpusDecoder structure. 
        /// </summary>
        /// <param name="channels"></param>
        /// <returns></returns>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int opus_decoder_get_size(Channels channels);

        /// <summary>
        /// Allocates and initializes a decoder state. 
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="channels"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr opus_decoder_create(int fs, int channels, out IntPtr error);

        /// <summary>
        /// Initializes a previously allocated decoder state. 
        /// </summary>
        /// <param name="st"></param>
        /// <param name="fs"></param>
        /// <param name="channels"></param>
        /// <returns></returns>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern ErrorCode opus_decoder_init(IntPtr st, SamplingRate fs, Channels channels);

        /// <summary>
        /// Decode an Opus packet. 
        /// </summary>
        /// <param name="st"></param>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <param name="pcm"></param>
        /// <param name="frameSize"></param>
        /// <param name="decodeFec"></param>
        /// <returns></returns>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int opus_decode(IntPtr st, byte[] data, int len, IntPtr pcm, int frameSize, int decodeFec);

        /// <summary>
        /// Decode an Opus packet with floating point output. 
        /// </summary>
        /// <param name="st"></param>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <param name="pcm"></param>
        /// <param name="frameSize"></param>
        /// <param name="decodeFec"></param>
        /// <returns></returns>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int opus_decode_float(IntPtr st, byte[] data, int len, float[] pcm, int frameSize, int decodeFec);

        ////Perform a CTL function on an Opus decoder. 
        //internal static extern int 	opus_decoder_ctl (IntPtr *st, int request,...)

        /// <summary>
        /// Frees an OpusDecoder allocated by opus_decoder_create(). 
        /// </summary>
        /// <param name="decoder"></param>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void opus_decoder_destroy(IntPtr decoder);


        //int 	opus_packet_parse (const unsigned char *data, opus_int32 len, unsigned char *out_toc, const unsigned char *frames[48], opus_int16 size[48], int *payload_offset)
        //Parse an opus packet into one or more frames.


        /// <summary>
        /// Gets the bandwidth of an Opus packet.    
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int opus_packet_get_bandwidth(byte[] data);


        //int 	opus_packet_get_samples_per_frame (const unsigned char *data, opus_int32 Fs)
        //Gets the number of samples per frame from an Opus packet.


        /// <summary>
        /// Gets the number of channels from an Opus packet. 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int opus_packet_get_nb_channels(byte[] data);

        /// <summary>
        /// Gets the number of frames in an Opus packet. 
        /// </summary>
        /// <param name="?"></param>
        /// <param name="packet"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int opus_packet_get_nb_frames(byte[] packet, Int32 len);

        /// <summary>
        /// Gets the number of samples of an Opus packet. 
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="len"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int opus_packet_get_nb_samples(byte[] packet, Int32 len, Int32 fs);

        //int 	opus_decoder_get_nb_samples (const OpusDecoder *dec, const unsigned char packet[], opus_int32 len)
        //Gets the number of samples of an Opus packet.

        //void 	opus_pcm_soft_clip (float *pcm, int frame_size, int channels, float *softclip_mem)
        //Applies soft-clipping to bring a float signal within the [-1,1] range. 

        #endregion Opus Decoder
        
        #region Opus Encoder

        /// <summary>
        /// Gets the size of an OpusEncoder structure.
        /// </summary>
        /// <param name="channels"></param>
        /// <returns></returns>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int opus_encoder_get_size(Channels channels);

        /// <summary>
        /// Allocates and initializes an encoder state.
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="channels"></param>
        /// <param name="application"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr opus_encoder_create(int fs, int channels, int application, out IntPtr error);

        /// <summary>
        /// Initializes a previously allocated encoder state The memory pointed to by st must be at least the size returned by opus_encoder_get_size().
        /// </summary>
        /// <param name="st"></param>
        /// <param name="fs"></param>
        /// <param name="channels"></param>
        /// <param name="application"></param>
        /// <returns></returns>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern ErrorCode opus_encoder_init(IntPtr st, SamplingRate fs, Channels channels, ApplicationType application);

        /// <summary>
        /// Encodes an Opus frame.
        /// </summary>
        /// <param name="st"></param>
        /// <param name="pcm"></param>
        /// <param name="frameSize"></param>
        /// <param name="data"></param>
        /// <param name="maxDataBytes"></param>
        /// <returns></returns>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int opus_encode(IntPtr st, byte[] pcm, int frameSize, IntPtr data, int maxDataBytes);
        
        /// <summary>
        /// Encodes an Opus frame from floating point input.
        /// </summary>
        /// <param name="st"></param>
        /// <param name="pcm"></param>
        /// <param name="frameSize"></param>
        /// <param name="data"></param>
        /// <param name="maxDataBytes"></param>
        /// <returns></returns>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int opus_encode_float(IntPtr st, float[] pcm, int frameSize, byte[] data, int maxDataBytes);

        /// <summary>
        /// Frees an OpusEncoder allocated by opus_encoder_create().
        /// </summary>
        /// <param name="encoder"></param>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void opus_encoder_destroy(IntPtr encoder);
        
        /// <summary>
        /// Perform a CTL function on an Opus encoder. 
        /// </summary>
        /// <param name="st"></param>
        /// <param name="request"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int opus_encoder_ctl(IntPtr st, CtlSetRequest request, int value);

        /// <summary>
        /// Perform a CTL function on an Opus encoder. 
        /// </summary>
        /// <param name="st"></param>
        /// <param name="request"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int opus_encoder_ctl(IntPtr st, CtlGetRequest request, out int value);
        
        #endregion Opus Encoder
        

        //opus_get_version_string
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern string opus_get_version_string();
        //opus_multistream_decode
        //opus_multistream_decode_float
        //opus_multistream_decoder_create
        //opus_multistream_decoder_ctl
        //opus_multistream_decoder_destroy
        //opus_multistream_decoder_get_size
        //opus_multistream_decoder_init
        //opus_multistream_encode
        //opus_multistream_encode_float
        //opus_multistream_encoder_create
        //opus_multistream_encoder_ctl
        //opus_multistream_encoder_destroy
        //opus_multistream_encoder_get_size
        //opus_multistream_encoder_init
        //opus_multistream_surround_encoder_create
        //opus_multistream_surround_encoder_get_size
        //opus_multistream_surround_encoder_init

        //opus_repacketizer_cat
        //opus_repacketizer_create
        //opus_repacketizer_destroy
        //opus_repacketizer_get_nb_frames
        //opus_repacketizer_get_size
        //opus_repacketizer_init
        //opus_repacketizer_out
        //opus_repacketizer_out_range
        //opus_strerror
        [DllImport("libopus.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern string opus_strerror(ErrorCode error);
    }
}