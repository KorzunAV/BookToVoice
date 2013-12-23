using System;
using System.IO;
using System.Linq;

namespace OpusWrapper.Ogg
{
    //public class StreamState : IDisposable
    //{
    //    private IntPtr _stream;

    //    /// <summary>
    //    /// Creates a new StreamState.
    //    /// </summary>
    //    /// <param name="SerialNo">Serial number that we will attach to this stream.</param>
    //    /// <returns></returns>
    //    public StreamState(int SerialNo)
    //    {
    //        int error = Api.ogg_stream_init(out _stream, SerialNo);
    //        if ((ErrorCode)error != ErrorCode.OK)
    //        {
    //            throw new Exception("Exception occured while initializing stream");
    //        }
    //    }

    //    public int Packetin(ref Packet packet)
    //    {
    //        try
    //        {
    //            int error = Api.ogg_stream_packetin(ref _stream, ref packet);
    //        }
    //        catch (Exception exception)
    //        {
    //            string msg = exception.ToString();
    //        }
    //        catch
    //        {

    //            throw;
    //        }
    //        return 0;
    //    }

    //    #region IDisposable
    //    // Use interop to call the method necessary 
    //    // to clean up the unmanaged resource.
    //    [DllImport("Kernel32")]
    //    private extern static Boolean CloseHandle(IntPtr handle);

    //    private bool _disposed;

    //    /// <summary>
    //    /// Implement IDisposable. 
    //    /// Do not make this method virtual. 
    //    /// A derived class should not be able to override this method. 
    //    /// </summary>
    //    public void Dispose()
    //    {
    //        Dispose(true);
    //        // This object will be cleaned up by the Dispose method. 
    //        // Therefore, you should call GC.SupressFinalize to 
    //        // take this object off the finalization queue 
    //        // and prevent finalization code for this object 
    //        // from executing a second time.
    //        GC.SuppressFinalize(this);
    //    }

    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (!_disposed)
    //        {
    //            // If disposing equals true, dispose all managed and unmanaged resources. 
    //            if (disposing)
    //            {
    //                // Dispose managed resources.
    //            }

    //            // Call the appropriate methods to clean up 
    //            // unmanaged resources here. 
    //            // If disposing is false, only the following code is executed.
    //            if (_stream != IntPtr.Zero)
    //            {
    //                Api.ogg_stream_clear(ref _stream);
    //                CloseHandle(_stream);
    //                _stream = IntPtr.Zero;
    //            }

    //            // Note disposing has been done.
    //            _disposed = true;
    //        }
    //    }

    //    /// <summary>
    //    /// Use C# destructor syntax for finalization code. 
    //    /// This destructor will run only if the Dispose method does not get called. 
    //    /// It gives your base class the opportunity to finalize. 
    //    /// Do not provide destructors in types derived from this class.
    //    /// </summary>
    //    ~StreamState()
    //    {
    //        // Do not re-create Dispose clean-up code here. 
    //        // Calling Dispose(false) is optimal in terms of 
    //        // readability and maintainability.
    //        Dispose(false);
    //    }
    //    #endregion IDisposable
    //}

    /// <summary>
    /// ogg_stream_state contains the current encode/decode state of a logical Ogg bitstream
    /// </summary>
    public class StreamState
    {
        #region Property

        /// <summary>
        /// bytes from packet bodies
        /// </summary>
        public byte[] BodyData;

        /// <summary>
        /// storage elements allocated
        /// </summary>
        public Int32 BodyStorage;

        /// <summary>
        /// elements stored; fill mark
        /// </summary>
        public Int32 BodyFill;

        /// <summary>
        /// elements of fill returned
        /// </summary>
        public Int32 BodyReturned;

        /// <summary>
        /// The values that will go to the segment table
        /// </summary>
        public Int32[] LacingVals;

        /// <summary>
        /// granulepos values for headers. Not compact this way, but it is simple coupled to the lacing fifo
        /// </summary>
        public Int64[] GranuleVals;

        public Int32 LacingStorage;

        public Int32 LacingFill;

        public Int32 LacingPacket;

        public Int32 LacingReturned;

        /// <summary>
        /// working space for header encode 282
        /// </summary>
        public OggsHeader Header;

        public Int32 HeaderFill;

        /// <summary>
        /// set when we have buffered the last packet in the logical bitstream
        /// </summary>
        public Int32 Eos;

        /// <summary>
        /// set after we've written the initial page of a logical bitstream
        /// </summary>
        public Int32 Bos;

