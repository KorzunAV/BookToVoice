//using System;
//using System.IO;
//using NUnit.Framework;
//using OpusWrapper.Ogg;

//namespace OpusWrapper.Test.Ogg
//{
//    [TestFixture]
//    internal class SaveTest
//    {
//        [Test]
//        public void Test()
//        {
//            var dataLen = 2048;

//            /* Do safest and most used sample rates first. */
//            int[] sample_rates = { 44100, 48000, 32000, 22050, 16000, 96000 };

//            byte[] dataOut = gen_windowed_sine(dataLen, 255);

//            for (int ch = 1; ch <= 8; ch++)
//            {
//                float q = -.05f;
//                while (q < 1.0)
//                {
//                    for (int k = 0; k < sample_rates.Length; k++)
//                    {
//                        string filename = string.Format("vorbis_{0}_{1}_{2}.ogg", ch, q * 10, sample_rates[k]);

//                        /* Set to know value. */
//                        var data_in = set_data_in(dataLen, 13);

//                        using (FileStream fs = new FileStream(filename, FileMode.Create))
//                        {
//                            var conteiner = new OggConteiner();
//                            conteiner.write_vorbis_data_or_die(fs, sample_rates[k], q, dataOut, dataLen, ch);
//                        }
                        
//                        //read_vorbis_data_or_die(filename, sample_rates[k], data_in, ARRAY_LEN(data_in));

//                        check_output(ref data_in, (.15f - .1f * q));
//                    }
//                    q += 0.1f;
//                }
//            }
//        }

//        private byte[] set_data_in(int len, byte value)
//        {
//            var data = new byte[len];

//            for (int k = 0; k < len; k++)
//            {
//                data[k] = value;
//            }
//            return data;

//        }

//        private byte[] gen_windowed_sine(int len, float maximum)
//        {
//            var dataOut = new byte[len];

//            len /= 2;
//            for (int k = 0; k < len; k++)
//            {
//                var t1 = Math.Sin(2.0 * k * Math.PI * 1.0 / 32.0 + 0.4) * maximum * (0.5 - 0.5 * Math.Cos(2.0 * Math.PI * k / ((len) - 1)));
//                var t2 = maximum * 0.95 * (0.5 - 0.5 * Math.Cos(2.0 * Math.PI * k / ((len) - 1)));
//                dataOut[k++] = (byte)t1;
//                dataOut[k] = (byte)t2;
//            }

//            return dataOut;
//        }

//        private int check_output(ref byte[] dataIn, float allowable)
//        {
//            float maxAbs = 0.0f;

//            for (int k = 0; k < dataIn.Length; k++)
//            {
//                float temp = Math.Abs(dataIn[k]);
//                maxAbs = Math.Max(maxAbs, temp);
//            }

//            Assert.IsTrue(maxAbs < 0.95 - allowable, string.Format("Error : max_abs {0} too small.", maxAbs));
//            Assert.IsTrue(maxAbs > .95 + allowable, string.Format("Error : max_abs {0} too big.", maxAbs));

//            return 0;
//        }

//        /// <summary>
//        /// The following function is basically a hacked version of the code in examples/decoder_example.c
//        /// </summary>
//        /// <param name="?"></param>
//        private void read_vorbis_data_or_die(string filename, int srate, byte[] data, int count)
//        {
//            //  ogg_sync_state   oy;
//            //  ogg_stream_state os;
//            //  ogg_page         og;
//            //  ogg_packet       op;

//            //  vorbis_info      vi;
//            //  vorbis_comment   vc;
//            //  vorbis_dsp_state vd;
//            //  vorbis_block     vb;

//            //  FILE *file;
//            //  char *buffer;
//            //  int  bytes;
//            //  int eos = 0;
//            //  int i;
//            //  int read_total = 0 ;

//            //  if ((file = fopen (filename, "rb")) == NULL) {
//            //    printf("\n\nError : fopen failed : %s\n", strerror (errno)) ;
//            //    exit (1) ;
//            //  }

//            //  ogg_sync_init (&oy);

//            //  {
//            //    // fragile!  Assumes all of our headers will fit in the first 8kB, which currently they will 
//            //    buffer = ogg_sync_buffer (&oy,8192);
//            //    bytes = fread (buffer,1,8192,file);
//            //    ogg_sync_wrote (&oy,bytes);

//            //    if(ogg_sync_pageout (&oy,&og) != 1) {
//            //      if(bytes < 8192) {
//            //        printf ("Out of data.\n") ;
//            //          goto done_decode ;
//            //      }

