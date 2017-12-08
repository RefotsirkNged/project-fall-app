using System;
using System.Threading.Tasks;
using System.Windows.Input;
using project_fall_app.Models;
using project_fall_app.Views;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using View = Xamarin.Forms.View;
using PCLStorage;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using project_fall_app.Droid;
using project_fall_app.Tools;
using Plugin.FirebasePushNotification.Abstractions;

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
                CrossPlatFormMethod.MakeCall(user);
                return;
            }

            string url = "https://prbw36cvje.execute-api.us-east-1.amazonaws.com/dev/citizen/" + user.id + "/alarm";
            HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(url);
            
            request.Method = "POST";
            request.Headers.Add("Token",currentUser.Token);

            using (var response = request.GetResponse())
            {
                switch (((HttpWebResponse)response).StatusCode)
                {
                    case HttpStatusCode.OK:
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            string json = reader.ReadToEnd();

                            json = JsonConvert.DeserializeObject<dynamic>(json)["body"].ToString();
                            dynamic code = JsonConvert.DeserializeObject<dynamic>(json);

                            if (code["status"] == -1)
                            {
                                CrossPlatFormMethod.MakeCall(user);
                            }
                            else
                                CrossPlatFormMethod.WriteTextToScreen("Alarm blev successfuldt sendt.");
                        }
                        break;
                    default:
                        CrossPlatFormMethod.MakeCall(user);
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
            MessagingCenter.Subscribe<Droid.MainActivity>(this, "logOut", (sender) =>
            {
                DeleteUserCredentials();
                shiftLogIn();
            });

            MessagingCenter.Subscribe<Droid.MainActivity, string>(this, "tokenRefreshed", (sender, value) =>
            {
                if (value != null)
                {
                    Token = value;
                }
            });


            MessagingCenter.Subscribe<Droid.MainActivity, FirebasePushNotificationResponseEventArgs>(this, "helpRequested",
                (sender, data) =>
                {
                    //TODO do something with the data infomation
                    shiftMessageResponse();
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
