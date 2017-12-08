using System;
using System.Collections.Generic;
using System.Text;

namespace project_fall_app.Models
{
    public class User
    {
        public int id;
        public string name;
        public string email;
        public List<Device> devices;
        public string city;
        public string address;
        public string postnr;
        public string number;
        public List<User> contacts;
        public string role;
        public string jsonCredentials;
        public string Token;
    }

    public class AlarmCode
    {
        public string responder;
        public string status;
        public string activatedBy;
        public string name;
        public string address;
        public string id;
        public string city;
        public string postnr;
        public string email;
        public string devices;
        public string role;
    }

    public class Device
    {
        public string id;
        public string devicetype;
        public string content;
        public string number;
    }

    public class Content
    {
        public string number;
    }
}
