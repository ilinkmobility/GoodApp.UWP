using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodApp.Model
{
    public class UploadInfo
    {
        public string Type { get; set; }

        public string DownloadLink { get; set; }

        public string DateTime { get; set; }

        public string FileName { get { return System.IO.Path.GetFileName(new Uri(DownloadLink).LocalPath); } }
    }
}
