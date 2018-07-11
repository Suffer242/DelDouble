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

            CrcLibrary crcLibrary = new CrcLibrary(new CrcIniStore(@"f:\_debug\test.txt"));

            FileList fileList = new FileList(1, new CrcCalculator(crcLibrary)) {CallBack = Console.WriteLine};


            fileList.AddDirectory(@"f:\____PHOTO_C\");

            crcLibrary.SaveCrc();

            var list = fileList.DoubleDirectoryList;


            //  Common.MtsFileDate(@"d:\[MAIN_MEDIA]\[STAGE2]\HDR-UX20E\1-сентября\AVCHD\BDMV\STREAM\00004.MTS");



            Assert.AreEqual(true, true);
            /*
             * 
             * 
                        var r = Common.parallel_comparefile(@"d:\1\Ready.Player.One.2018.WEB-DL.720p.ExKinoRay.mkv",
                            @"e:\downloads\Ready.Player.One.2018.WEB-DL.720p.ExKinoRay.mkv");
                        Assert.AreEqual(r, true);
                        */

        }
    }
}