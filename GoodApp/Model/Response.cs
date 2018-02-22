using GoodApp.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodApp.Model
{
    public class Response
    {
        public bool Success { get; set; }

        public Code Code { get; set; }

        public ObservableCollection<Contact> Contacts { get; set; }

        public ObservableCollection<UploadInfo> UploadInfos { get; set; }
    }
}
