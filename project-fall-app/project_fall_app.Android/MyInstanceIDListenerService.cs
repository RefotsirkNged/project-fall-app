using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Iid;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace project_fall_app.Droid
{
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.gms.iid.InstanceID" })]
    class MyInstanceIDListenerService : InstanceIDListenerService
    {
        public override void OnTokenRefresh()
        {
            var intent = new Intent(this, typeof(RegistrationIntentService));
            StartService(intent);
        }
    }
}