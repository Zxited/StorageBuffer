﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageBuffer.Domain
{
    public interface IItem
    {
        int Id { get; set; }
        string Name { get; set; }
        string Data { get; }
        string Type { get; }

    }
}
