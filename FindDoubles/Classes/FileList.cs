using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace FindDoubles
{

    public class FileData
    {
        public String Name { set; get; }
        public uint Crc { set; get; }
        public long Size { set; get; }

        public bool Deleted { set; get; }

        public override string ToString()
        {
            return Name + " [" + (Size / (1024 * 1024)) + "mb]";
        }

    }

    public class DoubleFileSet : IEnumerable<FileData>
    {
        public FileData[] Files { get; }
        public DoubleFileSet(IEnumerable<FileData> src) => Files = src.ToArray();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Files.GetEnumerator();
        }

        public IEnumerator<FileData> GetEnumerator()
        {
            return ((IEnumerable<FileData>)Files).GetEnumerator();
        }
    }

    public class DirectoryData
    {
        public String Name { set; get; }
        public int DoubleCnt { set; get; }
        public int Cnt { set; get; }

    }

    public interface ICrcCalculator
    {
        uint Calculate(String filename);
    }

    public class CrcCalculator : ICrcCalculator
    {
        const int Crclength = 2014;
        readonly CrcLibrary _crclibrary;
        public CrcCalculator(CrcLibrary crcLibrary)
        {
            _crclibrary = crcLibrary;
        }

        public uint Calculate(string fileName)
        {
            if (_crclibrary.Get(fileName, out var crc)) return crc;

            byte[] buf1 = new byte[Crclength];

            var fs1 = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            fs1.Read(buf1, 0, Crclength);
            crc = Crc32C.Crc32CAlgorithm.Compute(buf1);

            fs1.Seek(Crclength, SeekOrigin.End);
            fs1.Read(buf1, 0, Crclength);
            Crc32C.Crc32CAlgorithm.Append(crc, buf1);

            _crclibrary.Add(fileName, crc);

            return crc;
        }
    }

    public class FileList
    {
        readonly ConcurrentBag<FileData> _filelist;
        readonly uint _minsize;
        readonly ICrcCalculator _сrccalculator;

        List<DoubleFileSet> _doubleset;

        List<DoubleFileSet> Doubleset
        {
            get
            {
                return _doubleset ?? (_doubleset = _filelist.GroupBy(f => new {CRC = f.Crc, f.Size}).Where(f => f.Count() > 1)
                           .Select(f => new DoubleFileSet(f)).ToList());
            }
        }

        public Action<String> CallBack { set; get; }
        public Action<String> ErrorCallBack { set; get; }

        public Func<FileData,FileData,bool> FileComparer { set; get; }

        public FileList(uint minSize, ICrcCalculator crcCalculator)
        {
            _minsize = minSize;
            _сrccalculator = crcCalculator;
            _filelist = new ConcurrentBag<FileData>();
        }


        public void AddDirectory(String dir)
        {
            CallBack?.Invoke(dir);
            _doubleset = null;

            DirectoryInfo source = new DirectoryInfo(dir);
            FileInfo[] files = source.GetFiles();

            foreach (var s in files)
            {

                if (s.Length > _minsize * 1024 * 1024)
                {

                    _filelist.Add(new FileData { Name = s.FullName, Size = s.Length, Crc = _сrccalculator.Calculate(s.FullName) });
                }
            }

            try
            {
                foreach (var st in Directory.GetDirectories(dir))
                    try
                    {
                        AddDirectory(st);
                    }
                    catch (Exception ex)
                    {
                        ErrorCallBack?.Invoke(st + ": " + ex.Message);
                    }
            }
            catch (Exception ex)
            {
                ErrorCallBack?.Invoke( dir + ": " + ex.Message);
            }
        }



        IEnumerable<DirectoryData> DirectoryList(IEnumerable<DoubleFileSet> src)
        {
            return src.SelectMany(f => f.Select(ff => ff.Name)).Distinct()
                         .GroupBy(Path.GetDirectoryName)
                         .Select(f => new DirectoryData { Name = f.Key, DoubleCnt = f.Count(), Cnt = Directory.EnumerateFiles(f.Key).Count() })
                         .OrderBy(f => f.Name);
        }

        public IEnumerable<DirectoryData> DoubleDirectoryList => DirectoryList(Doubleset);

        public IEnumerable<DirectoryData> GetDoubleDirectoryListOfDirecroty(String path) => DirectoryList(Doubleset.Where(f => f.FirstOrDefault(g => Path.GetDirectoryName(g.Name) == path) != null ));

        public IEnumerable<FileData> GetFileToDelete(String path)
        {
           if (FileComparer != null) 
            foreach (DoubleFileSet fileset in Doubleset.Where(f => f.FirstOrDefault(g => Path.GetDirectoryName(g.Name) == path && !g.Deleted) != null) )
            {
                foreach ( FileData delfile in fileset.Where(f => Path.GetDirectoryName(f.Name) == path) )
                {
                    var checkfile = fileset.FirstOrDefault(f => Path.GetDirectoryName(f.Name) != path && !f.Deleted);

                        if (FileComparer(delfile, checkfile)) yield return delfile;
                }
            }
        }


    }
}
