﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core
{
    internal interface ISengleton<T>
    {
        T Instance { get; }
    }
}
