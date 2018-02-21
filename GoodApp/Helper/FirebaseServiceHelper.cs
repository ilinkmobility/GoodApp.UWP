using Firebase.Auth;
using GoodApp.Model;
using GoodApp.Utility;
using System;
using System.Collections.Generic;
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
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Response> Login(Model.User user)
        {
            try
            {
                var authLink = await authProvider.SignInWithEmailAndPasswordAsync(user.EmailID, user.Password);

                var firebaseUser = await authProvider.GetUserAsync(authLink.FirebaseToken);

                return new Response { Success = true, Code = (firebaseUser.IsEmailVerified) ? Code.Default : Code.EmailNotVerified };
            }
            catch (Exception)
            {
                return new Response { Success = false };
            }
        }
    }
}
