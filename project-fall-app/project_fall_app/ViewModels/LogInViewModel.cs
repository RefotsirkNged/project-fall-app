using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using project_fall_app.Models;
using PCLStorage;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;
using System.Net.NetworkInformation;
using project_fall_app.Tools;


namespace project_fall_app.ViewModels
{
    class LogInViewModel : BaseViewModel
    {
        private IMessagingCenter mscntr;
        private IDevice dvce;
        private INetwork ntwrk;
        private User currentUser;

        public LogInViewModel()
        {
            mscntr = Resolver.Resolve<IMessagingCenter>();
            dvce = Resolver.Resolve<IDevice>();
            ntwrk = Resolver.Resolve<INetwork>();


            LogOnCommand = new Command(() =>
            {
                bool isThereInternet = CheckForInternetConnection();
                if (isThereInternet)
                {
                    bool loginSuccessful = PerformLogin();
                    if (loginSuccessful)
                    {
                        mscntr.Send(this, "performLogin", currentUser);
                    }
                    else
                    {
                        CrossPlatFormMethod.CreateAleartDialog("Login Error", "Forkert kodeord eller email", "okay");
                    }
                }
                else
                    CrossPlatFormMethod.WriteTextToScreen("Ingen forbindelse til internettet.");
            });
        }

        public static bool CheckForInternetConnection()
        {
            Ping sender = new Ping();
            PingReply reply = sender.Send("8.8.8.8");

            if (reply.Status == IPStatus.Success)
                return true;
            else
                return false;
        }

        private bool PerformLogin()
        {
            string url = "https://prbw36cvje.execute-api.us-east-1.amazonaws.com/dev/user";

            try
            {
                HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(url);
                request.Method = "GET";
                request.Headers.Add("email", UsernameText);
                request.Headers.Add("password", PasswordText);

                using (var response = request.GetResponse())
                {
                    switch (((HttpWebResponse)response).StatusCode)
                    {
                        case HttpStatusCode.OK:
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                string json = reader.ReadToEnd();
                                json = JsonConvert.DeserializeObject<dynamic>(json)["body"].ToString();
                                currentUser = JsonConvert.DeserializeObject<User>(json);
                                currentUser.jsonCredentials = json;

                                if(currentUser.contacts != null)
                                    currentUser.setNumber();
                            }
                            break;

                        default:
                            CrossPlatFormMethod.WriteTextToScreen("Kunne ikke forbinde til serveren.");
                            return false;
                    }
                }
                if(currentUser.role == "citizen")
                    url = "https://prbw36cvje.execute-api.us-east-1.amazonaws.com/dev/citizen/" + currentUser.id +
                      "/device";
                else if(currentUser.role == "contact")
                    url = "https://prbw36cvje.execute-api.us-east-1.amazonaws.com/dev/contact/" + currentUser.id +
                          "/device";
                request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "POST";
                request.Headers.Add("token", currentUser.token);
                request.Headers.Add("device", "{'token': 'temp_token','arn':'', 'devicetype': 'appdevice', 'id': -1}");


                using (var response = request.GetResponse())
                {
                    switch (((HttpWebResponse)response).StatusCode)
                    {
                        case HttpStatusCode.OK:
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                string content = reader.ReadToEnd();
                                currentUser.currentDevice = JsonConvert.DeserializeObject<dynamic>(content)["body"].ToString() ;
                                CreateUserCredentialsFile(currentUser);
                                return true;
                            }
                            break;

                        default:
                            CrossPlatFormMethod.WriteTextToScreen("fejl ved log in");
                            return false;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }


        private async Task CreateUserCredentialsFile(User user)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFile userFile = await rootFolder.CreateFileAsync("userCredentials.txt", CreationCollisionOption.ReplaceExisting);
            
            await userFile.WriteAllTextAsync(currentUser.jsonCredentials + "\n" + currentUser.currentDevice);
        }

        public async Task ReadContactList()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFile contactFile =
                await rootFolder.CreateFileAsync("userCredentials.txt", CreationCollisionOption.OpenIfExists);

            string filecontent = await contactFile.ReadAllTextAsync();
            string[] split = filecontent.Split('\n');
            currentUser = JsonConvert.DeserializeObject<User>(filecontent);
            currentUser.jsonCredentials = filecontent;
        }

        private string usernameText;
        private string passwordText;

        public ICommand LogOnCommand { protected set; get; }

        public string UsernameText
        {
            get { return usernameText; }
            set { SetProperty(ref usernameText, value.Replace(@"\", @"\\")); }
        }

        public string PasswordText
        {
            get { return passwordText; }
            set { SetProperty(ref passwordText, value.Replace(@"\", @"\\")); }
            
        }
    }
}