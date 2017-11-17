using System;
using System.Collections.Generic;
using System.Text;

namespace project_fall_app.Models
{
    class ResponseObject
    {
        public string statusCode;
        public Body body;
        public class Body
        {
            public string id;
            public string username;
            public string email;
            public string name;
            public string role;
        }
    }
}
