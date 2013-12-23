using System;
using System.Linq;
using NUnit.Framework;
using OpusWrapper.Ogg;

namespace OpusWrapper.Test.Ogg
{
    [TestFixture]
    class OggsHeaderTest
    {
        [Test]
        public void Test()
        {
            var header = new OggsHeader(0);
            byte p1 = 0x1;
            header.StreamStructureVersion = p1;
            var p2 = OggsHeader.HeaderType.FirstPage | OggsHeader.HeaderType.LastPage;
            header.HeaderTypeFlag = p2;
            UInt64 p3 = 578437695752307201;
            header.GranulePosition = p3;
            UInt32 p4 = 67305985;
            header.BitstreamSerialNumber = p4;
            UInt32 p5 = 134678021;
            header.PageSequenceNumber = p5;
            UInt32 p6 = 50462985;
            header.CrcChecksum = p6;
            byte p7 = 0x7;
            header.NumberPageSegments = p7;
            byte p8 = 0x8;
            //header.SegmentTable = p8;

            Assert.IsTrue(header.CapturePattern.SequenceEqual("OggS".Select(c => (byte)c).ToArray()));
            Assert.IsTrue(p1 == header.StreamStructureVersion);
            Assert.IsTrue(p2 == header.HeaderTypeFlag);
            Assert.IsTrue(p3 == header.GranulePosition);
            Assert.IsTrue(p4 == header.BitstreamSerialNumber);
            Assert.IsTrue(p5 == header.PageSequenceNumber);
            Assert.IsTrue(p6 == header.CrcChecksum);
            Assert.IsTrue(p7 == header.NumberPageSegments);
            //Assert.IsTrue(p8 == header.SegmentTable);
        }
    }
}