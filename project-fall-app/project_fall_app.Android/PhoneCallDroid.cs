using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Views;
using Android.Widget;
using project_fall_app.InterFace;

namespace project_fall_app.Droid
{
    class PhoneCallDroid : IMakeCall
    {

        public void MakeQuickCall(string phoneNumber)
        {
            try
            {

                var uri = Android.Net.Uri.Parse(string.Format("tel:{0}", phoneNumber));
                var intent = new Intent(Intent.ActionCall, uri);
                Xamarin.Forms.Forms.Context.StartActivity(intent);

                AudioManager audioManager = (AudioManager)Android.App.Application.Context.GetSystemService(Context.AudioService);
                audioManager.Mode = Mode.InCall;
                audioManager.SpeakerphoneOn = true;
            }
            catch (Exception e)
            {
                new AlertDialog.Builder(Xamarin.Forms.Forms.Context).SetPositiveButton("Ok", (sender, args) =>
                    {
                        //TODO what should happen
                    })
                    .SetMessage(e.ToString())
                    .SetTitle("Android Error")
                    .Show();
            }
        }
    }
}