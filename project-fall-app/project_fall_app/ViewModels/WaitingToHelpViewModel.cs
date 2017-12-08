using System.Collections.Generic;
using Plugin.FirebasePushNotification;
using Xamarin.Forms;
using XLabs.Ioc;

namespace project_fall_app.ViewModels
{
    class WaitingToHelpViewModel : BaseViewModel
    {
        private List<string> citizenList;
        private IMessagingCenter mscntr;

        public WaitingToHelpViewModel()
        {
            mscntr = Resolver.Resolve<IMessagingCenter>();

            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN: {p.Token}"); //TODO: send userid og token til server
            };

            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
#if __ANDROID__
                FirebasePushNotificationManager.NotificationContentTitleKey = "Kan du hjælpe?";
#endif
            };
            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                mscntr.Send(this, "helpRequested");
            };
        }

        public List<string> CitizenList
        {
            get { return citizenList; }
            set { SetProperty(ref citizenList, value); }
        }
    }
}