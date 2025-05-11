using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIStarter.Interfaces
{
    internal interface ISlot
    {
        string ValueJSONPath { get; set; }
        string Value { get; set; }
    }
}
