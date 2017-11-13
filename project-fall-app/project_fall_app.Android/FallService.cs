using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace project_fall_app.Droid
{
    class FallService : Service, ISensorEventListener
    {
        private SensorManager sensman;
        private Sensor sens;
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            System.Diagnostics.Debug.WriteLine("Service created");
        }

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
            //nothing
        }

        public void OnSensorChanged(SensorEvent e)
        {
            //handle sensor input if we decide to implement this
        }
    }
}