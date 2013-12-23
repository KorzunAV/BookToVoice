using System;
using System.IO;

namespace OpusWrapper.Ogg
{
    public class Page
    {
        /// <summary>
        /// Pointer to the page header for this page. The exact contents of this header are defined in the framing spec document.
        /// </summary>
        public OggsHeader header;

        /// <summary>
        /// Length of the page header in bytes. 
        /// </summary>
        public Int32 header_len;

        /// <summary>
        /// Pointer to the data for this page.
        /// </summary>
        public byte[] body;

        /// <summary>
        /// Length of the body data in bytes.
        /// </summary>
        public Int32 body_len;


        public int Write(Stream stream)
        {
            stream.Write(header.Data, 0, header_len);
            stream.Write(body, 0, body_len);
            return header_len + body_len;
        }
    }
}
