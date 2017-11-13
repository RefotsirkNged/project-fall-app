using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Android.Content;
using Android.Widget;
using project_fall_app.Models;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace project_fall_app.ViewModels
{
    class LogInViewModel : BaseViewModel
    {
        private IMessagingCenter mscntr;
        private IDevice dvce;
        public LogInViewModel()
        {
            mscntr = Resolver.Resolve<IMessagingCenter>();
            dvce = Resolver.Resolve<IDevice>();
            User testUser = new User();
            testUser.ID = "1234";
            testUser.Name = "Gerda Gertsen";
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
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        //TODO: if we can get the context somehow, then we can make platform specific stuff like this
                    }
                }

            });
        }


        private bool PerformLogin()
        {
            //TODO: put some code calling the right stuff
            return true;
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
