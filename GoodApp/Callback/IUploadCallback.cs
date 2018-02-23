using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodApp.Callback
{
    public interface IUploadCallback
    {
        void SetUploadPercentage(int percentage);

        void UploadCompleted();
    }
}
