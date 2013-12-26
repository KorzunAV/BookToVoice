using System;

namespace OpusWrapper.Ogg
{
    /// <summary>
    /// ogg_packet is used to encapsulate the data and metadata belonging to a single raw Ogg/Vorbis packet
    /// </summary>
    //[StructLayout(LayoutKind.Sequential)]
    public class Packet
    {
        public byte[] PacketData;
        //public UInt32 Bytes;
        public Int32 Bos;
        public Int32 Eos;

        public UInt64 Granulepos;

        /// <summary>
        /// sequence number for decode; the framing knows where there's a hole in the data,
        /// but we need coupling so that the codec (which is in a separate abstraction layer) 
        /// also knows about the gap
        /// </summary>
        public UInt64 PacketNo;
    }
}
