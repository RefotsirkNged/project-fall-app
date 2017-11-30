using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
#if __ANDROID__ 
    using Android.Content;
    using project_fall_app.Droid;
#endif
using project_fall_app.Models;
using project_fall_app.Views;
using Plugin.FirebasePushNotification;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using View = Xamarin.Forms.View;
using PCLStorage;
using System.IO;

namespace project_fall_app.ViewModels
{
    class MainPageViewModel : BaseViewModel
    {
        #region Fields

        private int topBarHeight;
        private View pageContent;
        private int topBarLabelWidth;
        private string token;

        private User currentUser = null; //store here, we can always access this if we need the user after the initial login has been done

        private IDevice device;
        private IMessagingCenter mscntr;

        #endregion

        public MainPageViewModel()
        {
            token = string.Empty;
            device = Resolver.Resolve<IDevice>();
            mscntr = Resolver.Resolve<IMessagingCenter>();

            InitMessages();
            //if (IsUserLoggedIn())
            //{
            //    mscntr.Send(this, "performLogin", currentUser);
            //}
            //else
            //{
            //    PageContent = new LogInView(); //Startup screen, dont change pls, containted within the mainpage thingy
            //}

            VerifyUserCredentialFileExistence();

            //if (currentUser != null)
            //    mscntr.Send(this, "performLogin", currentUser);
            //else 
            //    PageContent = new LogInView(); 


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




            return false;

        }

        private async Task VerifyUserCredentialFileExistence()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            ExistenceCheckResult fileExist = await rootFolder.CheckExistsAsync("userCredentials.txt");

            if (fileExist == ExistenceCheckResult.FileExists)
            {
                IFile userFile = await rootFolder.CreateFileAsync("userCredentials.txt", CreationCollisionOption.OpenIfExists);
                String fileContext = await userFile.ReadAllTextAsync();

                //TODO replace \ with \\ in login

                String[] split = fileContext.Split('\n');
                currentUser.ID = split[0];
                currentUser.Name = split[1];
                currentUser.Type = (User.UserTypes) Enum.Parse(typeof(User.UserTypes), split[2]);

                mscntr.Send(this, "performLogin", currentUser);
            }
            else
                mscntr.Send(this, "logOut", currentUser);


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

#if __ANDROID__
            MessagingCenter.Subscribe<MainActivity, string>(this, "tokenRefreshed", (sender, value) =>
            {
                if (value != null)
                {
                    Token = value;
                }
            });
#endif

            //send messages
            //InfoButtonCommand = new Command(() =>
            //{
            //    var clpman = (ClipboardManager) Forms.Context.GetSystemService(Context.ClipboardService);

            //    mscntr.Send<MainPageViewModel, string>(this, "showInfoAlert", Token);
            //});
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