using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
#if __ANDROID__
    using Org.Json;
using Android.Widget;
#endif
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
                        Page page = new Page();
                        page.DisplayAlert("Login Error", "Forkert password eller brugernavn", "Ok").Start();
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
                                //responseObject = JsonConvert.DeserializeObject<ResponseObject>(reader.ReadToEnd());
                                string json = reader.ReadToEnd();
                                json = JsonConvert.DeserializeObject<dynamic>(json)["body"].ToString();
                                currentUser = JsonConvert.DeserializeObject<User>(json);
                                currentUser.jsonCredentials = json;

                                if (currentUser.contacts == null && currentUser.id != -1)
                                {
                                    CreateUserCredentialsFile(currentUser);
                                    return true;
                                }

                                foreach (var contact in currentUser.contacts)
                                {
                                    foreach (var device in contact.devices)
                                    {
                                        if (device.devicetype == "smartphone")
                                        {
                                            Models.Device Ndevice = JsonConvert.DeserializeObject<Models.Device>(device.content);
                                            device.number = Ndevice.number;
                                        }
                                    }
                                }
                            }
                            if (currentUser.id != -1)
                            {
                                CreateUserCredentialsFile(currentUser);
                                return true;
                            }
                            break;

                        default:
                            CrossPlatFormMethod.WriteTextToScreen("Kunne ikke forbinde til serveren.");
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
            //return true;
        }


        private async Task CreateUserCredentialsFile(User user)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFile userFile = await rootFolder.CreateFileAsync("userCredentials.txt", CreationCollisionOption.ReplaceExisting);
            
            await userFile.WriteAllTextAsync(currentUser.jsonCredentials);
        }

        public async Task ReadContactList()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFile contactFile =
                await rootFolder.CreateFileAsync("userCredentials.txt", CreationCollisionOption.OpenIfExists);
            string filecontent = await contactFile.ReadAllTextAsync();
            currentUser = JsonConvert.DeserializeObject<User>(filecontent);
            currentUser.jsonCredentials = filecontent;



            //TODO do something with the list
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