using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TotCommonLibrary;

namespace FindDoubles
{
    public class Common
    {

        public static DateTime MtsFileDate(String fn)
        {

            var pi = new ProcessStartInfo(@"e:\Downloads\MediaInfo_CLI_18.05_Windows_x64\MediaInfo.exe", "--Output=XML \"" + fn + "\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                 
            };

            var p = Process.Start(pi);
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return DateTime.Parse(output.AfterFirst("<Recorded_Date>").BeforeFirst("+"));

        }

        public static uint CompureFullCrc(String FileName)
        {
            uint crc = 0;

            using (FileStream fs = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                byte[] buff = new byte[1024];
                while (fs.Length != fs.Position)
                {
                    int count = fs.Read(buff, 0, buff.Length);
                    crc = Crc32C.Crc32CAlgorithm.Append(crc, buff, 0, count);
                }
            }
            return crc;
        }

        public static unsafe bool SequenceEqual(byte[] thisArray, byte[] array)
        {
            int length = thisArray.Length;
            if (length != array.Length) return false;
            fixed (byte* str = thisArray)
            {
                byte* chPtr = str;
                fixed (byte* str2 = array)
                {
                    byte* chPtr2 = str2;
                    while (length >= 20)
                    {
                        if ((((*(((int*)chPtr)) != *(((int*)chPtr2))) ||
                        (*(((int*)(chPtr + 4))) != *(((int*)(chPtr2 + 4))))) ||
                        ((*(((int*)(chPtr + 8))) != *(((int*)(chPtr2 + 8)))) ||
                        (*(((int*)(chPtr + 12))) != *(((int*)(chPtr2 + 12)))))) ||
                        (*(((int*)(chPtr + 16))) != *(((int*)(chPtr2 + 16))))) break;

                        chPtr += 20;
                        chPtr2 += 20;
                        length -= 20;
                    }

                    while (length >= 4)
                    {
                        if (*(((int*)chPtr)) != *(((int*)chPtr2))) break;
                        chPtr += 4;
                        chPtr2 += 4;

                        length -= 4;
                    }

                    while (length > 0)
                    {
                        if (*chPtr != *chPtr2) break;
                        chPtr++;
                        chPtr2++;
                        length--;
                    }

                    return (length <= 0);
                }
            }
        }
        const int bufsize = 100 * 1024 * 1024; // 100 mb
        class FileCacher
        {
            byte[][] buf;

            FileStream fs;
            int cursor = 1;
            public int BytesRead { get; private set; }

            public byte[] CurrentBuf { get
                {
                    return buf[cursor];
                } }

            public uint crc { get; private set; }


            public AutoResetEvent HaveData { get; private set; }
            public AutoResetEvent NeedData { get; private set; }

            public FileCacher(String fn)
            {
                buf = new byte[2][];

                NeedData = new AutoResetEvent(true);
                HaveData = new AutoResetEvent(false);
                fs = new FileStream(fn, FileMode.Open, FileAccess.Read);
            }

            public void Execute()
            {
                crc = 0;
                while (true)
                {
                    NeedData.WaitOne();

                    cursor = (cursor + 1) % 2;

                    if (buf[cursor] == null) buf[cursor] = new byte[bufsize];

                    BytesRead = fs.Read(buf[cursor], 0, bufsize);

                    HaveData.Set();

                    if (BytesRead==0)
                    {
                        fs.Close();
                        return;
                    }

                    crc=Crc32C.Crc32CAlgorithm.Append(crc, buf[cursor], 0, BytesRead);
                }
            }
        }

        public static (bool, uint) parallel_comparefile(String f1, String f2)
        {
            FileCacher fc1, fc2;

            fc1 = new FileCacher(f1);
            fc2 = new FileCacher(f2);

            Task.Factory.StartNew(fc1.Execute);
            Task.Factory.StartNew(fc2.Execute);

            while (true)
            {

                fc2.HaveData.WaitOne();
                fc1.HaveData.WaitOne();

                if (fc1.BytesRead != fc2.BytesRead) return (false,0);
                if (fc1.BytesRead == 0) 
                {
                    return (fc1.crc == fc2.crc, fc1.crc);
                }

                var buf1 = fc1.CurrentBuf;
                var buf2 = fc2.CurrentBuf;

                fc1.NeedData.Set();
                fc2.NeedData.Set();

                if (!SequenceEqual(buf1, buf2)) return (false, 0);
   
            }
        }

        public static bool comparefile(String f1, String f2)
        {
            bool samedisk = f1[0] == f2[0];
           

            try
            {
                var fs1 = new FileStream(f1, FileMode.Open, FileAccess.Read);
                var fs2 = new FileStream(f2, FileMode.Open, FileAccess.Read);

                if (fs1.Length != fs2.Length) return false;

                try
                {
                    
                    byte[] buf1 = new byte[bufsize];
                    byte[] buf2 = new byte[bufsize];

                    while (true)
                    {

                        int bytesRead1=0, bytesRead2=0;

                        if (samedisk)
                        {
                            bytesRead1 = fs1.Read(buf1, 0, bufsize);
                            bytesRead2 = fs2.Read(buf2, 0, bufsize);
                        }
                        else
                        {
                            var j1 = Task.Factory.StartNew(() => { bytesRead1 = fs1.Read(buf1, 0, bufsize);
                                Console.WriteLine("j1 "+Thread.CurrentThread.ManagedThreadId ); 
                            });
                            var j2 = Task.Factory.StartNew(() => { bytesRead2 = fs2.Read(buf2, 0, bufsize);
                                Console.WriteLine("j2 " + Thread.CurrentThread.ManagedThreadId);
                            });

                            Task.WaitAll(j1, j2);
                        }

                        if (bytesRead1 != bytesRead2) return false;
                        if (bytesRead1 == 0) return true;

                        for (int i = 0; i < bytesRead1; i++)
                            if (buf1[i] != buf2[i])
                                return false;
                    }

                }
                finally
                {
                    fs1.Close();
                    fs2.Close();
                }
            }
            catch (Exception ex)
            {
                //textBox4.Text += ex.Message + "\r\n\r\n";
                return false;
            }
        }


    }
}
