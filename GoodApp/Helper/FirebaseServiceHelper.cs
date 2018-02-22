using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using GoodApp.Callback;
using GoodApp.Model;
using GoodApp.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodApp.Helper
{
    public class FirebaseServiceHelper
    {
        private static FirebaseServiceHelper instance;

        FirebaseAuthProvider authProvider;

        Firebase.Auth.User firebaseUser;

        FirebaseClient client;

        FirebaseStorage storageClient;

        private FirebaseServiceHelper()
        {
            authProvider = new FirebaseAuthProvider(new FirebaseConfig(Config.FirebaseClientKey));
        }

        public static FirebaseServiceHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FirebaseServiceHelper();
                }
                return instance;
            }
        }

        public async Task<bool> Register(Model.User user)
        {
            try
            {
                await authProvider.CreateUserWithEmailAndPasswordAsync(user.EmailID, user.Password, user.Username, true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Response> Login(Model.User user)
        {
            try
            {
                var authLink = await authProvider.SignInWithEmailAndPasswordAsync(user.EmailID, user.Password);

                firebaseUser = await authProvider.GetUserAsync(authLink.FirebaseToken);

                client = new FirebaseClient(Config.FirebaseWebApp,
                    new FirebaseOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult<string>(authLink.FirebaseToken)
                    }
                    );

                storageClient = new FirebaseStorage(Config.FirebaseStorageApp,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult<string>(authLink.FirebaseToken)
                    }
                    );

                return new Response { Success = true, Code = (firebaseUser.IsEmailVerified) ? Code.Default : Code.EmailNotVerified };
            }
            catch (Exception ex)
            {
                return new Response { Success = false };
            }
        }

        public async Task<Response> InsetContact(Contact contact)
        {
            try
            {
                await client.Child("Contact").PostAsync<Contact>(contact);

                return new Response { Success = true };
            }
            catch (Exception ex)
            {
                return new Response { Success = false };
            }
        }

        public async Task<Response> GetContacts()
        {
            try
            {
                var result = await client.Child("Contact").OrderByKey().OnceAsync<Contact>();
                var contacts = new ObservableCollection<Contact>();

                foreach (var contact in result)
                {
                    contacts.Add(new Contact { Name = (contact.Object as Contact).Name, EmailID = (contact.Object as Contact).EmailID });
                }

                return new Response { Success = true, Contacts = contacts };
            }
            catch (Exception ex)
            {
                return new Response { Success = false };
            }
        }

        public async void UploadFile(string filePath, UploadFileType uploadFileType, IUploadCallback uploadCallback)
        {
            FileStream fileStream = null;

            try
            {
                await Task.Run(() => { fileStream = File.Open(filePath, FileMode.Open); });
                var fileName = Path.GetFileName(filePath);

                var uploadTask = storageClient.Child(uploadFileType.ToString()).Child(fileName).PutAsync(fileStream);
                uploadTask.Progress.ProgressChanged += (s, e) => uploadCallback.SetUploadPercentage(e.Percentage);

                var downloadLink = await uploadTask;
                
                InsertUploadInfo(new UploadInfo { Type = uploadFileType.ToString(), DownloadLink = downloadLink });
            }
            catch (Exception ex)
            {
                uploadCallback.SetUploadPercentage(-1);
            }
        }

        async void InsertUploadInfo(UploadInfo uploadInfo)
        {
            try
            {
                uploadInfo.DateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                await client.Child("UploadInfo").PostAsync<UploadInfo>(uploadInfo);
            }
            catch (Exception)
            {
            }
        }
        
        public async Task<Response> GetUploadInfos(UploadFileType uploadFileType)
        {
            try
            {
                var result = await client.Child("UploadInfo").OrderByKey().OnceAsync<UploadInfo>();
                var uploadInfos = new ObservableCollection<UploadInfo>();

                foreach (var item in result)
                {
                    var uploadInfo = item.Object as UploadInfo;
                    if (uploadInfo.Type == uploadFileType.ToString())
                    {
                        uploadInfos.Add(new UploadInfo { DateTime = uploadInfo.DateTime, DownloadLink = uploadInfo.DownloadLink });
                    }
                }

                return new Response { Success = true, UploadInfos = uploadInfos };
            }
            catch (Exception ex)
            {
                return new Response { Success = false };
            }
        }
    }
}
