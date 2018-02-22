using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodApp.Callback
{
    public interface IDialogHelper
    {
        void ShowMessageDialog(string message);

        Task<string> PickImageDialog();

        Task<string> PickAudioDialog();
    }
}
