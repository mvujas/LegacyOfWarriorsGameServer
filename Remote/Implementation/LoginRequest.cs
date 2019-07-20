﻿using Remote.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote.Implementation
{
    [Serializable]
    public class LoginRequest : IRemoteObject
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
