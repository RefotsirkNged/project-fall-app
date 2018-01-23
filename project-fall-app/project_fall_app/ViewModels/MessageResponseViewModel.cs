using System.Windows.Input;
using Xamarin.Forms;
using XLabs.Ioc;

namespace project_fall_app.ViewModels
{
    class MessageResponseViewModel : BaseViewModel, Models.IWriteToScreen
    {
#if __ANDROID__
        private Droid.MyCountDownTimer liveCounter;
#endif

        public MessageResponseViewModel()
        {
            mscntr = Resolver.Resolve<IMessagingCenter>();

            CanHelpCommand = new Command(() =>
            {
#if __ANDROID__
                liveCounter.Cancel();
                liveCounter.Dispose();
#endif
                mscntr.Send(this, "canHelp");
            });
            CannotHelpCommand = new Command(() =>
            {
#if __ANDROID__
                liveCounter.Cancel();
                liveCounter.Dispose();
#endif
                mscntr.Send(this, "cannotHelp");
            });
#if __ANDROID__
            liveCounter = new Droid.MyCountDownTimer(1 * 60, this);
            liveCounter.Start();
#endif

        }

        private IMessagingCenter mscntr;

        public ICommand CanHelpCommand { protected set; get; }
        public ICommand CannotHelpCommand { protected set; get; }

        private string countDown = "1.00";

        public string CountDown
        {
            get { return countDown; }
            set { SetProperty(ref countDown, value); }
        }

        public void WriteToScreenElement(string text)
        {
            CountDown = text;
        }

        public void FinishCountDown()
        {
#if __ANDROID__
            liveCounter.Cancel();
            liveCounter.Dispose();
#endif
            mscntr.Send(this, "cannotHelp");
        }
    }
}
