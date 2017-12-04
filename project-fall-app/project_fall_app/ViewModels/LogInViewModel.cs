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
#endif
using project_fall_app.Models;
using PCLStorage;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;
using PCLStorage;
using System.IO;


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
                bool loginSuccessful = PerformLogin();
                if (loginSuccessful)
                {
                    mscntr.Send(this, "performLogin", currentUser);
                }
                else
                {
                    Page page = new Page();
                    page.DisplayAlert("Login Error", "Forkert password eller bruger navn", "okay").Start();
                }
            });
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
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        //responseObject = JsonConvert.DeserializeObject<ResponseObject>(reader.ReadToEnd());
                        currentUser = JsonConvert.DeserializeObject<Citizen>(JsonConvert.DeserializeObject<dynamic>(reader.ReadToEnd())["body"].ToString());

                        for (var i = 0; i < currentUser.devices.Count; i++)
                        {
                            currentUser.devices[i] = new Models.Device()
                            {
                                id = currentUser.devices[i].id,
                                content = JsonConvert.DeserializeObject<Content>(currentUser.devices[i].content, new JsonSerializerSettings()
                                {
                                    TypeNameHandling = TypeNameHandling.All
                                })
                            };
                        }
                    }
                }

                if (currentUser.id != -1)
                {
                    CreateUserCredentialsFile(currentUser);
                }
                else
                {
                    return false;
                }
                return true;
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

            await userFile.WriteAllTextAsync(JsonConvert.SerializeObject(user));
        }

        public async Task ReadContactList()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFile contactFile =
                await rootFolder.CreateFileAsync("contactList.txt", CreationCollisionOption.OpenIfExists);
            string filecontent = await contactFile.ReadAllTextAsync();



            currentUser = JsonConvert.DeserializeObject<User>(JsonConvert.DeserializeObject<dynamic>(filecontent)["body"].ToString());

            for (var i = 0; i < currentUser.devices.Count; i++)
            {
                currentUser.devices[i] = new Models.Device()
                {
                    id = currentUser.devices[i].id,
                    content = JsonConvert.DeserializeObject<Content>(currentUser.devices[i].content, new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.All
                    })
                };
            }

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