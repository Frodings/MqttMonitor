﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttMonitor.Core
{
    public interface ILogger
    {
        void Log(string message);
    }
}
