using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Android.App;
using Android.Content;
using project_fall_app.Droid;
using project_fall_app.Models;
using project_fall_app.Views;
using Plugin.FirebasePushNotification;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using View = Xamarin.Forms.View;
#if __ANDROID__
    using Android.Content;
    using Android.Preferences;
#endif

namespace project_fall_app.ViewModels
{
    class MainPageViewModel : BaseViewModel
    {
        #region Fields

        private int topBarHeight;
        private View pageContent;
        private int topBarLabelWidth;
        private string token;

        private User currentUser; //store here, we can always access this if we need the user after the initial login has been done

        private IDevice device;
        private IMessagingCenter mscntr;

        #endregion

        public MainPageViewModel()
        {
            token = string.Empty;
            device = Resolver.Resolve<IDevice>();
            mscntr = Resolver.Resolve<IMessagingCenter>();

            InitMessages();
            if (IsUserLoggedIn())
            {
                mscntr.Send(this, "performLogin", currentUser);
            }
            else
            {
                PageContent = new LogInView(); //Startup screen, dont change pls, containted within the mainpage thingy
            }

            Title = "Falddetektions-app";
            TopBarHeight = 50;
            TopBarLabelWidth = (int) (device.Display.Width * 0.9f);

            if (!CheckServerConnection())
            {
                //TODO: maybe add some kind of popup to handle this?
            }
        }
        private bool IsUserLoggedIn()
        {
            #if __ANDROID__
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Xamarin.Forms.Forms.Context);
            ISharedPreferencesEditor edit = prefs.Edit();
            edit.PutBoolean("isUserLoggedIn", true);
            edit.PutString("currentUserID", "2");
            edit.PutString("currentUserName", "fru jensen");
            edit.PutString("currentUserType", "citizen");
            edit.Apply();

            if (prefs.GetBoolean("isUserLoggedIn", false))
            {
                currentUser.ID = prefs.GetString("currentUserID", "-1");
                currentUser.Name = prefs.GetString("currentUserName", "-1");
                currentUser.Type = (User.UserTypes)Enum.Parse(typeof(User.UserTypes), prefs.GetString("currentUserType", "-1"));
                return true;
            }
            #endif
            return false;
        }


        private bool CheckServerConnection()
        {
            //TODO:perform a check of whether or not the server is available
            return true;
        }


        #region MessageCenter

        private void InitMessages()
        {
            //Subscriptions
            MessagingCenter.Subscribe<LogInViewModel, User>(this, "performLogin", (sender, userobj) =>
            {
                //TODO implement proper login
                currentUser = userobj;
                switch (currentUser.Type)
                {
                    case User.UserTypes.citizen:
                        shiftHelp();
                        break;
                    case User.UserTypes.contact:
                        shiftWaitingToHelp();
                        break;
                }
                
            });

            MessagingCenter.Subscribe<HelpViewModel>(this, "callForHelp", (sender) =>
            {
                shiftFallResponse(); 
            });


            MessagingCenter.Subscribe<FallResponseViewModel>(this, "callForHelpConfirmed", (sender) =>
            {
                //TODO implement server call
                shiftHelp();
            });

            MessagingCenter.Subscribe<FallResponseViewModel>(this, "callForHelpAborted", (sender) => { shiftHelp(); });

            MessagingCenter.Subscribe<MessageResponseViewModel>(this, "canHelp", (sender) =>
            {
                //TODO: implement telling the server that user id XXXX can help
            });

            MessagingCenter.Subscribe<MessageResponseViewModel>(this, "helpRequested",
                (sender) => { shiftMessageResponse(); });

            MessagingCenter.Subscribe<MainActivity, string>(this, "tokenRefreshed", (sender, value) =>
            {
                if (value != null)
                {
                    Token = value;
                }
            });

            //send messages
            InfoButtonCommand = new Command(() =>
            {
                var clpman = (ClipboardManager) Forms.Context.GetSystemService(Context.ClipboardService);

                mscntr.Send<MainPageViewModel, string>(this, "showInfoAlert", Token);
            });
        }

        #endregion


        #region page-shift

        private void shiftFallResponse()
        {
            Title = "Skal du bruge hjælp?";
            PageContent = new FallResponseView();
        }

        private void shiftLogIn()
        {
            Title = "Log på";
            PageContent = new LogInView();
        }

        private void shiftMessageResponse()
        {
            Title = "Kan du hjælpe?";
            PageContent = new MessageResponseView();
        }

        private void shiftHelp()
        {
            Title = "Skal du bruge hjælp?";
            PageContent = new HelpView();
        }

        private void shiftWaitingToHelp()
        {
            Title = "Du er klar til at hjælpe";
            PageContent = new WaitingToHelpView();
        }

        #endregion


        #region gettersetter/commands

        public View PageContent
        {
            get { return pageContent; }
            set { SetProperty(ref pageContent, value); }
        }

        public int TopBarHeight
        {
            get { return topBarHeight; }
            set { SetProperty(ref topBarHeight, value); }
        }

        public int TopBarLabelWidth
        {
            get { return topBarLabelWidth; }
            set { SetProperty(ref topBarLabelWidth, value); }
        }

        public string Token
        {
            get { return token; }
            set { SetProperty(ref token, value); }
        }

        public ICommand InfoButtonCommand { protected set; get; }

        #endregion
    }
}