using GoodApp.Callback;
using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace GoodApp.Views
{
    public class BasePage : SessionStateAwarePage, IDialogHelper
    {
        public async void ShowMessageDialog(string message)
        {
            await new MessageDialog(message).ShowAsync();
        }
    }
}
