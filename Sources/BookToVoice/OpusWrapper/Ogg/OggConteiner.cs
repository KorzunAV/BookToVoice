//using System;
//using System.IO;
//using OpusWrapper.Ogg;

//namespace OpusWrapper
//{
//    public class OggConteiner
//    {

//        /// <summary>
//        /// The following function is basically a hacked version of the code in examples/encoder_example.c
//        /// </summary>
//        /// <param name="?"></param>
//        public void write_vorbis_data_or_die(Stream stream, int srate, float q, byte[] data, int count, int ch)
//        {
//            Page og;
//            Packet op;

//            // int eos = 0, ret;

//            // Encode setup 
//            // vorbis_info      vi;
//            // vorbis_info_init (&vi);

//            // ret = vorbis_encode_init_vbr (&vi,ch,srate,q);
//            // if (ret) {
//            //   printf ("vorbis_encode_init_vbr return %d\n", ret) ;
//            //   exit (1) ;
//            // }

//            // vorbis_comment   vc;
//            // vorbis_comment_init (&vc);
//            // vorbis_comment_add_tag (&vc,"ENCODER","test/util.c");

//            vorbis_dsp_state vd;
//            vorbis_analysis_init(&vd, &vi);

//            // vorbis_block     vb;
//            // vorbis_block_init (&vd,&vb);
//            using (var os = new StreamState(12345678))
//            {
//                Packet header = new Packet();
//                //Packet header_comm;
//                //Packet header_code;

//                vorbis_analysis_headerout(&vd, &vc, &header, &header_comm, &header_code);
//                os.Packetin(ref header);
//                //os.Packetin(ref header_comm);
//                //os.Packetin(ref header_code);

//                //   //Ensures the audio data will start on a new page.
//                //   while (!eos){
//                //       int result = ogg_stream_flush (&os,&og);
//                //       if (result == 0)
//                //           break;
//                //       fwrite (og.header,1,og.header_len,file);
//                //       fwrite (og.body,1,og.body_len,file);
//                //   }

//            }

//            // {
//            //   //expose the buffer to submit data 
//            //   float **buffer = vorbis_analysis_buffer (&vd,count);
//            //   int i;

//            //   for(i=0;i<ch;i++)
//            //     memcpy (buffer [i], data, count * sizeof (float)) ;

//            //   // tell the library how much we actually submitted 
//            //   vorbis_analysis_wrote (&vd,count);
//            //   vorbis_analysis_wrote (&vd,0);
//            // }

//            // while (vorbis_analysis_blockout (&vd,&vb) == 1) {
//            //   vorbis_analysis (&vb,NULL);
//            //   vorbis_bitrate_addblock (&vb);

//            //   while (vorbis_bitrate_flushpacket (&vd,&op)) {
//            //     ogg_stream_packetin (&os,&op);

//            //     while (!eos) {
//            //         int result = ogg_stream_pageout (&os,&og);
//            //         if (result == 0)
//            //             break;
//            //         fwrite (og.header,1,og.header_len,file);
//            //         fwrite (og.body,1,og.body_len,file);

//            //         if (ogg_page_eos (&og))
//            //             eos = 1;
//            //     }
//            //   }
//            // }

//            // ogg_stream_clear (&os);
//            // vorbis_block_clear (&vb);
//            // vorbis_dsp_clear (&vd);
//            // vorbis_comment_clear (&vc);
//            // vorbis_info_clear (&vi);

//            //fclose (file) ;
//        }


////        /* arbitrary settings and spec-mandated numbers get filled in here */
////        int vorbis_analysis_init(vorbis_dsp_state* v, vorbis_info* vi){
////  private_state *b=NULL;

////  if(_vds_shared_init(v,vi,1))return 1;
////  b=v->backend_state;
////  b->psy_g_look=_vp_global_look(vi);

////  /* Initialize the envelope state storage */
////  b->ve=_ogg_calloc(1,sizeof(*b->ve));
////  _ve_envelope_init(b->ve,vi);

////  vorbis_bitrate_init(vi,&b->bms);

////  /* compressed audio packets start after the headers
////     with sequence number 3 */
////  v->sequence=3;

////  return(0);
////}

//        int vorbis_analysis_headerout(vorbis_dsp_state* v, vorbis_comment* vc, ogg_packet* op, ogg_packet* op_comm, ogg_packet* op_code)
//        {
//            //  int ret=OV_EIMPL;
//            //  vorbis_info *vi=v->vi;
//            //  oggpack_buffer opb;
//            private_state* b = v->backend_state;

//            if (!b)
//            {
//                ret = OV_EFAULT;
//                goto err_out;
//            }

//            //  /* first header packet **********************************************/

//            //  oggpack_writeinit(&opb);
//            //  if(_vorbis_pack_info(&opb,vi))goto err_out;

//            //  /* build the packet */
//            if (b->header) _ogg_free(b->header);
//            b->header = _ogg_malloc(oggpack_bytes(&opb));
//            memcpy(b->header, opb.buffer, oggpack_bytes(&opb));
//            op->packet = b->header;
//            op->bytes = oggpack_bytes(&opb);
//            op->b_o_s = 1;
//            op->e_o_s = 0;
//            op->granulepos = 0;
//            op->packetno = 0;

//            //  /* second header packet (comments) **********************************/

//            //  oggpack_reset(&opb);
//            //  if(_vorbis_pack_comment(&opb,vc))goto err_out;

//            //  if(b->header1)_ogg_free(b->header1);
//            //  b->header1=_ogg_malloc(oggpack_bytes(&opb));
//            //  memcpy(b->header1,opb.buffer,oggpack_bytes(&opb));
//            //  op_comm->packet=b->header1;
//            //  op_comm->bytes=oggpack_bytes(&opb);
//            //  op_comm->b_o_s=0;
//            //  op_comm->e_o_s=0;
//            //  op_comm->granulepos=0;
//            //  op_comm->packetno=1;

//            //  /* third header packet (modes/codebooks) ****************************/

//            //  oggpack_reset(&opb);
//            //  if(_vorbis_pack_books(&opb,vi))goto err_out;

//            //  if(b->header2)_ogg_free(b->header2);
//            //  b->header2=_ogg_malloc(oggpack_bytes(&opb));
//            //  memcpy(b->header2,opb.buffer,oggpack_bytes(&opb));
//            //  op_code->packet=b->header2;
//            //  op_code->bytes=oggpack_bytes(&opb);
//            //  op_code->b_o_s=0;
//            //  op_code->e_o_s=0;
//            //  op_code->granulepos=0;
//            //  op_code->packetno=2;

//            //  oggpack_writeclear(&opb);
//            //  return(0);
//            // err_out:
//            //  memset(op,0,sizeof(*op));
//            //  memset(op_comm,0,sizeof(*op_comm));
//            //  memset(op_code,0,sizeof(*op_code));

//            //  if(b){
//            //    oggpack_writeclear(&opb);
//            //    if(b->header)_ogg_free(b->header);
//            //    if(b->header1)_ogg_free(b->header1);
//            //    if(b->header2)_ogg_free(b->header2);
//            //    b->header=NULL;
//            //    b->header1=NULL;
//            //    b->header2=NULL;
//            //  }
//            //  return(ret);
//        }
//    }
//}
