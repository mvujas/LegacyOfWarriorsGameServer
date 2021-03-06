﻿using Remote.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote.Implementation
{
    [Serializable]
    public abstract class AbstractResponse : IRemoteObject
    {
        public bool Successfulness { get; set; }
        public string Message { get; set; }
    }
}
