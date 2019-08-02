﻿using Remote.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote.Implementation
{
    [Serializable]
    public class LoginResponse : AbstractResponse
    {
        public UserInfo UserInfo { get; set; } = null;
    }
}
