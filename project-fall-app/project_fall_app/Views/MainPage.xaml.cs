using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using project_fall_app.ViewModels;
using Xamarin.Forms;

namespace project_fall_app
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

            //noooo pattern breaking pls no
            MessagingCenter.Subscribe<MainPageViewModel>(this, "showInfoAlert", (sender) =>
            {
                DisplayAlert("App information", "insert information", "OK");
            });
		}

	}
}
