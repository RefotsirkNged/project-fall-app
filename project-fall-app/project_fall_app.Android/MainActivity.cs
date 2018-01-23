using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Views;
using Android.Widget;
using Android.OS;
using ButtonCircle.FormsPlugin.Droid;
using Xamarin.Forms;
using XLabs.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;
using Android.Gms.Common;
using project_fall_app.Models;
using project_fall_app.ViewModels;
using PCLStorage;
using Plugin.FirebasePushNotification;
using Plugin.FirebasePushNotification.Abstractions;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace project_fall_app.Droid
{
	[Activity (Label = "project_fall_app", Icon = "@drawable/icon", Theme="@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : XFormsApplicationDroid //, global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
	    private static bool firstTime;
	    private static bool firstTimeNotifi;

        private IMessagingCenter mscntr;


        protected override void OnCreate (Bundle bundle)
		{


			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);
            ButtonCircleRenderer.Init();
		    var container = new SimpleContainer();
		    container.Register<IDevice>(t => AndroidDevice.CurrentDevice);
		    container.Register<IDisplay>(t => t.Resolve<IDevice>().Display);
		    container.Register<INetwork>(t => t.Resolve<IDevice>().Network);
		    container.Register<IMessagingCenter>(t => MessagingCenter.Instance);
		    if (!Resolver.IsSet)
		    {
		        Resolver.SetResolver(container.GetResolver());
            }
            LoadApplication (new project_fall_app.App ());
		    mscntr = Resolver.Resolve<IMessagingCenter>();
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);

		    Intent intent = new Intent(this, typeof(FallService));
		    //StartService(intent);

            //Notification initialization and debugging stuff

            

		    MessagingCenter.Subscribe<WaitingToHelpViewModel>(this, "initFirebase", (sender) =>
		    {
                InitFirebase();
            });

            MessagingCenter.Subscribe<HelpViewModel>(this, "initFirebase", (sender) =>
            {
                InitFirebase();
            });


        }

	    private async Task CreateAlarmFile(string data)
	    {
	        IFolder rootFolder = FileSystem.Current.LocalStorage;
	        IFile userFile = await rootFolder.CreateFileAsync("alarm.txt", CreationCollisionOption.ReplaceExisting);

	        await userFile.WriteAllTextAsync(data);
	    }

        public bool IsPlayServicesAvailable()
	    {
	        int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
	        if (resultCode != ConnectionResult.Success)
	        {
	            if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
	                Toast.MakeText(ApplicationContext, GoogleApiAvailability.Instance.GetErrorString(resultCode), ToastLength.Long).Show();
	            else
	            {
	                Toast.MakeText(ApplicationContext, "Sorry, this device is not supported", ToastLength.Long).Show();
	                Finish();
	            }
	            return false;
	        }
	        else
	        {
	            Toast.MakeText(ApplicationContext, "Device connection OK", ToastLength.Long).Show();
	            return true;
	        }
	    }

	    private void InitFirebase()
	    {
	        FirebasePushNotificationManager.Initialize(this, true);

	        //FirebasePushNotificationManager.Initialize(this, false);
	        FirebasePushNotificationManager.ProcessIntent(Intent);

	        if (IsPlayServicesAvailable())
	        {
	            FirebasePushNotificationManager.IconResource = Resources.GetIdentifier("testimg", "drawable", PackageName);
	            FirebasePushNotificationManager.NotificationContentTextKey = "Title";

	            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
	            {
	                mscntr.Send(this, "tokenRefreshed", p.Token);
	            };

	            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
	            {

	                string outPut;
	                p.Data.TryGetValue("default", out outPut);

	                CreateAlarmFile(outPut);
	            };

	            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
	            {

	            };
	        }
        }
    }

}