//            //      fprintf (stderr,"Input does not appear to be an Ogg bitstream.\n");
//            //      exit (1);
//            //    }

//            //    ogg_stream_init (&os,ogg_page_serialno(&og));

//            //    vorbis_info_init (&vi);
//            //    vorbis_comment_init (&vc);
//            //    if (ogg_stream_pagein (&os,&og) < 0) {
//            //      fprintf (stderr,"Error reading first page of Ogg bitstream data.\n");
//            //      exit (1);
//            //    }

//            //    if (ogg_stream_packetout(&os,&op) != 1) {
//            //      fprintf (stderr,"Error reading initial header packet.\n");
//            //      exit (1);
//            //    }

//            //    if (vorbis_synthesis_headerin (&vi,&vc,&op) < 0) {
//            //      fprintf (stderr,"This Ogg bitstream does not contain Vorbis "
//            //          "audio data.\n");
//            //      exit (1);
//            //    }

//            //    i = 0;
//            //    while ( i < 2) {
//            //      while (i < 2) {

//            //        int result = ogg_sync_pageout (&oy,&og);
//            //        if(result == 0)
//            //          break;
//            //        if(result==1) {
//            //          ogg_stream_pagein(&os,&og);

//            //          while (i < 2) {
//            //            result = ogg_stream_packetout (&os,&op);
//            //            if (result == 0) break;
//            //            if (result < 0) {
//            //              fprintf (stderr,"Corrupt secondary header.  Exiting.\n");
//            //              exit(1);
//            //            }
//            //            vorbis_synthesis_headerin (&vi,&vc,&op);
//            //            i++;
//            //          }
//            //        }
//            //      }

//            //      buffer = ogg_sync_buffer (&oy,4096);
//            //      bytes = fread (buffer,1,4096,file);
//            //      if (bytes == 0 && i < 2) {
//            //        fprintf (stderr,"End of file before finding all Vorbis headers!\n");
//            //        exit (1);
//            //      }

//            //      ogg_sync_wrote (&oy,bytes);
//            //    }

//            //    if (vi.rate != srate) {
//            //      printf ("\n\nError : File '%s' has sample rate of %ld when it should be %d.\n\n", filename, vi.rate, srate);
//            //      exit (1) ;
//            //    }

//            //    vorbis_synthesis_init (&vd,&vi);
//            //    vorbis_block_init (&vd,&vb);

//            //    while(!eos) {
//            //      while (!eos) {
//            //        int result = ogg_sync_pageout (&oy,&og);
//            //        if (result == 0)
//            //          break;
//            //        if (result < 0) {
//            //          fprintf (stderr,"Corrupt or missing data in bitstream; "
//            //                   "continuing...\n");
//            //        } else {
//            //          ogg_stream_pagein (&os,&og);
//            //          while (1) {
//            //            result = ogg_stream_packetout (&os,&op);

//            //            if (result == 0)
//            //              break;
//            //            if (result < 0) {
//            //              // no reason to complain; already complained above 
//            //            } else {
//            //              float **pcm;
//            //              int samples;

//            //              if (vorbis_synthesis (&vb,&op) == 0)
//            //                vorbis_synthesis_blockin(&vd,&vb);
//            //              while ((samples = vorbis_synthesis_pcmout (&vd,&pcm)) > 0 && read_total < count) {
//            //                int bout = samples < count ? samples : count;
//            //                bout = read_total + bout > count ? count - read_total : bout;

//            //                memcpy (data + read_total, pcm[0], bout * sizeof (float)) ;

//            //                vorbis_synthesis_read (&vd,bout);
//            //                read_total += bout ;
//            //              }
//            //            }
//            //          }

//            //          if (ogg_page_eos (&og)) eos = 1;
//            //        }
//            //      }

//            //      if (!eos) {
//            //        buffer = ogg_sync_buffer (&oy,4096);
//            //        bytes = fread (buffer,1,4096,file);
//            //        ogg_sync_wrote (&oy,bytes);
//            //        if (bytes == 0) eos = 1;
//            //      }
//            //    }

//            //    ogg_stream_clear (&os);

//            //    vorbis_block_clear (&vb);
//            //    vorbis_dsp_clear (&vd);
//            //    vorbis_comment_clear (&vc);
//            //    vorbis_info_clear (&vi);
//            //  }
//            //done_decode:

//            //  // OK, clean up the framer
//            //  ogg_sync_clear (&oy);

//            //  fclose (file) ;
//        }


//    }
//}