using GoodApp.Callback;
using GoodApp.Helper;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace GoodApp.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        string _emailID;
        public string EmailID
        {
            get { return _emailID; }
            set { SetProperty(ref _emailID, value); }
        }

        string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public ICommand LoginCommand { get; set; }

        public ICommand RegisterCommand { get; set; }

        private readonly IDialogHelper dialogHelper;

        private readonly INavigationService navigationService;

        public LoginPageViewModel(IDialogHelper dialogHelper, INavigationService navigationService)
        {
            LoginCommand = new DelegateCommand(OnLoginClicked);

            RegisterCommand = new DelegateCommand(OnRegisterClicked);
            
            this.navigationService = navigationService;

            this.dialogHelper = dialogHelper;
        }

        async void OnLoginClicked()
        {
            if (string.IsNullOrEmpty(EmailID))
            {
                dialogHelper.ShowMessageDialog("Email ID cannot be empty!");
                return;
            }

            if (!Utility.Utility.IsValidEmailID(EmailID))
            {
                dialogHelper.ShowMessageDialog("Invalid Email ID format!");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                dialogHelper.ShowMessageDialog("Password cannot be empty!");
                return;
            }

            var response = await FirebaseServiceHelper.Instance.Login(new Model.User { EmailID = EmailID, Password = Password });

            if (response.Code == Utility.Code.EmailNotVerified)
            {
                dialogHelper.ShowMessageDialog("Email NOT Verified.");
            }
            else
            {
                dialogHelper.ShowMessageDialog("Success");
            }
        }

        void OnRegisterClicked()
        {
            navigationService.Navigate("Register", null);
        }
    }
}
