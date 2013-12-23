using System.Collections.Generic;
using BookToVoice.Core.TextFilters;
using NUnit.Framework;

namespace BookToVoice.Core.Test.TextFilters
{
    [TestFixture]
    class ReplaceByPatternsFilterTest
    {
        private static readonly Dictionary<string, string> CleanStrategy = new Dictionary<string, string>
            {
                {@"(\^\[\d+\] <#n_\d+>)", string.Empty},
                {"[/*<>_]", string.Empty},
                {"—", "-"},
                {"хитр", "хит р"},
            };
        
        [Test]
        public void ReplaceByPatternsTest()
        {
            var filter = new ReplaceByPatternsFilter(CleanStrategy);
            string inStr = "Жил был /ежик, без рогов—и без ножек. ^[1] <#n_1> конец";
            var outstr = filter.Execute(inStr);
            Assert.IsTrue(outstr.Contains("-"));
            Assert.IsFalse(outstr.Contains("—"));
            Assert.IsFalse(outstr.Contains("/"));
            Assert.IsFalse(outstr.Contains("#"));
            Assert.IsFalse(outstr.Contains(@"\ < > _"));
            Assert.IsFalse(outstr.Contains(@"хитрее чем"));
            Assert.IsFalse(outstr.Contains(@"хитрый жук"));
        }
    }
}
