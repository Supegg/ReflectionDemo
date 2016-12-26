using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfaceLib
{
    public interface IHello
    {
        string Name{get;set;}
        void Hello(string name);
    }
}
