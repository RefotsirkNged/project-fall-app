﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            citizenList = new List<string>()
            {
                "Bent Bentsen",
                "Carsten Ibsen",
                "August Kørvell",
                "Frederik Voldby",
                "Kristoffer Degn"
            };

            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN: {p.Token}"); //TODO: send userid og token til server
            };

            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                FirebasePushNotificationManager.NotificationContentTitleKey = "Kan du hjælpe " + citizenList.First() + "?";
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