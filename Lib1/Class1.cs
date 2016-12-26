using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib1
{
    public class Class1
    {
        private string name = "Sara";
        public string Name { get { return name; } set { name = value; } }

        public string Hello(string name)
        {
            this.name = name;
            Console.WriteLine("Lib1.Class1: Hello " + name);
            return "I'm Public Supegg.";
        }

        private string hello(string name)
        {
            this.name = name;
            Console.WriteLine("Lib1.Class1: Hello " + name);
            return "I'm private Supegg.";
        }
    }
}
