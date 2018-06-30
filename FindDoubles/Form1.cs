using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace FindDoubles
{
    public partial class Form1 : Form
    {

        int crclength = 1024;

        List<List<FileData>> dbls;
        public Form1()
        {
            InitializeComponent();
        }


        public static bool comparefile(String f1, String f2)
        { 

            const int bufsize = 100 * 1024 * 1024; // 100 mb

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
                        int bytesRead1 = fs1.Read(buf1, 0, bufsize);
                        int bytesRead2 = fs2.Read(buf2, 0, bufsize);

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

        int DifCnt(String f1, String f2)
        {
            const int bufsize = 0xffff;

            int dcnt = 0; int rcnt = 0; long pos = 0;
            var fs1 = new FileStream(f1, FileMode.Open);
            var fs2 = new FileStream(f2, FileMode.Open);
            try
            {
                byte[] buf1 = new byte[bufsize];
                byte[] buf2 = new byte[bufsize];

                while (0 < fs1.Read(buf1, 0, bufsize))
                {
                    rcnt = fs2.Read(buf2, 0, bufsize);



                    for (int i = 0; i < rcnt; i++) if (buf1[i] != buf2[i])
                        {
                            dcnt++;
                            //listBox1.Items.Add((i + pos).ToString() + " " + buf1[i].ToString() +  " " +buf2[i].ToString());
                        }

                    pos += rcnt;
                }


                return dcnt;
            }
            finally
            {
                fs1.Close();
                fs2.Close();
            }

        }


        bool crclibrarychanged = false;
        public ConcurrentDictionary<String, uint> crclibrary;

        uint CompureCrc(String FileName)
        {
            uint crc;

            if (crclibrary.TryGetValue(FileName, out crc)) return crc;

            byte[] buf1 = new byte[crclength];

            var fs1 = new FileStream(FileName, FileMode.Open, FileAccess.Read);
     
            fs1.Read(buf1, 0, crclength);
            crc = Crc32C.Crc32CAlgorithm.Compute(buf1);

            fs1.Seek(crclength, SeekOrigin.End);
            fs1.Read(buf1, 0, crclength);
            Crc32C.Crc32CAlgorithm.Append(crc,buf1);

            crclibrary.TryAdd(FileName, crc);
            crclibrarychanged = true;

            return crc;
        }


        uint CompureFullCrc(String FileName)
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
        void SaveCRC()
        {
            if (crclibrarychanged)
             File.WriteAllLines("crc" + crclength + ".txt", crclibrary.Select(f => f.Key + "=" + f.Value));
        }


        void LoadCRC()
        {
            crclibrary = new ConcurrentDictionary<String, uint>();
            var filename = "crc" + crclength + ".txt";

            if (File.Exists(filename))
                foreach (var line in File.ReadAllLines(filename))
                {
                    var p = line.LastIndexOf('=');
                    crclibrary.TryAdd(line.Substring(0, p), uint.Parse(line.Substring(p + 1)));
                }
        }



        class FileData
        {
            public String Name { set; get; }
            public uint CRC { set; get; }
            public long Size { set; get; }

            public bool Deleted { set; get; }

            public override string ToString()
            {
                return Name + " [" + (Size / (1024 * 1024)) + "mb]";
            }

        }


        ConcurrentBag<FileData> Filelist;
        SortedList<long, List<FileData>> sl = new SortedList<long, List<FileData>>();

        void adddir(object dir)
        {


            Invoke((Action)(
                 () => {
                    label3.Text = dir.ToString();
                 }
                )
                );

            //try
            //{
            DirectoryInfo source = new DirectoryInfo(dir.ToString());
            FileInfo[] files = source.GetFiles();

            foreach (var s in files)
            {



                if (s.Length > min_mb.Value  * 1024 * 1024)
                {

                    Filelist.Add(new FileData { Name = s.FullName, Size = s.Length, CRC = CompureCrc(s.FullName) });
                }
            }

            try
            {
                foreach (var st in Directory.GetDirectories(dir.ToString()))
                    try
                    {
                        adddir(st);
                    }
                    catch (Exception ex)
                    {

                        BeginInvoke((Action)(
                         () => { error.Text += st+" - "+ex.Message + "\r\n\r\n"; }
                        )
                        );
                    }
            }
            catch (Exception ex )
            {

                BeginInvoke( (Action)(
                 ()=> { error.Text += dir+" - "+ex.Message + "\r\n\r\n"; }
                )
                );
            }

            


        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Task.Factory.StartNew(adddir, (object)@"g:\Adult\");
            //Task.Factory.StartNew(adddir, (object)@"k:\Private\");

            LoadCRC();
            Filelist = new ConcurrentBag<FileData>();

            Task.Factory.StartNew(() =>
           {
               var g = textBox2.Lines.GroupBy(f => f.Substring(1)).Select(f=>f.ToList()).ToList();
               Parallel.ForEach(g, (f)=>
               {
                   f.ToList().ForEach(adddir);
               });

               SaveCRC();

               //foreach (var s in textBox2.Lines) adddir(s);

               long totalsize = 0;

               dbls = Filelist.GroupBy(f => new { f.CRC, f.Size }).Where(f => f.Count() > 1).Select(f => f.ToList()).ToList();


               File.WriteAllLines("finded_files.txt", Filelist.Select(f=>f.ToString()) );

               File.WriteAllLines("finded_sets.txt", dbls.Select(f => String.Join("\r\n", f) + "\r\n\r\n"));

               var alldbls = dbls.SelectMany(f => f.Select(ff => ff.Name)).Distinct()
               .GroupBy(f => Path.GetDirectoryName(f))
               .Select(f => new { Path = f.Key, Count = f.Count(), Total = Directory.EnumerateFiles(f.Key).Count() })
               // .OrderByDescending(f => (float)f.Count / f.Total)
               .OrderBy(f => f.Path)
               .ToList();




               String addtext = null;

               foreach (var d in dbls)
               {
                   //foreach (var l in d) addtext += "\r\n" + l.Name;
                   //addtext += "\r\n";
                   foreach (var l in d.Skip(1)) totalsize += l.Size;
               }


               Invoke((Action)(() =>
               {

                   label3.Text = totalsize / (1024 * 1024) + " MB";
                   Dirs.DataSource = alldbls;
               }));

               /*
                              long totalsize = 0;
                              foreach (var list in sl)
                                  if (list.Value.Count > 1)
                                  {
                                      for (int i = 0; i < list.Value.Count; i++)
                                          if (list.Value[i] != null)
                                              for (int j = i + 1; j < list.Value.Count; j++)
                                                  if (list.Value[j] != null)
                                                  {
                                                      if (comparefile(list.Value[i], list.Value[j]))
                                                      {
                                                          totalsize += list.Key;

                                                          var addtext = "\r\n" + list.Value[i]
                                                              + "\r\n" + list.Value[j]
                                                              + "\r\n";

                                                          Invoke((Action)(() =>
                                                             {
                                                                 textBox1.Text += addtext;
                                                                 label3.Text = totalsize / (1024 * 1024) + " MB";
                                                             }));

                                                           //textBox1.Text += "\r\n" + list.Value[i];
                                                           //textBox1.Text += "\r\n" + list.Value[j];
                                                           //textBox1.Text += "\r\n";

                                                           list.Value[j] = null;

                                                      }
                                                  }

                                      Thread.Sleep(0);

                                  }
                                  */
           }
            );




        }

        private void button2_Click(object sender, EventArgs e)
        {


        }

        private void Dirs_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void Dirs_SelectionChanged(object sender, EventArgs e)
        {
            string addtext = "";
            if (Dirs.SelectedRows.Count > 0)
            {
                var path = Dirs.SelectedRows[0].Cells[0].Value.ToString();

                int cnt = 0;
                foreach (var set in dbls)
                {
                    if (set.Count(f => Path.GetDirectoryName(f.Name) == path) > 0)
                    {
                        addtext += ++cnt + ".\r\n";
                        foreach (var l in set) addtext += l.Name + "\r\n";
                        //addtext += "\r\n";
                    }
                }

                log.Text = addtext;


                var alldbls = dbls.Where(f => f.Count(g => Path.GetDirectoryName(g.Name) == path) > 0)

                    .SelectMany(f => f.Select(ff => ff.Name)).Distinct()
                .GroupBy(f => Path.GetDirectoryName(f))
                .Select(f => new { Path = f.Key, Count = f.Count(), Total = Directory.EnumerateFiles(f.Key).Count() })
                .OrderByDescending(f => (float)f.Count / f.Total)
                .ToList();

                detail.DataSource = alldbls;

            }
        }


        public void Log(string log)
        {
            File.AppendAllText(@"Delete_log.txt", DateTime.Now.ToString() + " : " + log + "\r\n");

        }



        void DeleteOnPath(String path)
        {
            try
            {


                if (path[path.Length - 1] == '\\') path = path.Substring(0, path.Length - 1);

                log.Text = "";
                int cnt = 0;

                foreach (var set in dbls)
                {
                    if (set.Count(f => Path.GetDirectoryName(f.Name) == path) > 0)
                    {
                        foreach (var delfile in set.Where(f => Path.GetDirectoryName(f.Name) == path).ToList())
                        {

                            var exits_cnt = set.Count(f => !f.Deleted);

                            var checkfile = set.FirstOrDefault(f => Path.GetDirectoryName(f.Name) != path && !f.Deleted);

                            if (exits_cnt > 1)
                            {
                                log.Text += ++cnt + ") " + delfile.Name + "\r\n";


                                //var dst = "i" + delfile.Name.Substring(1);

                                try
                                {
                                    //  Directory.CreateDirectory(Path.GetDirectoryName(dst));

                                    if (delfile.Name.ToLower() != checkfile.Name.ToLower())
                                    {
                                        var crc1 = CompureFullCrc(delfile.Name);
                                        var crc2 = CompureFullCrc(checkfile.Name);

                                        if (crc1 == crc2)
                                        {
                                            Log("delete: " + delfile.Name + String.Format(" crc:{0:X8}", crc1) + " with compare: " + checkfile.Name);

                                            log.AppendText("delete: " + delfile.Name + "\r\n");
                                            File.Delete(delfile.Name);
                                        }

                                        delfile.Deleted = true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    error.Text += "!!! err " + delfile.Name + "  - " + ex.Message + "\r\n";
                                    log.Text += "!!! err " + delfile.Name + "  - " + ex.Message + "\r\n";
                                }
                            }
                            else
                            {
                                log.Text += "> " + delfile.Name + "\r\n";
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {
               

                MessageBox.Show(ex.Message);
            }

        }

        private void Dirs_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Dirs.SelectedRows.Count == 0) return;
            var path = Dirs.SelectedRows[0].Cells[0].Value.ToString();
            Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            });
        }


        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                DeleteOnPath(Dirs.SelectedRows[0].Cells[0].Value.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteOnPath(detail.SelectedRows[0].Cells[0].Value.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DeleteOnPath(PathText.Text);

            foreach (var dir in Directory.EnumerateDirectories(PathText.Text, "*.*", SearchOption.AllDirectories))
            {
                DeleteOnPath(dir);
            }
        }

        private void detail_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (detail.SelectedRows.Count == 0) return;
            var path = detail.SelectedRows[0].Cells[0].Value.ToString();
            Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            });
        }
    }
}
