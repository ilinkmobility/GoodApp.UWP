using GoodApp.Callback;
using GoodApp.Helper;
using GoodApp.Model;
using GoodApp.Utility;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GoodApp.ViewModels
{
    public class HomePageViewModel : ViewModelBase, IUploadCallback
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        string _emailID;
        public string EmailID
        {
            get { return _emailID; }
            set { SetProperty(ref _emailID, value); }
        }

        string _pictureFilePath;
        public string PictureFilePath
        {
            get { return _pictureFilePath; }
            set { SetProperty(ref _pictureFilePath, value); }
        }

        string _audioFilePath;
        public string AudioFilePath
        {
            get { return _audioFilePath; }
            set { SetProperty(ref _audioFilePath, value); }
        }

        ObservableCollection<Contact> _contacts;
        public ObservableCollection<Contact> Contacts
        {
            get { return _contacts; }
            set { SetProperty(ref _contacts, value); }
        }

        ObservableCollection<UploadInfo> _pictures;
        public ObservableCollection<UploadInfo> Pictures
        {
            get { return _pictures; }
            set { SetProperty(ref _pictures, value); }
        }

        bool _uploading;
        public bool Uploading
        {
            get { return _uploading; }
            set { SetProperty(ref _uploading, value); }
        }

        int _uploadPercentage;
        public int UploadPercentage
        {
            get { return _uploadPercentage; }
            set { SetProperty(ref _uploadPercentage, value); }
        }

        public ICommand InsertContactCommand { get; set; }

        public ICommand UpdateContactCommand { get; set; }

        public ICommand BrowsePictureCommand { get; set; }

        public ICommand BrowseAudioCommand { get; set; }

        private readonly IDialogHelper dialogHelper;

        private readonly INavigationService navigationService;

        public HomePageViewModel(IDialogHelper dialogHelper, INavigationService navigationService)
        {
            InsertContactCommand = new DelegateCommand(OnInsertContactClicked);

            UpdateContactCommand = new DelegateCommand(OnUpdateContactClicked);

            BrowsePictureCommand = new DelegateCommand(OnBrowsePictureClicked);

            BrowseAudioCommand = new DelegateCommand(OnBrowseAudioClicked);

            this.navigationService = navigationService;

            this.dialogHelper = dialogHelper;

            RefreshContacts();

            RefreshPicturess();
        }

        async void OnInsertContactClicked()
        {
            if (string.IsNullOrEmpty(Name))
            {
                dialogHelper.ShowMessageDialog("Name cannot be empty!");
                return;
            }

            if (string.IsNullOrEmpty(EmailID))
            {
                dialogHelper.ShowMessageDialog("Email ID cannot be empty!");
                return;
            }

            var resultInsert = await FirebaseServiceHelper.Instance.InsetContact(new Contact { Name = Name, EmailID = EmailID });

            if (resultInsert.Success)
            {
                RefreshContacts();
            }
            else
            {
                dialogHelper.ShowMessageDialog("Insert failed!");
            }
        }

        void OnUpdateContactClicked()
        {
            dialogHelper.ShowMessageDialog("Contact Clicked");
        }

        async void OnBrowsePictureClicked()
        {
            var selectedFilePath = await dialogHelper.PickImageDialog();

            if (selectedFilePath != null)
            {
                PictureFilePath = selectedFilePath;

                Uploading = true;

                FirebaseServiceHelper.Instance.UploadFile(PictureFilePath, UploadFileType.Picture, this);
            }
        }

        async void OnBrowseAudioClicked()
        {
            var selectedFilePath = await dialogHelper.PickAudioDialog();

            if (selectedFilePath != null)
            {
                AudioFilePath = selectedFilePath;

                Uploading = true;

                FirebaseServiceHelper.Instance.UploadFile(AudioFilePath, UploadFileType.Audio, this);
            }
        }

        async void RefreshContacts()
        {
            var resultContacts = await FirebaseServiceHelper.Instance.GetContacts();

            if (resultContacts.Success)
            {
                Contacts = resultContacts.Contacts;
            }
        }

        async void RefreshPicturess()
        {
            Pictures = await GetDownloadLinks(UploadFileType.Picture);
        }

        public void SetUploadPercentage(int percentage)
        {
            if (percentage == -1 || percentage == 100)
            {
                Uploading = false;

                if (percentage == 100)
                {
                    //dialogHelper.ShowMessageDialog("File uploaded successflly!");

                    RefreshPicturess();
                }
                return;
            }

            UploadPercentage = percentage;
        }

        async Task<ObservableCollection<UploadInfo>> GetDownloadLinks(UploadFileType uploadFileType)
        {
            var response = await FirebaseServiceHelper.Instance.GetUploadInfos(uploadFileType);
            if (response.Success)
            {
                return response.UploadInfos;
            }

            return new ObservableCollection<UploadInfo>();
        }
    }
}
