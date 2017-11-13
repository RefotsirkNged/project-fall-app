using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XLabs.Ioc;

namespace project_fall_app.ViewModels
{
    class LogInViewModel : BaseViewModel
    {
        private IMessagingCenter mscntr;
        public LogInViewModel()
        {
            mscntr = Resolver.Resolve<IMessagingCenter>();
            string[] values = new[] {UsernameText, PasswordText};
            LogOnCommand = new Command(() =>
            {
                mscntr.Send(this, "performLogin",values);
            });
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
