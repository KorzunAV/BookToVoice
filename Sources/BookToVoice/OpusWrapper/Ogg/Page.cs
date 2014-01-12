using System;
using System.IO;

namespace OpusWrapper.Ogg
{
    public class Page
    {
        /// <summary>
        /// Pointer to the page header for this page. The exact contents of this header are defined in the framing spec document.
        /// </summary>
        public OggsHeader Header;

        /// <summary>
        /// Length of the page header in bytes. 
        /// </summary>
        public Int32 HeaderLen;

        /// <summary>
        /// Pointer to the data for this page.
        /// </summary>
        public byte[] Body;

        /// <summary>
        /// Length of the body data in bytes.
        /// </summary>
        public Int32 BodyLen;


        public int Write(Stream stream)
        {
            stream.Write(Header.Data, 0, HeaderLen);
            stream.Write(Body, 0, BodyLen);
            return HeaderLen + BodyLen;
        }
    }
}
