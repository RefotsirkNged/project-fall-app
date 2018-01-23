using System.Windows.Input;
using Android.Content;
using Android.Content.Res;
using Plugin.FirebasePushNotification;
using Xamarin.Forms;
using XLabs.Ioc;

namespace project_fall_app.ViewModels
{
    class HelpViewModel : BaseViewModel
    {
        private IMessagingCenter mscntr;
        public static bool firstTime;
        public HelpViewModel()
        {
            mscntr = Resolver.Resolve<IMessagingCenter>();
            CallForHelpCommand = new Command(() =>
            {
                mscntr.Send<HelpViewModel>(this, "callForHelp");
            });
            if(!firstTime)
                mscntr.Send<HelpViewModel>(this, "initFirebase");
            firstTime = true;
        }

        public ICommand CallForHelpCommand { protected set; get; }

    }
}
