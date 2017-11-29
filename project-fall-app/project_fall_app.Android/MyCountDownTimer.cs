using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.OS;
using Android.Runtime;
using project_fall_app.Models;

namespace project_fall_app.Droid
{
    class MyCountDownTimer : CountDownTimer
    {
        private IWriteToScreen writeToScreen;

        public MyCountDownTimer(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public MyCountDownTimer(long millisInFuture, long countDownInterval) : base(millisInFuture, countDownInterval)
        {
        }

        public MyCountDownTimer(long countDownTime, IWriteToScreen writeToScreen) : base(countDownTime, 100)
        {
            this.writeToScreen = writeToScreen;

        }

        public override void OnFinish()
        {
            writeToScreen.FinishCountDown();
        }

        public override void OnTick(long millisUntilFinished)
        {
            writeToScreen.WriteToScreenElement((millisUntilFinished/1000/60).ToString());
        }
    }
}