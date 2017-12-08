using System;
using System.Collections.Generic;
using System.Text;
using Android.Widget;
using project_fall_app.Droid;
using project_fall_app.Models;

namespace project_fall_app.Tools
{
    class CrossPlatFormMethod
    {
        public static void WriteTextToScreen(string message)
        {
#if __ANDROID__
            Toast.MakeText(Xamarin.Forms.Forms.Context, message, ToastLength.Long).Show();
#elif IOS
#endif
        }

        public static void MakeCall(User user)
        {
#if __ANDROID__
            if (user.contacts.Count > 0)
            {
                PhoneCallDroid call = new PhoneCallDroid();
                if (user.contacts[0].devices.Count > 0)
                    call.MakeQuickCall(user.contacts[0].devices[0].number);
                else
                    CrossPlatFormMethod.WriteTextToScreen("Ingen fundne kontakt personer til at ringe til.");

            }
#endif
        }
    }
}
