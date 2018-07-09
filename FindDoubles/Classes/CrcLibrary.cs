using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindDoubles.Classes
{
    class CrcLibrary
    {
        IDictionary<String, uint> _crclibrary;
        bool _crclibrarychanged = false;
        ICrcStore _store;

        public CrcLibrary(ICrcStore Store = null, IDictionary<String, uint> Dictionary = null)
        {
            _store = Store;
            _crclibrary = Dictionary!=null ? Dictionary : new Dictionary<String, uint>();
            _store.LoadCRC(_crclibrary);
        }

        public void SaveCRC()
        {
            if (_crclibrarychanged) _store.SaveCRC(_crclibrary);
        }

        public void Add(String key, uint value)
        {
            _crclibrary.Add(key, value);
            _crclibrarychanged = true;
        }

        public (bool,uint) Get(String key)
        {
            uint result;
            return (_crclibrary.TryGetValue(key, out result), result);   
        }
    }

    public interface ICrcStore
    {
         void LoadCRC(IDictionary<String, uint> _crclibrary);
         void SaveCRC(IDictionary<String, uint> _crclibrary);
    }



    public class CrcIniStore : ICrcStore
    {
        String _filename;
        public CrcIniStore(String FileName)
        {
            _filename = FileName;
        }
        public void LoadCRC(IDictionary<String, uint> _crclibrary)
        {
            if (File.Exists(_filename))
                foreach (var line in File.ReadAllLines(_filename))
                {
                    var p = line.LastIndexOf('=');
                    _crclibrary.Add(line.Substring(0, p), uint.Parse(line.Substring(p + 1)));
                }
        }

        public void SaveCRC(IDictionary<String, uint> _crclibrary)
        { 

                File.WriteAllLines(_filename, _crclibrary.Select(f => f.Key + "=" + f.Value));
        }
    }
}
