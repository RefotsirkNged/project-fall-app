using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
#if __ANDROID__ 
    using Android.Content;
    using project_fall_app.Droid;
    using Android.App;
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
using System.Net;
using Newtonsoft.Json;

namespace project_fall_app.ViewModels
{
    class MainPageViewModel : BaseViewModel
    {
        #region Fields

        private int topBarHeight;
        private View pageContent;
        private int topBarLabelWidth;
        private string token;

        private User currentUser = new User(); //store here, we can always access this if we need the user after the initial login has been done

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

#if __ANDROID__
            if (!CheckServerConnection())
            {
                new AlertDialog.Builder(Xamarin.Forms.Forms.Context).SetPositiveButton("Ok", (sender, args) => {})
                    .SetMessage("Kan ikke forbinde til serveren")
                    .SetTitle("Forbindelse fejl")
                    .Show();

            }
#endif
        }

        private async Task VerifyUserCredentialFileExistence()
        {
            try
            {
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                ExistenceCheckResult fileExist = await rootFolder.CheckExistsAsync("userCredentials.txt");

                if (fileExist == ExistenceCheckResult.FileExists)
                {
                    IFile userFile =
                        await rootFolder.CreateFileAsync("userCredentials.txt", CreationCollisionOption.OpenIfExists);
                    String fileContent = await userFile.ReadAllTextAsync();

                    currentUser = JsonConvert.DeserializeObject<User>(JsonConvert.DeserializeObject<dynamic>(fileContent)["body"].ToString());

                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        if (currentUser.GetType() == typeof(Citizen))
                        {
                            shiftHelp();
                        }
                        else if (currentUser.GetType() == typeof(Contact))
                        {
                            shiftWaitingToHelp();
                        }
                    });
                }
                else
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        shiftLogIn();
                    });
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private bool CheckServerConnection()
        {
            //TODO:perform a check of whether or not the server is available
            HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create("https://google.dk");
            request.Method = "GET";
            //request.Headers.Add("email", UsernameText);
            //request.Headers.Add("password", PasswordText);

            using (var response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    //responseObject = JsonConvert.DeserializeObject<ResponseObject>(reader.ReadToEnd());
                }
            }

            
            return true;
        }


        private async Task DeleteUserCredentials()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFile userFile = await rootFolder.CreateFileAsync("userCredentials.txt", CreationCollisionOption.OpenIfExists);
            await userFile.DeleteAsync();
        }

        #region MessageCenter

        private void InitMessages()
        {
            //Subscriptions
            MessagingCenter.Subscribe<LogInViewModel, User>(this, "performLogin", (sender, userobj) =>
            {
                currentUser = userobj;
                if (currentUser.GetType() == typeof(Citizen))
                {
                    shiftHelp(); 
                }
                else if (currentUser.GetType() == typeof(Contact))
                {
                    shiftWaitingToHelp();
                }
                
            });

            MessagingCenter.Subscribe<HelpViewModel>(this, "callForHelp", (sender) =>
            {
                shiftFallResponse(); 
            });

#if __ANDROID__
            MessagingCenter.Subscribe<MainActivity>(this, "logOut", (sender) =>
            {
                DeleteUserCredentials();
                shiftLogIn();
            });
#endif


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
