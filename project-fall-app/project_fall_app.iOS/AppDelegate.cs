using System;
using System.Collections.Generic;
using System.Linq;
using ButtonCircle.FormsPlugin.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using XLabs.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;

namespace project_fall_app.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : XFormsApplicationDelegate //global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init ();
            ButtonCircleRenderer.Init();
		    var container = new SimpleContainer();
		    container.Register<IDevice>(t => AppleDevice.CurrentDevice);
		    container.Register<IDisplay>(t => t.Resolve<IDevice>().Display);
		    container.Register<INetwork>(t => t.Resolve<IDevice>().Network);
		    container.Register<IMessagingCenter>(t => MessagingCenter.Instance);
            Resolver.SetResolver(container.GetResolver());
            LoadApplication (new project_fall_app.App ());

            return base.FinishedLaunching (app, options);
		}
	}
}
