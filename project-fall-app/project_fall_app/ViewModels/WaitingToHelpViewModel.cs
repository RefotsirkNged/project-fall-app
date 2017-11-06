using System;
using System.Collections.Generic;
using System.Text;

namespace project_fall_app.ViewModels
{
    class WaitingToHelpViewModel : BaseViewModel
    {
        private List<string> citizenList;

        public WaitingToHelpViewModel()
        {
            citizenList = new List<string>()
            {
                "Bent Bentsen",
                "Carsten Ibsen",
                "August Kørvell",
                "Frederik Voldby",
                "Kristoffer Degn"
            };
        }

        public List<string> CitizenList
        {
            get { return citizenList; }
            set { SetProperty(ref citizenList, value); }
        }
    }
}