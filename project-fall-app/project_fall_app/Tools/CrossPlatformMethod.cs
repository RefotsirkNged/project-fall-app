#if __ANDROID__
using Android.Support.V7.App;
using Android.Widget;
using project_fall_app.Droid;
#endif
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
                    call.MakeQuickCall(user.contacts[0].devices[0].phone_number);
                else
                    CrossPlatFormMethod.WriteTextToScreen("Ingen fundne kontakt personer til at ringe til.");

            }
#endif
        }

        public static void CreateAleartDialog(string title, string message, string button)
        {
#if __ANDROID__
            new AlertDialog.Builder(Xamarin.Forms.Forms.Context)
                .SetTitle(title)
                .SetMessage(message)
                .SetCancelable(true)
                .SetNegativeButton(button, (sender, e) =>{})
                .Create()
                .Show();
#endif
        }
    }
}
