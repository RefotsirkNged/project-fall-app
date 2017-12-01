using System;
using System.Collections.Generic;
using System.Text;

namespace project_fall_app.Models
{
    class Contact
   {

       public Contact(string name, string phone)
       {
           this.name = name;
           this.phone = phone;
       }

       public Contact(string info)
       {
           string[] infos = info.Split(',');
           name = infos[0];
           phone = infos[1];
       }


        private string name;
        private string phone;



        public String Name
        {
            get { return name; }
        }
        public String Phone
        {
            get { return phone; }
        }

        public String Info
        {
            get { return String.Format("{0},{1}", Name, Phone); }
        }
    }
}
