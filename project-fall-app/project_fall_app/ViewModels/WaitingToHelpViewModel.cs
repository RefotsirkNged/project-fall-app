using System.Collections.Generic;
using System.Threading.Tasks;
using Java.Lang;
using Newtonsoft.Json;
using project_fall_app.Models;
using PCLStorage;
using Plugin.FirebasePushNotification;
using Xamarin.Forms;
using XLabs.Ioc;

namespace project_fall_app.ViewModels
{
    class WaitingToHelpViewModel : BaseViewModel
    {
        private List<string> citizenList;
        private IMessagingCenter mscntr;
        private static bool firstTime;

        public WaitingToHelpViewModel()
        {
            mscntr = Resolver.Resolve<IMessagingCenter>();
            if (!firstTime)
                mscntr.Send<WaitingToHelpViewModel>(this, "initFirebase");
            firstTime = true;
            VerifyAlarmFileExistence();

        }

        private async Task VerifyAlarmFileExistence()
        {
            try
            {
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                ExistenceCheckResult fileExist = await rootFolder.CheckExistsAsync("alarm.txt");


                if (fileExist == ExistenceCheckResult.FileExists)
                {
                    IFile userFile =
                        await rootFolder.CreateFileAsync("alarm.txt", CreationCollisionOption.OpenIfExists);
                    string fileContent = await userFile.ReadAllTextAsync();
                    await userFile.DeleteAsync();
                    mscntr.Send(this, "helpRequested", fileContent);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<string> CitizenList
        {
            get { return citizenList; }
            set { SetProperty(ref citizenList, value); }
        }
    }
}