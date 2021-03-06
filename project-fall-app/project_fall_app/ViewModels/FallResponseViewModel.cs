﻿using System.Windows.Input;
using Xamarin.Forms;
using XLabs.Ioc;

namespace project_fall_app.ViewModels
{
    class FallResponseViewModel : BaseViewModel , Models.IWriteToScreen
    {
        string countDown = "1.00";
#if __ANDROID__
        private Droid.MyCountDownTimer liveCounter;
#endif

        public string CountDown
        {
            get { return countDown; }
            set { SetProperty(ref countDown, value); }
        }

        private IMessagingCenter mscntr;

        public FallResponseViewModel()
        {
            mscntr = Resolver.Resolve<IMessagingCenter>();
            ConfirmCallForHelpCommand = new Command(() =>
            {
#if __ANDROID__
                liveCounter.Cancel();
                liveCounter.Dispose();
#endif
                mscntr.Send<FallResponseViewModel>(this, "callForHelpConfirmed");
            });
            AbortCallForHelpCommand = new Command(() =>
            {
#if __ANDROID__
                liveCounter.Cancel();
                liveCounter.Dispose();
#endif
                mscntr.Send<FallResponseViewModel>(this, "callForHelpAborted");
            });
#if __ANDROID__
            liveCounter = new Droid.MyCountDownTimer(1 * 60, this);
            liveCounter.Start();
#endif

        }

        public ICommand ConfirmCallForHelpCommand { protected set; get; }
        public ICommand AbortCallForHelpCommand { protected set; get; }

        public void WriteToScreenElement(string text)
        {
            CountDown = text;
        }

        public void FinishCountDown()
        {
#if __ANDROID__
            liveCounter.Dispose();
#endif
            mscntr.Send<FallResponseViewModel>(this, "callForHelpConfirmed");
        }
    }
}
