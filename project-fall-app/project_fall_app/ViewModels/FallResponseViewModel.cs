using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XLabs.Ioc;

namespace project_fall_app.ViewModels
{
    class FallResponseViewModel : BaseViewModel
    {
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
        }

        public ICommand ConfirmCallForHelpCommand { protected set; get; }
        public ICommand AbortCallForHelpCommand { protected set; get; }
    }
}
