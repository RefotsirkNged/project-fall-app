using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;
using XLabs.Ioc;

namespace project_fall_app.ViewModels
{
    class FallResponseViewModel : BaseViewModel , Models.IWriteToScreen
    {
        string countDown = "1.00";
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
                mscntr.Send<FallResponseViewModel>(this, "callForHelpConfirmed");
            });
            AbortCallForHelpCommand = new Command(() =>
            {
                mscntr.Send<FallResponseViewModel>(this, "callForHelpAborted");
            });
            #if __ANDROID__
                new Droid.MyCountDownTimer(1*60, this).Start();
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
            mscntr.Send<FallResponseViewModel>(this, "callForHelpConfirmed");
        }
    }
}
