using System;
using System.Collections.Generic;
using System.Text;

namespace project_fall_app.Models
{
    //class ResponseObject
    //{
    //    public string statusCode;
    //    public Body body;
    //    public class Body
    //    {
    //        public string id;
    //        public string email;
    //        public string name;
    //        public string role;
    //        //public List<String> contacts;
    //    }
    //}
    public class User
    {
        public int id;
        public string name;
        public string email;
        public List<ParsableDevice> devices;
    }

    public class Citizen : User
    {
        public string city;
        public string address;
        public string postnr;
        public List<Contact> contacts;
    }

    public class Contact : User
    {
        public string phone;
    }

    public class ParsableDevice
    {
        public string id;
        public string content;
    }

    public class Device : ParsableDevice
    {
        public string id;
        public Content content;
    }

    public class Content
    {
        public string devicetype;
        public string messagetype;

        public string userid;
        public string token;
        public string arn;
        public string phone;
    }
}
