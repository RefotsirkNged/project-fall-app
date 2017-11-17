using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Android.Content;
using Android.Widget;
using Newtonsoft.Json;
using Org.Json;
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
        public LogInViewModel()
        {
            mscntr = Resolver.Resolve<IMessagingCenter>();
            dvce = Resolver.Resolve<IDevice>();
            ntwrk = Resolver.Resolve<INetwork>();
            User testUser = new User();
            testUser.ID = "1234";
            testUser.Type = Enum.GetName(typeof(User.UserTypes), User.UserTypes.CitizenAssistant);
            LogOnCommand = new Command(() =>
            {
                bool loginSuccessful = PerformLogin();
                if (loginSuccessful)
                {
                    mscntr.Send(this, "performLogin", testUser);
                }
                else
                {
                    //alertbuilder popup showing unsuccessfull
                }

            });
        }


        private bool PerformLogin()
        {
            string url = "https://prbw36cvje.execute-api.us-east-1.amazonaws.com/dev/user";
            string responsetext;
            dynamic responseObject;
            try
            {
                //TODO: put some code calling the right stuff
                //if (ntwrk.IsReachable(url, new TimeSpan(0, 0, 30)).Result)
               // {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                    request.Method = "GET";
                    request.Headers.Add("email", UsernameText);
                    request.Headers.Add("password", PasswordText);

                    using (var response = request.GetResponse())
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            responseObject = JsonConvert.DeserializeObject(reader.ReadToEnd());
                        }
                    }

                if (responseObject.id != -1)
                {
                    
                }
                    return true;
                //}





            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
           
           
            return false;
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
