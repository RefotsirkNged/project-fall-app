using System.Windows.Input;
using Xamarin.Forms;
using XLabs.Ioc;

namespace project_fall_app.ViewModels
{
    class HelpViewModel : BaseViewModel
    {
        private IMessagingCenter mscntr;
        public HelpViewModel()
        {
            mscntr = Resolver.Resolve<IMessagingCenter>();
            CallForHelpCommand = new Command(() =>
            {
                mscntr.Send<HelpViewModel>(this, "callForHelp");
            });
        }

        public ICommand CallForHelpCommand { protected set; get; }

    }
}
