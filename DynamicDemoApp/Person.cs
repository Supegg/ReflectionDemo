using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDemoApp
{
    internal class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }

        public string GreetMe()
        {
            return string.Format("Hello {0}", this.Name);
        }
    }
}
