using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Newtonsoft.Json;
#if __ANDROID__
    using Org.Json;
    using Android.Content;
    using Android.Preferences;
#endif
using project_fall_app.Models;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;


namespace project_fall_app.ViewModels
{
    class LogInViewModel : BaseViewModel
    {
        private IMessagingCenter mscntr;
        private IDevice dvce;
        private INetwork ntwrk;
        private User currentUser;

        public LogInViewModel()
        {
            mscntr = Resolver.Resolve<IMessagingCenter>();
            dvce = Resolver.Resolve<IDevice>();
            ntwrk = Resolver.Resolve<INetwork>();


            LogOnCommand = new Command(() =>
            {
                bool loginSuccessful = PerformLogin();
                if (loginSuccessful)
                {
                    mscntr.Send(this, "performLogin", currentUser);
                }
                else
                {
                    Page page = new Page();
                    page.DisplayAlert("Login Error", "Forkert password eller bruger navn", "okay").Start();
                }
            });
        }



        private bool PerformLogin()
        {
        #if __ANDROID__
            string url = "https://prbw36cvje.execute-api.us-east-1.amazonaws.com/dev/user";
            ResponseObject responseObject;

            try
            {
                //TODO: put some code calling the right stuff
                //if (ntwrk.IsReachable(url, new TimeSpan(0, 0, 30)).Result)
                // {
                HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(url);
                request.Method = "GET";
                request.Headers.Add("email", UsernameText);
                request.Headers.Add("password", PasswordText);

                using (var response = request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseObject = JsonConvert.DeserializeObject<ResponseObject>(reader.ReadToEnd());
                    }
                }

                if (responseObject.body.id != "-1")
                {
                    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Xamarin.Forms.Forms.Context);
                    ISharedPreferencesEditor edit = prefs.Edit();

                    currentUser = new User();
                    currentUser.ID = responseObject.body.id;
                    currentUser.Name = responseObject.body.name;
                    currentUser.Type = (User.UserTypes)Enum.Parse(typeof(User.UserTypes), responseObject.body.role);

                    edit.PutBoolean("isUserLoggedIn", true);
                    edit.PutString("currentUserID", responseObject.body.id);
                    edit.PutString("currentUserName", responseObject.body.name);
                    edit.PutString("currentUserType", responseObject.body.role.ToString());
                    edit.Apply();
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            //return false;
        #else 
            return true;
        #endif

        }
        private string usernameText;
        private string passwordText;

        public ICommand LogOnCommand { protected set; get; }

        public string UsernameText
        {
            get { return usernameText; }
            set { SetProperty(ref usernameText, value); }
        }

        public string PasswordText
        {
            get { return passwordText; }
            set { SetProperty(ref passwordText, value); }
        }
    }
}