using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XLabs.Ioc;

namespace project_fall_app.ViewModels
{
    class MessageResponseViewModel : BaseViewModel
    {

        public MessageResponseViewModel()
        {
            mscntr = Resolver.Resolve<IMessagingCenter>();

            CanHelpCommand = new Command(() =>
            {
                mscntr.Send(this, "canHelp");
            });
            CannotHelpCommand = new Command(() =>
            {
                mscntr.Send(this, "cannotHelp");
            });


        }

        private IMessagingCenter mscntr;

        public ICommand CanHelpCommand { protected set; get; }
        public ICommand CannotHelpCommand { protected set; get; }
    }
}
