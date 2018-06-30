using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FindDoubles
{
    public class Common
    {

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
            public byte[] buf = new byte[bufsize];

            FileStream fs;
            public int BytesRead;

            public AutoResetEvent NeedData, HaveData;

            public FileCacher(String fn)
            {
                NeedData = new AutoResetEvent(true);
                HaveData = new AutoResetEvent(false);
                fs = new FileStream(fn, FileMode.Open, FileAccess.Read);
            }

            public void Execute()
            {
                while (true)
                {
                    NeedData.WaitOne();
                    BytesRead = fs.Read(buf, 0, bufsize);
                    HaveData.Set();

                    if (BytesRead==0)
                    {
                        fs.Close();
                        return;
                    }
                }
            }
        }

        public static bool parallel_comparefile(String f1, String f2)
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

                if (fc1.BytesRead != fc2.BytesRead) return false;
                if (fc1.BytesRead == 0) return true;


                if (!SequenceEqual(fc1.buf, fc2.buf)) return false;

                //for (int i = 0; i < fc1.BytesRead; i++)
                //    if (fc1.buf[i] != fc2.buf[i])
                //        return false;

                fc1.NeedData.Set();
                fc2.NeedData.Set();
            }
        }

        public static bool comparefile(String f1, String f2)
        {
            bool samedisk = f1[0] == f2[0] && true;
           

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
