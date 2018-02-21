using GoodApp.ViewModels;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GoodApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : BasePage, INotifyPropertyChanged
    {
        public LoginPage()
        {
            this.InitializeComponent();
            DataContextChanged += LoginPage_DataContextChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public LoginPageViewModel ConcreteDataContext
        {
            get
            {
                return DataContext as LoginPageViewModel;
            }
        }

        private void LoginPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(nameof(ConcreteDataContext)));
            }
        }
    }
}
