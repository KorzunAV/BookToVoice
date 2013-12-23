using System;
using System.Runtime.InteropServices;


namespace OpusWrapper.Ogg
{
    internal class Api
    {
        /// <summary>
        /// This function submits a packet to the bitstream for page encapsulation. 
        /// After this is called, more packets can be submitted, or pages can be written out.
        /// In a typical encoding situation, this should be used after filling a packet with data. 
        /// The data in the packet is copied into the internal storage managed by the ogg_stream_state, 
        /// so the caller is free to alter the contents of op after this call has returned.
        /// </summary>
        /// <param name="os">Pointer to a previously declared ogg_stream_state struct.</param>
        /// <param name="op">Pointer to the packet we are putting into the bitstream. </param>
        /// <returns>0 returned on success. -1 returned in the event of internal error.</returns>
        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int ogg_stream_packetin(ref IntPtr os, ref Packet op);

        /// <summary>
        /// This function is used to initialize an ogg_stream_state struct and allocates appropriate memory in preparation for encoding or decoding.
        /// It also assigns the stream a given serial number. 
        /// </summary>
        /// <param name="os">Pointer to the ogg_stream_state struct that we will be initializing.</param>
        /// <param name="serialno">Serial number that we will attach to this stream.</param>
        /// <returns>0 if successful. -1 if unsuccessful.</returns>
        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int ogg_stream_init(out IntPtr os, int serialno);
        
        /// <summary>
        /// This function clears and frees the internal memory used by the ogg_stream_state struct, but does not free the structure itself. 
        /// It is safe to call ogg_stream_clear on the same structure more than once. 
        /// </summary>
        /// <param name="os">Pointer to the ogg_stream_state struct to be cleared.</param>
        /// <returns>0 is always returned.</returns>
        [DllImport("libogg.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int ogg_stream_clear(ref IntPtr os);
    }
}
