using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Globalization;
using System.Threading;
using System.Reflection;

namespace DynamicDemoApp
{
    class Program
    {
        public void ResolveDynamic(dynamic obj)
        {
            Console.WriteLine(obj.Name);
            obj.Name = "Abhijit";

            Console.WriteLine(obj.Name);
        }
        public void ResolveStatic(object obj)
        {
            PropertyInfo pinfo = obj.GetType().GetProperty("Name");

            //To Get from a property Name
            object value = pinfo.GetValue(obj, null);
            string content = value as string;

            // To Set to Property Name
            pinfo.SetValue(obj, "Abhijit", null);

            //Invoke a method 
            MethodInfo minfo = obj.GetType().GetMethod("GreetMe");
            string retMessage = minfo.Invoke(obj, null) as string;

            Console.WriteLine(retMessage);
        }

        public void DisplayExpandoObject(dynamic expobj)
        {
            Console.WriteLine(expobj.Name);
            Console.WriteLine(expobj.Age);
            Console.WriteLine(expobj.Weight);
            expobj.FetchData();
        }
        static void Main(string[] args)
        {
            Program p = new Program();

            Person obj = new Person();
            obj.Name = "Abhishek";

            p.ResolveStatic(obj);

            dynamic d = new ExpandoObject();
            d.Name = "Abhishek Sur";
            d.Age = 26;
            d.Weight = 62.5d;
            d.GreetMe = new Action(delegate
            {
                   Console.WriteLine("Hello {0}", d.Name);
            });

            p.ResolveDynamic(d);

            d.FetchData = new Action(() => { Console.WriteLine("From FetchData = {0}", d.Age); });

            dynamic expandoObject = new CustomExpando();
            expandoObject.Name = "Akash";
            expandoObject.CallMe = new Func<string, string>(delegate(string name)
            {
                expandoObject.Name = name;
                return expandoObject.Name;
            });

            Console.WriteLine(expandoObject.Name);
            Console.WriteLine(expandoObject.CallMe("Hello"));

            expandoObject.Dictionary.Remove("Name");

            p.DisplayExpandoObject(d);
            Console.Read();
        }
    }
}
