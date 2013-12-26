using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpusWrapper.Ogg
{
    /// <summary>
    /// ogg_stream_state contains the current encode/decode state of a logical Ogg bitstream
    /// </summary>
    public class StreamState
    {
        private UInt64 _totalBits = 0;

        #region Property
        /// <summary>
        /// working space for header encode 282
        /// </summary>
        public OggsHeader Header;

        /// <summary>
        /// set when we have buffered the last packet in the logical bitstream
        /// </summary>
        public bool Eos;

        /// <summary>
        /// set after we've written the initial page of a logical bitstream
        /// </summary>
        public Int32 Bos;

        public UInt32 SerialNo;

        public UInt32 PageNo;

        public UInt64 GranulePos;

        #endregion Property

        public List<Packet> Packets = new List<Packet>();

        public StreamState(UInt32 serialNo)
        {
            SerialNo = serialNo;
        }

        public void Packetin(Packet packet)
        {
            Packets.Add(packet);
            _totalBits += (uint)packet.PacketData.Length;
        }

        public string GetVersion()
        {
            return Opus.Api.opus_get_version_string();
        }

        private byte[] CreateSegmentalTable(int size)
        {
            byte[] segmentTable = Enumerable.Repeat((byte)255, size).ToArray();
            int count = 0;
            foreach (var packet in Packets)
            {
                count += packet.PacketData.Length / 255;
                if (count < size)
                {
                    segmentTable[count++] = (byte)(packet.PacketData.Length % 255);
                }
                else
                {
                    break;
                }
            }
            return segmentTable;
        }

        public void Flush(Stream stream)
        {
            int paketPos = 0;
            var dataLen = Packets.Sum(p => p.PacketData.Length);
            while (Packets.Count > 0)
            {
                int segmentsCount = Math.Max((dataLen / 255) + 1, Packets.Count);
                int segmentPerPage = segmentsCount > 255 ? 255 : segmentsCount;

                var oggsHeader = new OggsHeader(PageNo, SerialNo, CreateSegmentalTable(segmentPerPage),
                                                Packets[0].Granulepos, paketPos != 0, Bos == 0,
                                                Eos && segmentPerPage == segmentsCount);
                Bos = 1;
                PageNo++;

                // set pointers in the ogg_page struct
                var og = new Page
                    {
                        header = oggsHeader,
                        header_len = oggsHeader.Data.Length,
                        body_len = oggsHeader.SegmentTableSum
                    };
                og.body = new byte[og.body_len];

                int saved = 0;
                while (saved < og.body_len)
                {
                    var savingPaket = Packets[0];
                    var copyLen = Math.Min(og.body_len - saved, savingPaket.PacketData.Length);
                    Array.Copy(savingPaket.PacketData, paketPos, og.body, saved, copyLen);
                    dataLen -= copyLen;
                    if (copyLen == savingPaket.PacketData.Length)
                    {
                        Packets.Remove(savingPaket);
                    }
                    else
                    {
#if DEBUG
                        if (saved + copyLen != og.body_len)
                        {
                            throw new ArithmeticException();
                        }
#endif
                        paketPos = savingPaket.PacketData.Length - copyLen;
                    }
                    saved += copyLen;
                }
                ogg_page_checksum_set(og);
                og.Write(stream);
            }
            _totalBits = 0;
        }

        public int TotalPages()
        {
            int segmentsCount = Math.Max((int)(_totalBits / 255) + 1, Packets.Count);
            return segmentsCount / 255;
        }

        private void ogg_page_checksum_set(Page og)
        {
            UInt32 crcReg = 0;
            /* safety; needed for API behavior, but not framing code */
            og.header.CrcChecksum = 0;

            for (int i = 0; i < og.header_len; i++)
            {
                crcReg = (crcReg << 8) ^ Presets.Presets.CrcLookup[((crcReg >> 24) & 0xff) ^ og.header.Data[i]];
            }

            for (int i = 0; i < og.body_len; i++)
            {
                crcReg = (crcReg << 8) ^ Presets.Presets.CrcLookup[((crcReg >> 24) & 0xff) ^ og.body[i]];
            }
            og.header.CrcChecksum = crcReg;
        }

    }
}