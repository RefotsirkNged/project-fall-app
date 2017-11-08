using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Gms.Iid;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace project_fall_app.Droid
{
    [Service(Exported = false)]
    class RegistrationIntentService : IntentService
    {
        static object locker = new object();

        public RegistrationIntentService() : base("RegistrationIntentService") { }

        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                Log.Info("RegistrationIntentService", "Calling InstanceID.GetToken");
                lock (locker)
                {
                    var instanceID = InstanceID.GetInstance(this);
                    //TODO: replace with real iD
                    var token = instanceID.GetToken(
                        "505159225877", GoogleCloudMessaging.InstanceIdScope, null);

                    Log.Info("RegistrationIntentService", "GCM Registration Token: " + token);
                    SendRegistrationToAppServer(token);
                    Subscribe(token);
                }
            }
            catch (Exception e)
            {
                Log.Debug("RegistrationIntentService", "Failed to get a registration token");
                return;
            }
        }

        void SendRegistrationToAppServer(string token)
        {
            // Add custom implementation here as needed.
            //TODO: send this token to the server
        }

        void Subscribe(string token)
        {
            var pubSub = GcmPubSub.GetInstance(Application.Context);
            pubSub.Subscribe(token, "/topics/global", null);
        }
    }
}