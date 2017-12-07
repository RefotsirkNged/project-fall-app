using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
#if __ANDROID__ 
    using Android.Content;
    using project_fall_app.Droid;
    using Android.App;
    using Android.Widget;
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
        //store here, we can always access this if we need the user after the initial login has been done
        private User currentUser = new User();

        private IDevice device;
        private IMessagingCenter mscntr;

        #endregion

        public MainPageViewModel()
        {
            token = string.Empty;
            device = Resolver.Resolve<IDevice>();
            mscntr = Resolver.Resolve<IMessagingCenter>();

            InitMessages();
            VerifyUserCredentialFileExistence();

            Title = "Falddetektions-app";
            TopBarHeight = 50;
            TopBarLabelWidth = (int) (device.Display.Width * 0.9f);
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

                    currentUser = JsonConvert.DeserializeObject<User>(fileContent);
                    currentUser.jsonCredentials = fileContent;

                    if (currentUser.role == "citizen")
                    {
                        foreach (var contact in currentUser.contacts)
                        {
                            foreach (var device in contact.devices)
                            {
                                if (device.devicetype == "smartphone")
                                {
                                    Models.Device Ndevice =
                                        JsonConvert.DeserializeObject<Models.Device>(device.content);
                                    device.number = Ndevice.number;
                                }
                            }
                        }
                    }

                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        if (currentUser.role == "citizen")
                        {
                            shiftHelp();
                        }
                        else if (currentUser.role == "contact")
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

        private void SendAlarm(User user)
        {
            if (!LogInViewModel.CheckForInternetConnection())
            {
                if (currentUser.contacts.Count >= 1)
                {
#if __ANDROID__
                    PhoneCallDroid call = new PhoneCallDroid();
                    call.MakeQuickCall(user.contacts[0].devices[0].number);
#endif
                }
                return;
            }

            string url = "https://prbw36cvje.execute-api.us-east-1.amazonaws.com/dev/alarm";
            HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(url);
            
            request.Method = "POST";
            request.Headers.Add("citizen", user.jsonCredentials);
            //request.Headers.Add("location", location);

            using (var response = request.GetResponse())
            {
                switch (((HttpWebResponse)response).StatusCode)
                {
                    case HttpStatusCode.OK:
#if __ANDROID__
                        Toast.MakeText(Xamarin.Forms.Forms.Context, "Alarm blev successfuldt sendt.", ToastLength.Long);
#endif
                        break;
                    default:
#if __ANDROID__
                        if (currentUser.contacts.Count >= 1)
                        {
                            PhoneCallDroid call = new PhoneCallDroid();
                            call.MakeQuickCall(currentUser.contacts[0].number);
                            
                        }
#endif
                        break;
                }
            }
        }

        #region MessageCenter

        private void InitMessages()
        {
            //Subscriptions
            MessagingCenter.Subscribe<LogInViewModel, User>(this, "performLogin", (sender, userobj) =>
            {
                currentUser = userobj;
                if (currentUser.role == "citizen")
                {
                    shiftHelp(); 
                }
                else if (currentUser.role == "contact")
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
                SendAlarm(currentUser);
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
