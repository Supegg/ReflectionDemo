using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib2
{
    /// <summary>
    /// 通过继承共同接口，调用类可方便的动态创建，转换为接口调用
    /// </summary>
    public class Class2:InterfaceLib.IHello
    {
        public string Name { get; set; }
        public void Hello(string name)
        {
            Name = name;
            Console.WriteLine("Lib2.Class2: Hello " + name);
        }

        private void hello(string name)
        {
            Name = name;
            Console.WriteLine("Lib2.Class2: Hello " + name);
        }
    }
}