        public UInt32 SerialNo;

        public UInt32 PageNo;

        /// <summary>
        /// sequence number for decode; the framing knows where there's a hole in the data,
        /// but we need coupling so that the codec (which is in a separate abstractionlayer) also knows about the gap
        /// </summary>
        public Int64 PacketNo;

        public Int64 GranulePos;

        #endregion Property
        
        public StreamState(UInt32 serialNo)
        {
            BodyStorage = 16 * 1024;
            LacingStorage = 1024;

            BodyData = new byte[BodyStorage];
            LacingVals = new int[LacingStorage];
            GranuleVals = new long[LacingStorage];

            SerialNo = serialNo;
        }

        public int Packetin(Packet packet)
        {
            var lacingVals = packet.Bytes / 255 + 1;

            if (BodyReturned != 0)
            {
                /* advance packet data according to the body_returned pointer. We
                   had to keep it around to return a pointer into the buffer last
                   call */
                BodyFill -= BodyReturned;
                if (BodyFill != 0)
                {
                    Array.Copy(BodyData, BodyReturned, BodyData, 0, BodyFill);
                }
                BodyReturned = 0;
            }


            /* Copy in the submitted packet.  Yes, the copy is a waste; this is
               the liability of overly clean abstraction for the time being.  It
               will actually be fairly easy to eliminate the extra copy in the
               future */

            Array.Copy(packet.PacketData, 0, BodyData, BodyFill, packet.Bytes);
            BodyFill += packet.Bytes;


            /* Store lacing vals for this packet */
            for (int i = 0; i < lacingVals - 1; i++)
            {
                LacingVals[LacingFill + i] = 255;
                GranuleVals[LacingFill + i] = GranulePos;
            }
            LacingVals[LacingFill + lacingVals - 1] = packet.Bytes % 255;
            GranulePos = GranuleVals[LacingFill + lacingVals - 1] = packet.Granulepos;

            /* flag the first segment as the beginning of the packet */
            LacingVals[LacingFill] |= 0x100;

            LacingFill += lacingVals;

            /* for the sake of completeness */
            PacketNo++;

            if (packet.Eos != 0)
            {
                Eos = 1;
            }

            return (0);
        }

        /* Conditionally flush a page; force==0 will only flush nominal-size
           pages, force==1 forces us to flush a page regardless of page size
           so long as there's any data available at all. */

        public int ogg_stream_flush(Page og)
        {
            return ogg_stream_flush_i(og, true, 4096);
        }

        public void Flush(Stream stream)
        {
            int ret;
            do
            {
                var og = new Page();
                ret = ogg_stream_flush_i(og, true, 4096);
                if (ret != 0)
                {
                    og.Write(stream);
                }
            } while (ret == 1);
        }

        public string Get()
        {
            return Opus.Api.opus_get_version_string();
        }

