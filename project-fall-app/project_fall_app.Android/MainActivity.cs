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
            Resolver.SetResolver(container.GetResolver());
            LoadApplication (new project_fall_app.App ());
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);


		    if (IsPlayServicesAvailable())
		    {
		        var intent = new Intent(this, typeof(RegistrationIntentService));
		        StartService(intent);
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

