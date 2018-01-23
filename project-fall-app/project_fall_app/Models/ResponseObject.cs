using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace project_fall_app.Models
{
    public class User
    {
        public int id;
        public string name;
        public string email;
        public List<Device> devices;
        public string currentDevice;
        public string city;
        public string address;
        public string postnr;
        public List<User> contacts;
        public string role;
        public string jsonCredentials;
        public string token;

        public void setNumber()
        {
            if (role == "citizen")
            {
                foreach (var contact in contacts)
                {
                    foreach (var device in contact.devices)
                    {
                        if (device.devicetype == "phonecalldevice" && device.content != null)
                        {
                            Models.Device Ndevice =
                                JsonConvert.DeserializeObject<Models.Device>(device.content);
                            device.phone_number = Ndevice.phone_number;
                        }
                    }
                }
            }
        }
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
        public string phone_number;
    }

    public class Content
    {
        public string phone_number;
    }

    public class CurrentDevice
    {
        public string id;
        public string devicetype;
        public string token;
        public string arn;
    }
}
