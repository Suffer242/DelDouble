using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FindDoubles
{ 
    class FileComparers
    {
        private const int Bufsize = 100 * 1024 * 1024; // 100 mb
    class FileCacher
    {
        byte[][] buf;

        FileStream fs;
        private int _cursor = 1;
        private int _bytesRead;

        bool cancel = false;

        public uint Crc { get; private set; }


        AutoResetEvent HaveData { get;  set; }
        AutoResetEvent NeedData { get; set; }

        public FileCacher(String fn)
        {
            buf = new byte[2][];

            NeedData = new AutoResetEvent(true);
            HaveData = new AutoResetEvent(false);
            fs = new FileStream(fn, FileMode.Open, FileAccess.Read);
        }

        public void Execute()
        {
            Crc = 0;
            while (true)
            {
                NeedData.WaitOne();

                if (cancel) return;

                _cursor = (_cursor + 1) % 2;

                if (buf[_cursor] == null) buf[_cursor] = new byte[Bufsize];

                _bytesRead = fs.Read(buf[_cursor], 0, Bufsize);

                HaveData.Set();

                if (_bytesRead == 0)
                {
                    fs.Close();
                    return;
                }

                Crc = Crc32C.Crc32CAlgorithm.Append(Crc, buf[_cursor], 0, _bytesRead);
            }
        }

            public byte[] WaitForReadData(out int bytesread)
            {
                HaveData.WaitOne();
                bytesread = _bytesRead;
                var c = _cursor;
                NeedData.Set();
                return buf[c];
               
            }

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


        public static bool hash_comparefile(String fileName1, String fileName2)
        {

            System.Security.Cryptography.HashAlgorithm hash = System.Security.Cryptography.HashAlgorithm.Create();

            byte[] fileHash1;
            byte[] fileHash2=null;

            void SecondHash()
            {
                using (FileStream fileStream2 = new FileStream(fileName2, FileMode.Open)) fileHash2 = hash.ComputeHash(fileStream2);
            }


            bool samedrive = fileName1.Substring(0, 1) == fileName2.Substring(0, 1);

            Task task=null;


            if (samedrive) SecondHash();    
            else
            {
                task = Task.Factory.StartNew(SecondHash);
            }

            using (FileStream fileStream1 = new FileStream(fileName1, FileMode.Open))  fileHash1 = hash.ComputeHash(fileStream1);
                

            if (!samedrive) task.Wait();

            return BitConverter.ToString(fileHash1) == BitConverter.ToString(fileHash2);

        }

            public static (bool, uint) parallel_comparefile(String f1, String f2)
    {
        FileCacher fc1, fc2;

        fc1 = new FileCacher(f1);
        fc2 = new FileCacher(f2);
        int BytesRead1, BytesRead2;

        Task.Factory.StartNew(fc1.Execute);
        Task.Factory.StartNew(fc2.Execute);

        while (true)
        {

            var buf1 = fc2.WaitForReadData(out BytesRead1);
            var buf2 = fc1.WaitForReadData(out BytesRead2);

            if (BytesRead1 != BytesRead2) return (false, 0);
            if (BytesRead1 == 0)
            {
                return (fc1.Crc == fc2.Crc, fc1.Crc);
            }

                if (!SequenceEqual(buf1, buf2))
                {
                    
                    return (false, 0);
                }

        }
    }


  }
}
