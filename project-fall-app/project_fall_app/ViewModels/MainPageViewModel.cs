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
#if __ANDROID__
using Android.Support.V7.App;
#endif
using Newtonsoft.Json;
using project_fall_app.Tools;

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
        private static bool firstTime;
        private string data;

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
#if __ANDROID__
            InfoButtonCommand = new Command(() =>
            {

                new AlertDialog.Builder(Xamarin.Forms.Forms.Context)
                    .SetTitle("Log ud")
                    .SetMessage("vil du logge ud?")
                    .SetCancelable(true)
                    .SetPositiveButton("Log ud", (e, sender) =>
                    { mscntr.Send<MainPageViewModel>(this, "logOut");})
                    .SetNegativeButton("Cancel", (sender, e) => { })
                    .Create()
                    .Show();
            });
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
                    string filecontent = await userFile.ReadAllTextAsync();
                    string[] split = filecontent.Split('\n');

                    currentUser = JsonConvert.DeserializeObject<User>(split[0]);
                    currentUser.jsonCredentials = split[0];
                    currentUser.currentDevice = split[1];

                    currentUser.setNumber();

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
            string url;
            if (currentUser.role == "citizen")
                url = "https://prbw36cvje.execute-api.us-east-1.amazonaws.com/dev/citizen/" + currentUser.id +
                             "/device";
            else
                url = "https://prbw36cvje.execute-api.us-east-1.amazonaws.com/dev/contact/" + currentUser.id +
                    "/device";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.Method = "DELETE";
            request.Headers.Add("token", currentUser.token);
            request.Headers.Add("device", currentUser.currentDevice);

            using (var response = request.GetResponse())
            {
                switch (((HttpWebResponse)response).StatusCode)
                {
                    case HttpStatusCode.OK:
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                         
                        }
                        break;
                    default:
                        CrossPlatFormMethod.WriteTextToScreen("Fejl i log ud");
                        return;
                }
            }

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
            request.Headers.Add("token",currentUser.token);

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
            MessagingCenter.Subscribe<MainPageViewModel>(this, "logOut", (sender) =>
            {

                DeleteUserCredentials();
                shiftLogIn();
            });

            MessagingCenter.Subscribe<Droid.MainActivity, string>(this, "tokenRefreshed", (sender, value) =>
            {
                if (value != null)
                {
                    try
                    {
                        string url = "";
                        if (currentUser.role == "citizen")
                            url = "https://prbw36cvje.execute-api.us-east-1.amazonaws.com/dev/citizen/" + currentUser.id +
                                  "/device";
                        else if (currentUser.role == "contact")
                            url = "https://prbw36cvje.execute-api.us-east-1.amazonaws.com/dev/contact/" + currentUser.id +
                                  "/device";
                        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                        request.Method = "PUT";

                        request.Headers.Add("token", currentUser.token);
                        CurrentDevice currentDevice =
                            JsonConvert.DeserializeObject<CurrentDevice>(currentUser.currentDevice);
                        currentDevice.token = value;
                        string json = JsonConvert.SerializeObject(currentDevice).ToString();
                        request.Headers.Add("device", "{'token': '"+currentDevice.token+"','arn':'', 'devicetype': '"+currentDevice.devicetype+"', 'id': "+currentDevice.id+"}");

                        using (var response = request.GetResponse())
                        {
                            switch (((HttpWebResponse)response).StatusCode)
                            {
                                case HttpStatusCode.OK:
                                    using (var reader = new StreamReader(response.GetResponseStream()))
                                    {

                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    

                }
            });


            MessagingCenter.Subscribe<WaitingToHelpViewModel,string>(this, "helpRequested",
                (sender, data) =>
                {
                    this.data = data;
                    shiftMessageResponse();
                });
#endif


            MessagingCenter.Subscribe<FallResponseViewModel>(this, "callForHelpConfirmed", (sender) =>
            {
                SendAlarm(currentUser);
                shiftHelp();
            });

            MessagingCenter.Subscribe<FallResponseViewModel>(this, "callForHelpAborted", (sender) =>
            {
                shiftHelp();
            });

            MessagingCenter.Subscribe<MessageResponseViewModel>(this, "canHelp", (sender) =>
            {
                string url = "https://prbw36cvje.execute-api.us-east-1.amazonaws.com/dev/citizen/" + currentUser.id +
                             "/alarm";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "PUT";
                request.Headers.Add("alarm", data.Replace("\"responder\": null", "\"responder\": " + currentUser.jsonCredentials ).Replace("\"", "'"));
                request.Headers.Add("token", currentUser.token);

                using (var response = request.GetResponse())
                {
                    switch (((HttpWebResponse)response).StatusCode)
                    {
                        case HttpStatusCode.OK:
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                string json = reader.ReadToEnd();
                            }
                            break;

                        default:
                            CrossPlatFormMethod.WriteTextToScreen("Kunne ikke forbinde til serveren.");
                            break;
                    }
                }
#if __ANDROID__
                var dataDyn = JsonConvert.DeserializeObject<dynamic>(data);
                
                new Android.Support.V7.App.AlertDialog.Builder(Xamarin.Forms.Forms.Context)
                    .SetTitle("Infomation Om ")
                    .SetMessage("Navn: " + dataDyn.activatedby.name + "\nAddresse: " + dataDyn.activatedby.address)
                    .SetCancelable(true)
                    .Create()
                    .Show();
#endif
                shiftWaitingToHelp();
            });

            MessagingCenter.Subscribe<MessageResponseViewModel>(this, "cannotHelp", (sender) =>
            {
                string url = "https://prbw36cvje.execute-api.us-east-1.amazonaws.com/dev/citizen/" + currentUser.id +
                             "/alarm";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "PUT";
                request.Headers.Add("alarm", data.Replace("\"","'"));
                request.Headers.Add("token", currentUser.token);

                using (var response = request.GetResponse())
                {
                    switch (((HttpWebResponse)response).StatusCode)
                    {
                        case HttpStatusCode.OK:
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                string json = reader.ReadToEnd();
                            }
                            break;

                        default:
                            CrossPlatFormMethod.WriteTextToScreen("Kunne ikke forbinde til serveren.");
                            break;
                    }
                }
                shiftWaitingToHelp();
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
            var dataDyn = JsonConvert.DeserializeObject<dynamic>(data);
            Title = "Kan du hjælpe " + dataDyn.activatedby.name + "?";
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
