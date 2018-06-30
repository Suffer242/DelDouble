using Microsoft.VisualStudio.TestTools.UnitTesting;
using FindDoubles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindDoubles.Tests
{
    [TestClass()]
    public class Form1Tests
    {
        [TestMethod()]
        public void comparefileTest()
        {

            var r = Common.parallel_comparefile(@"d:\1\Ready.Player.One.2018.WEB-DL.720p.ExKinoRay.mkv",
                @"e:\downloads\Ready.Player.One.2018.WEB-DL.720p.ExKinoRay.mkv");
            Assert.AreEqual(r, true);
        }
    }
}