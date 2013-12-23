using System;
using System.Linq;

namespace OpusWrapper.Ogg
{
    public class OggsHeader
    {
        public byte[] Data;

        [Flags]
        public enum HeaderType : byte
        {
            /// <summary>
            /// set: page contains data of a packet continued from the previous page
            /// unset: page contains a fresh packet
            /// </summary>
            ContinuedPage = 0x01,
            /// <summary>
            /// set: this is the first page of a logical bitstream (bos)
            /// unset: this page is not a first page
            /// </summary>
            FirstPage = 0x02,
            /// <summary>
            /// set: this is the last page of a logical bitstream (eos)
            /// unset: this page is not a last page
            /// </summary>
            LastPage = 0x04
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="?">decide how many segments to include</param>
        public OggsHeader(int segmentsCount)
        {
            Data = new byte[27 + segmentsCount];
            Data[0] = 0x4f;
            Data[1] = 0x67;
            Data[2] = 0x67;
            Data[3] = 0x53;
        }


        /// <summary>1.[0-3] capture_pattern:
        /// a 4 Byte field that signifies the beginning of a page.  It contains the magic numbers:
        /// 0x4f 'O'
        /// 0x67 'g'
        /// 0x67 'g'
        /// 0x53 'S'
        /// It helps a decoder to find the page boundaries and regain
        /// synchronisation after parsing a corrupted stream.  Once the
        /// capture pattern is found, the decoder verifies page sync and
        /// integrity by computing and comparing the checksum.
        ///  </summary>
        public byte[] CapturePattern
        {
            get { return Data.Take(4).ToArray(); }
        }

        /// <summary> 2.[4] stream_structure_version:
        /// 1 Byte signifying the version number of
        /// the Ogg file format used in this stream (this document specifies version 0).
        /// </summary>
        public byte StreamStructureVersion
        {
            get { return Data[4]; }
            set { Data[4] = value; }
        }

        /// <summary> 3.[5] header_type_flag:
        /// the bits in this 1 Byte field identify the specific type of this page.
        /// </summary>
        public HeaderType HeaderTypeFlag
        {
            get { return (HeaderType)Data[5]; }
            set { Data[5] = (byte)value; }
        }

        /// <summary> 4.[6-13] granule_position:
        /// an 8 Byte field containing position information.
        /// For example, for an audio stream, it MAY contain the total number
        /// of PCM samples encoded after including all frames finished on this
        /// page.  For a video stream it MAY contain the total number of video
        /// frames encoded after this page.  This is a hint for the decoder
        /// and gives it some timing and position information.  Its meaning is
        /// dependent on the codec for that logical bitstream and specified in
        /// a specific media mapping.  A special value of -1 (in two's complement) 
        /// indicates that no packets finish on this page.
        ///  </summary>
        public UInt64 GranulePosition
        {
            get { return BitConverter.ToUInt64(Data, 6); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 6, 8); }
        }

        /// <summary> 5.[14-17] bitstream_serial_number: 
        /// a 4 Byte field containing the unique
        /// serial number by which the logical bitstream is identified.
        /// </summary>
        public UInt32 BitstreamSerialNumber
        {
            get { return BitConverter.ToUInt32(Data, 14); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 14, 4); }
        }

        /// <summary> 6.[18-21] page_sequence_number:
        /// a 4 Byte field containing the sequence number of the page so the decoder can identify page loss.  
        /// This sequence number is increasing on each logical bitstream separately.
        /// </summary>
        public UInt32 PageSequenceNumber
        {
            get { return BitConverter.ToUInt32(Data, 18); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 18, 4); }
        }

        /// <summary>7.[22-25] CRC_checksum:
        /// a 4 Byte field containing a 32 bit CRC checksum of
        /// the page (including header with zero CRC field and page content).
        /// The generator polynomial is 0x04c11db7.
        /// </summary>
        public UInt32 CrcChecksum
        {
            get { return BitConverter.ToUInt32(Data, 22); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 22, 4); }
        }

        /// <summary>8.[26] number_page_segments:
        /// 1 Byte giving the number of segment entries encoded in the segment table.
        /// </summary>
        public byte NumberPageSegments
        {
            get { return Data[26]; }
            set { Data[26] = value; }
        }

        /// <summary>9.[27..27+segmentsCount] segment_table:
        /// </summary>
        public byte[] SegmentTable
        {
            get { return Data.Skip(27).ToArray(); }
            set { Array.Copy(value, 0, Data, 27, value.Length); }
        }

        public int SegmentTableSum
        {
            get { return Data.Skip(27).Sum(b => b); }
        }

        ///// <summary>9.[27] segment_table:
        ///// number_page_segments Bytes containing the lacing values of all segments in this page.  
        ///// Each Byte contains one lacing value.
        ///// </summary>
        //public byte SegmentTable
        //{
        //    get { return Data[27]; }
        //    set { Data[27] = value; }
        //}

        private void TestLength(int current, int expected)
        {
            if (current != expected)
            {
                throw new Exception(string.Format("{0} Byte requared", expected));
            }
        }
    }
}
