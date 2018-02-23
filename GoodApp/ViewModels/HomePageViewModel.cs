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

        string _audioPlayStateIcon;
        public string AudioPlayStateIcon
        {
            get { return _audioPlayStateIcon; }
            set { SetProperty(ref _audioPlayStateIcon, value); }
        }

        ObservableCollection<Contact> _contacts;
        public ObservableCollection<Contact> Contacts
        {
            get { return _contacts; }
            set { SetProperty(ref _contacts, value); }
        }

        UploadInfo _selectedPicture;
        public UploadInfo SelectedPicture
        {
            get { return _selectedPicture; }
            set
            {
                SetProperty(ref _selectedPicture, value);
                IsShowImageViewer = true;
            }
        }

        ObservableCollection<UploadInfo> _pictures;
        public ObservableCollection<UploadInfo> Pictures
        {
            get { return _pictures; }
            set { SetProperty(ref _pictures, value); }
        }

        UploadInfo _selectedAudio;
        public UploadInfo SelectedAudio
        {
            get { return _selectedAudio; }
            set { SetProperty(ref _selectedAudio, value); PlaySelectedAudio(); }
        }

        ObservableCollection<UploadInfo> _audios;
        public ObservableCollection<UploadInfo> Audios
        {
            get { return _audios; }
            set { SetProperty(ref _audios, value); }
        }

        bool _uploading;
        public bool Uploading
        {
            get { return _uploading; }
            set { SetProperty(ref _uploading, value); }
        }

        bool _isShowImageViewer;
        public bool IsShowImageViewer
        {
            get { return _isShowImageViewer; }
            set { SetProperty(ref _isShowImageViewer, value); }
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

        public ICommand AudioBackCommand { get; set; }

        public ICommand AudioForwardCommand { get; set; }

        public ICommand AudioResumePauseCommand { get; set; }

        public ICommand AudioStopCommand { get; set; }

        public ICommand CloseImageViewerCommand { get; set; }

        public ICommand LeftArrowClickedCommand { get; set; }

        public ICommand RightArrowClickedCommand { get; set; }

        private readonly IDialogHelper dialogHelper;

        private readonly INavigationService navigationService;

        public HomePageViewModel(IDialogHelper dialogHelper, INavigationService navigationService)
        {
            InsertContactCommand = new DelegateCommand(OnInsertContactClicked);

            UpdateContactCommand = new DelegateCommand(OnUpdateContactClicked);

            BrowsePictureCommand = new DelegateCommand(OnBrowsePictureClicked);

            BrowseAudioCommand = new DelegateCommand(OnBrowseAudioClicked);

            AudioBackCommand = new DelegateCommand(() => { });

            AudioForwardCommand = new DelegateCommand(() => { });

            AudioResumePauseCommand = new DelegateCommand(OnResumePauseAudio);

            AudioStopCommand = new DelegateCommand(() => { });

            CloseImageViewerCommand = new DelegateCommand(() => { IsShowImageViewer = false; });

            LeftArrowClickedCommand = new DelegateCommand(() => { OnChangeImage(true); });

            RightArrowClickedCommand = new DelegateCommand(() => { OnChangeImage(false); });

            this.navigationService = navigationService;

            this.dialogHelper = dialogHelper;

            RefreshContacts();

            RefreshPictures();

            RefreshAudios();

            AudioPlayStateIcon = "/Assets/AudioPlay.png";
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

        async void RefreshPictures()
        {
            Pictures = await GetDownloadLinks(UploadFileType.Picture);
        }

        async void RefreshAudios()
        {
            Audios = await GetDownloadLinks(UploadFileType.Audio);
        }

        public void SetUploadPercentage(int percentage)
        {
            if (percentage == -1 || percentage == 100)
            {
                Uploading = false;

                if (percentage == 100)
                {
                    //dialogHelper.ShowMessageDialog("File uploaded successflly!");
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

        public void UploadCompleted()
        {
            RefreshPictures();

            RefreshAudios();
        }

        void OnChangeImage(bool isBack)
        {
            if (SelectedPicture != null)
            {
                if (isBack)
                {
                    int index = Pictures.IndexOf(SelectedPicture);
                    if (index != 0)
                    {
                        SelectedPicture = Pictures[index - 1];
                    }
                }
                else
                {
                    int index = Pictures.IndexOf(SelectedPicture);
                    if (index != (Pictures.Count-1))
                    {
                        SelectedPicture = Pictures[index + 1];
                    }
                }
            }
        }

        void PlaySelectedAudio()
        {
            MediaPlayerHelper.Instance.StartAudio(SelectedAudio.DownloadLink);
            MediaPlayerHelper.Instance.IsPlaying = true;
            AudioPlayStateIcon = "/Assets/AudioPause.png";
        }

        void OnResumePauseAudio()
        {
            if (SelectedAudio != null)
            {
                if (MediaPlayerHelper.Instance.IsPlaying)
                {
                    MediaPlayerHelper.Instance.Pause();
                    AudioPlayStateIcon = "/Assets/AudioPlay.png";
                }
                else
                {
                    MediaPlayerHelper.Instance.Resume();
                    AudioPlayStateIcon = "/Assets/AudioPause.png";
                }
            }
        }
    }
}
