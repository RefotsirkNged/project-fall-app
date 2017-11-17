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
    [Service]
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
            sensman = (SensorManager)GetSystemService(Context.SensorService);
            sens = sensman.GetDefaultSensor(SensorType.Accelerometer);
            sensman.RegisterListener(this, sens, SensorDelay.Normal);
            System.Diagnostics.Debug.WriteLine("Service created");
        }

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
            //nothing
        }

        public void OnSensorChanged(SensorEvent e)
        {
            System.Diagnostics.Debug.WriteLine("sensor changed X: " + e.Values.First());
            //handle sensor input if we decide to implement this
        }
    }
}