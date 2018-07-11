using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace FindDoubles
{
    public class CrcLibrary
    {
        readonly IDictionary<String, uint> _crclibrary;
        bool _crclibrarychanged;
        readonly ICrcStore _store;

        public CrcLibrary(ICrcStore store, IDictionary<String, uint> dictionary = null)
        {
            _store = store;
            _crclibrary = dictionary ?? new Dictionary<String, uint>();
            _store.LoadCrc(_crclibrary);
        }

        public void SaveCrc()
        {
            if (_crclibrarychanged) _store.SaveCrc(_crclibrary);
        }

        public void Add(String key, uint value)
        {
            _crclibrary.Add(key, value);
            _crclibrarychanged = true;
        }

        public bool Get(String key, out uint result)
        {
            return _crclibrary.TryGetValue(key, out result);
        }
    }

    public interface ICrcStore
    {
         void LoadCrc(IDictionary<String, uint> crclibrary);
         void SaveCrc(IDictionary<String, uint> crclibrary);
    }



    public class CrcIniStore : ICrcStore
    {
        readonly String _filename;
        public CrcIniStore(String fileName)
        {
            _filename = fileName;
        }
        public void LoadCrc(IDictionary<String, uint> crclibrary)
        {
            if (File.Exists(_filename))
                foreach (var line in File.ReadAllLines(_filename))
                {
                    var p = line.LastIndexOf('=');
                    crclibrary.Add(line.Substring(0, p), uint.Parse(line.Substring(p + 1)));
                }
        }

        public void SaveCrc(IDictionary<String, uint> crclibrary)
        { 

                File.WriteAllLines(_filename, crclibrary.Select(f => f.Key + "=" + f.Value));
        }
    }
}
