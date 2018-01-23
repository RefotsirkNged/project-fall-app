
using System;
using System.Runtime.CompilerServices;

#if __ANDROID__
using Android.Content;
using Android.Support.V7.App;
using Android.Widget;
using project_fall_app.Droid;
#endif
using project_fall_app.Models;
using Xamarin.Forms;
using Device = project_fall_app.Models.Device;

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
            PhoneCallDroid call = new PhoneCallDroid();
            foreach (User conta in user.contacts)
            {
                foreach (Device device in conta.devices)
                {
                    if (device.phone_number != null)
                    {
                        call.MakeQuickCall(device.phone_number);
                        return;
                    }
                }
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
#if __ANDROID__
        public static void CreateAleartDialog(string title, string message, string button, IDialogInterfaceOnClickListener command)
        {

            new AlertDialog.Builder(Xamarin.Forms.Forms.Context)
                .SetTitle(title)
                .SetMessage(message)
                .SetCancelable(true)
                .SetPositiveButton(button, command)
                .SetNegativeButton("Cancel", (sender, e) =>
                {
                    
                })
                .Create()
                .Show();

        }
#endif
    }
}