        private int ogg_stream_flush_i(Page og, bool force, int nfill)
        {
            int vals;
            int maxvals = (LacingFill > 255 ? 255 : LacingFill);
            int bytes = 0;
            long acc = 0;
            Int64 granulePos = -1;

            //if(ogg_stream_check(os)) return(0);
            if (maxvals == 0)
            {
                return (0);
            }

            // construct a page
            // decide how many segments to include

            // If this is the initial header case, the first page must only include the initial header packet
            if (Bos == 0)
            {
                // 'initial header page' case
                granulePos = 0;
                for (vals = 0; vals < maxvals; vals++)
                {
                    if ((LacingVals[vals] & 0x0ff) < 255)
                    {
                        vals++;
                        break;
                    }
                }
            }
            else
            {

                //The extra packets_done, packet_just_done logic here attempts to do two things:
                //1) Don't unneccessarily span pages.
                //2) Unless necessary, don't flush pages if there are less than four packets on
                //   them; this expands page size to reduce unneccessary overhead if incoming packets are large.
                //These are not necessary behaviors, just 'always better than naive flushing'
                //without requiring an application to explicitly request a specific optimized
                //behavior. We'll want an explicit behavior setup pathway eventually as well.

                int packetsDone = 0;
                int packetJustDone = 0;
                for (vals = 0; vals < maxvals; vals++)
                {
                    if (acc > nfill && packetJustDone >= 4)
                    {
                        force = true;
                        break;
                    }
                    acc += LacingVals[vals] & 0x0ff;
                    if ((LacingVals[vals] & 0xff) < 255)
                    {
                        granulePos = GranuleVals[vals];
                        packetJustDone = ++packetsDone;
                    }
                    else
                    {
                        packetJustDone = 0;
                    }
                }
                if (vals == 255)
                {
                    force = true;
                }
            }

            if (!force)
            {
                return (0);
            }

            // construct the header in temp storage
            var oggsHeader = new OggsHeader(vals);

            if ((LacingVals[0] & 0x100) == 0)
            {
                oggsHeader.HeaderTypeFlag = OggsHeader.HeaderType.ContinuedPage;
            }
            if (Bos == 0)
            {
                oggsHeader.HeaderTypeFlag = OggsHeader.HeaderType.FirstPage;
            }
            if (Eos != 0 && LacingFill == vals)
            {
                oggsHeader.HeaderTypeFlag = OggsHeader.HeaderType.LastPage;
            }
            Bos = 1;

            oggsHeader.GranulePosition = (UInt64)granulePos;
            oggsHeader.BitstreamSerialNumber = SerialNo;

            /* 32 bits of page counter (we have both counter and page header because this val can roll over) */
            if (PageNo == -1)
            {
                PageNo = 0; /* because someone called
                                     stream_reset; this would be a
                                     strange thing to do in an
                                     encode stream, but it has
                                     plausible uses */
            }
            oggsHeader.PageSequenceNumber = PageNo++;
            oggsHeader.NumberPageSegments = (byte)(vals & 0xff);

            // segment table
            oggsHeader.SegmentTable = LacingVals.Take(vals).Select(v => (byte)(v & 0xff)).ToArray();
            bytes += oggsHeader.SegmentTableSum;

            // set pointers in the ogg_page struct
            og.header = Header = oggsHeader;
            og.header_len = HeaderFill = vals + 27;

            og.body = new byte[BodyData.Length - BodyReturned];
            Array.Copy(BodyData, BodyReturned, og.body, 0, BodyData.Length - BodyReturned);

            og.body_len = bytes;

            // advance the lacing data and set the body_returned pointer
            LacingFill -= vals;
            Array.Copy(LacingVals, vals, LacingVals, 0, LacingFill * LacingVals.Length);
            Array.Copy(GranuleVals, vals, GranuleVals, 0, LacingFill * GranuleVals.Length);
            BodyReturned += bytes;

            // calculate the checksum 
            ogg_page_checksum_set(og);
            // done 
            return (1);
        }


        /* checksum the page */
        /* Direct table CRC; note that this will be faster in the future if we
           perform the checksum simultaneously with other copies */

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

///// <summary>
///// ogg_stream_state contains the current encode/decode state of a logical Ogg bitstream
///// </summary>
//[StructLayout(LayoutKind.Sequential)]
//public struct ogg_stream_state
//{
//    /// <summary>
//    /// bytes from packet bodies
//    /// </summary>
//    public byte[] body_data;

//    /// <summary>
//    /// storage elements allocated
//    /// </summary>
//    public Int32 body_storage;

//    /// <summary>
//    /// elements stored; fill mark
//    /// </summary>
//    public Int32 body_fill;

//    /// <summary>
//    /// elements of fill returned
//    /// </summary>
//    public Int32 body_returned;

//    /// <summary>
//    /// The values that will go to the segment table
//    /// </summary>
//    public Int32[] lacing_vals;

//    /// <summary>
//    /// granulepos values for headers. Not compact this way, but it is simple coupled to the lacing fifo
//    /// </summary>
//    public Int64[] granule_vals;

//    public Int32 lacing_storage;

//    public Int32 lacing_fill;

//    public Int32 lacing_packet;

//    public Int32 lacing_returned;

//    /// <summary>
//    /// working space for header encode 282
//    /// </summary>
//    public byte[] header;

//    public Int32 header_fill;

//    /// <summary>
//    /// set when we have buffered the last packet in the logical bitstream
//    /// </summary>
//    public Int32 e_o_s;

//    /// <summary>
//    /// set after we've written the initial page of a logical bitstream
//    /// </summary>
//    public Int32 b_o_s;

//    public Int32 SerialNo;
//    public Int32 pageno;

//    /// <summary>
//    /// sequence number for decode; the framing knows where there's a hole in the data,
//    /// but we need coupling so that the codec (which is in a separate abstractionlayer) also knows about the gap
//    /// </summary>
//    public Int64 packetno;

//    public Int64 granulepos;
//}
//}