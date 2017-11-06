using System;

using Android.App;
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

		}
	}
}

