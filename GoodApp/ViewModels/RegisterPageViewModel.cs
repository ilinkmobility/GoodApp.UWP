using GoodApp.Callback;
using GoodApp.Helper;
using Prism.Commands;
using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace GoodApp.ViewModels
{
    public class RegisterPageViewModel : ViewModelBase
    {
        bool _registering;
        public bool Registering
        {
            get { return _registering; }
            set { SetProperty(ref _registering, value); }
        }

        string _username;
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

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

        string _retypePassword;
        public string RetypePassword
        {
            get { return _retypePassword; }
            set { SetProperty(ref _retypePassword, value); }
        }

        private readonly IDialogHelper dialogHelper;

        public ICommand RegisterCommand { get; set; }

        public RegisterPageViewModel(IDialogHelper dialogHelper)
        {
            RegisterCommand = new DelegateCommand(OnRegisterClicked);

            this.dialogHelper = dialogHelper;
        }

        async void OnRegisterClicked()
        {
            if (string.IsNullOrEmpty(Username))
            {
                dialogHelper.ShowMessageDialog("Username cannot be empty!");
                return;
            }

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

            if (string.IsNullOrEmpty(RetypePassword))
            {
                dialogHelper.ShowMessageDialog("Retype Password cannot be empty!");
                return;
            }

            if (!Password.Equals(RetypePassword, StringComparison.CurrentCulture))
            {
                dialogHelper.ShowMessageDialog("Password mismatch!");
                return;
            }

            Registering = true;
            bool result = await FirebaseServiceHelper.Instance.Register(new Model.User { EmailID = EmailID, Password = Password, Username = Username });
            Registering = false;

            if (result)
            {
                dialogHelper.ShowMessageDialog("Verification link sent to your email address. Please verify to activate your account.");
            }
        }
    }
}
