using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
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
using Android.Util;
using Firebase;
using Plugin.FirebasePushNotification;

namespace project_fall_app.Droid
{
	[Activity (Label = "project_fall_app", Icon = "@drawable/icon", Theme="@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : XFormsApplicationDroid //, global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{


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
		    this.Window.AddFlags(WindowManagerFlags.Fullscreen);

		    Intent intent = new Intent(this, typeof(FallService));
		    //StartService(intent);

            //Notification initialization and debugging stuff

            FirebasePushNotificationManager.Initialize(this, true);

                //FirebasePushNotificationManager.Initialize(this, false);
            FirebasePushNotificationManager.ProcessIntent(Intent);



		    if (IsPlayServicesAvailable())
		    {
		        FirebasePushNotificationManager.IconResource = Resources.GetIdentifier("ic_info_outline_black_24dp", "drawable", PackageName);

                CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
                {
                    IMessagingCenter mscntr = Resolver.Resolve<IMessagingCenter>();
                    mscntr.Send(this, "tokenRefreshed", p.Token);
                    System.Diagnostics.Debug.WriteLine($"TOKEN: {p.Token}");
		        };

		        CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
		        {
                    System.Diagnostics.Debug.WriteLine("recieved" + p.Data);
		        };

            }
            


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
    }
}

