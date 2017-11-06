using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using project_fall_app.Views;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace project_fall_app.ViewModels
{
    class MainPageViewModel : BaseViewModel
    {
        private IDevice device;
        private IMessagingCenter mscntr;

        public MainPageViewModel()
        {
            PageContent = new LogInView();
            Title = "Skal du bruge hjælp?";
            TopBarHeight = 50;
            device = Resolver.Resolve<IDevice>();
            mscntr = Resolver.Resolve<IMessagingCenter>();
            TopBarLabelWidth = (int)(device.Display.Width * 0.9f);
            InitMessages();

        }

        #region MessageCenter

        private void InitMessages()
        {

            //Subscriptions
            MessagingCenter.Subscribe<LogInViewModel>(this, "performLogin", (sender)=>
            {
                //TODO implement proper login
                shiftHelp();
            });

            MessagingCenter.Subscribe<HelpViewModel>(this, "callForHelp", (sender) =>
            {
                shiftFallResponse();
            });


            MessagingCenter.Subscribe<FallResponseViewModel>(this, "callForHelpConfirmed", (sender) =>
            {
                //TODO implement server call
                shiftHelp();
            });

            MessagingCenter.Subscribe<FallResponseViewModel>(this, "callForHelpAborted", (sender)=>
            {
               
                shiftHelp();
            });

            MessagingCenter.Subscribe<MessageResponseViewModel>(this, "canHelp", (sender) =>
            {
                //tell the server that device id XXXXXXXXXX can help

            });

            MessagingCenter.Subscribe<MessageResponseViewModel>(this, "helpRequested", (sender) =>
            {
                shiftMessageResponse();
            });


            //send messages
            InfoButtonCommand = new Command(() =>
            {
                mscntr.Send<MainPageViewModel>(this, "showInfoAlert");
            });
        }

        #endregion


        #region page-shift

        private void shiftFallResponse()
        {
            Title = "Skal du bruge hjælp?";
            PageContent = new FallResponseView();
        }
        private void shiftLogIn()
        {
            Title = "Log på";
            PageContent = new LogInView();
        }
        private void shiftMessageResponse()
        {
            Title = "Kan du hjælpe?";
            PageContent = new MessageResponseView();
        }
        private void shiftHelp()
        {
            Title = "Skal du bruge hjælp?";
            PageContent = new HelpView();
        }

        private void shiftWaitingToHelp()
        {
            Title = "Du er klar til at hjælpe";
            PageContent = new WaitingToHelpView();
        }

        #endregion

        public ICommand InfoButtonCommand { protected set; get; }


        #region Fields
        private int topBarHeight;
        private View pageContent;
        private int topBarLabelWidth;


        #endregion


        #region gettersetter
        public View PageContent
        {
            get { return pageContent; }
            set { SetProperty(ref pageContent, value); }
        }

        public int TopBarHeight
        {
            get { return topBarHeight; }
            set { SetProperty(ref topBarHeight, value); }
        }

        public int TopBarLabelWidth
        {
            get { return topBarLabelWidth; }
            set { SetProperty(ref topBarLabelWidth, value); }
        }

        #endregion




    }
}
