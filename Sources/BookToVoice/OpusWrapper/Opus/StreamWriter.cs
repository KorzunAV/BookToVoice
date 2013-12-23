using System;
using OpusWrapper.Ogg;
using OpusWrapper.Opus.Enums;

namespace OpusWrapper.Opus
{
    public class StreamWriter
    {

        public static void WriteHeader(byte channels, MappingFamily mappingFamily, SamplingRate samplingRate,
                                       byte preskip, UInt16 gain, byte version)
        {
            var header_data = new OpusHeader(channels, mappingFamily, samplingRate, preskip, gain, version);
            var op = new Packet();
            op.PacketData = header_data.Packet;
            op.Bytes = header_data.Packet.Length;
            op.Bos = 1;
            op.Eos = 0;
            op.Granulepos = 0;
            op.PacketNo = 0;

            var os = new StreamState(123456);

            //        os.Packetin(ref op);

            //while((ret=ogg_stream_flush(&os, &og))){
            //  if(!ret)break;
            //  ret=oe_write_page(&og, fout);
            //  if(ret!=og.header_len+og.body_len){
            //    fprintf(stderr,"Error: failed writing header to output stream\n");
            //    exit(1);
            //  }
            //  bytes_written+=ret;
            //  pages_out++;
            //}

            //comment_pad(&inopt.comments, &inopt.comments_length, comment_padding);
            //op.packet=(unsigned char *)inopt.comments;
            //op.bytes=inopt.comments_length;
            //op.b_o_s=0;
            //op.e_o_s=0;
            //op.granulepos=0;
            //op.packetno=1;
            //ogg_stream_packetin(&os, &op);
        }
    }
}
