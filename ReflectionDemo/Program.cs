using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace ReflectionDemo
{
    public class Program
    {
        /// <summary>
        /// 动态加载创建实例
        /// Reference http://www.cnblogs.com/zuozuo/archive/2011/09/29/2195309.html
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Assembly[] asm = AppDomain.CurrentDomain.GetAssemblies();
            try
            {
                test();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.Read();
        }

        public static void test()
        {
            Stopwatch watch = new Stopwatch();
            object rtn = null;
            string path = AppDomain.CurrentDomain.BaseDirectory;
            //load current assembly
            Assembly assemblyCurrent = Assembly.GetExecutingAssembly();
            //load remote code, awesome
            //load local dll
            //Assembly assemblyLib1 = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + @"Lib\Lib1.dll");
            //Assembly assemblyLib1 = Assembly.Load(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + @"Lib\Lib1.dll"));
            //Assembly assemblyLib2 = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + @"Lib\Lib2.dll");

            Console.WriteLine(" Reflect Public&private method, Public&private property");
            watch.Restart();
            Assembly assemblyLib1 = Assembly.Load(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + @"\Lib\Lib1.dll"));
            //invoke method & property
            Type type = assemblyLib1.GetType("Lib1.Class1");
            PropertyInfo property = type.GetProperty("Name");
            MethodInfo publicMethod = type.GetMethod("Hello");
            //Activator create an instansce with a matched constructor
            //object instansce = Activator.CreateInstance(type, null);
            //1.get Constructor, 2.invoke an instansce
            ConstructorInfo c = type.GetConstructor(new Type[] {});
            object instansce = c.Invoke(null);
            //invoke public method
            Console.WriteLine(publicMethod.Invoke(instansce, new object[] { "Contana" }));
            Console.WriteLine("Name Property: {0}", property.GetValue(instansce, null));
            //invoke private method
            rtn = type.InvokeMember("hello", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic, null, instansce, new object[] { "Susan" }, null, null, new string[] { "name" });
            Console.WriteLine("Name Property: {0}", property.GetValue(instansce, null));
            //invoke private property
            Console.WriteLine("private field name: {0}", type.InvokeMember("name", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic, null, instansce, null, null, null, null));
            Console.WriteLine("Used Times: {0}", watch.ElapsedMilliseconds);

            Console.WriteLine("\r\n Excute by Interface");
            Console.WriteLine(" Implement plug-in mode");
            watch.Restart();
            Assembly assemblyLib2 = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + @"\Lib\Lib2.dll");
            //Inherit same Interface 
            type = assemblyLib2.GetType("Lib2.Class2");
            object ifLib = Activator.CreateInstance(type, null);
            InterfaceLib.IHello ihello = (InterfaceLib.IHello)ifLib;//Converts to Interface
            ihello.Hello("Julia");
            Console.WriteLine("Name Property: {0}", ihello.Name);
            Console.WriteLine("Used Times: {0}", watch.ElapsedMilliseconds);

            Console.WriteLine("\r\n Excute in a new Domain");
            watch.Restart();
            //Load&Unload Domain
            AppDomain domain = AppDomain.CreateDomain("DLL Load&Unload test");
            //ProxyObject proxy = (ProxyObject)liveApp.CreateInstanceFromAndUnwrap("ReflectionDemo.exe", "ReflectionDemo.ProxyObject");
            ProxyObject proxy = (ProxyObject)domain.CreateInstanceFromAndUnwrap("ReflectionDemo.exe", "ReflectionDemo.ProxyObject", false, BindingFlags.Default, null, new object[] { path + @"\Lib\Lib1.dll" }, null, null);
            Console.WriteLine(proxy.InvokeMethod("Lib1.Class1", "Hello", "Aphrodite"));
            Console.WriteLine("Name Property: {0}", proxy.GetProperty("Lib1.Class1", "Name"));
            AppDomain.Unload(domain);
            //proxy.InvokeMethod("Lib1.Class1", "Hello", "Aphrodite");//throw System.AppDomainUnloadedException
            proxy = null;
            Console.WriteLine("Used Times: {0}", watch.ElapsedMilliseconds);//110ms

            Console.WriteLine("\r\n Real time compile in a new Domain");
            watch.Restart();
            AppDomain liveApp = AppDomain.CreateDomain("Real time compile");
            proxy = (ProxyObject)liveApp.CreateInstanceFromAndUnwrap("ReflectionDemo.exe", "ReflectionDemo.ProxyObject", false, BindingFlags.Default, null, new object[] { new string[] { File.ReadAllText("Class1.cs") }, new string[] { "InterfaceLib.dll", "System.Core.dll" } }, null, null);
            Console.WriteLine(proxy.InvokeMethod("Lib1.Class1", "Hello", "Phoebe"));
            Console.WriteLine("Name Property: {0}", proxy.GetProperty("Lib1.Class1", "Name"));
            AppDomain.Unload(liveApp);
            proxy = null;
            Console.WriteLine("Used Times: {0}", watch.ElapsedMilliseconds);//320ms
        }
    }

    /// <summary>
    /// Reflection & Real-Time Compile 
    /// </summary>
    class ProxyObject : MarshalByRefObject
    {
        Assembly assembly = null;

        public ProxyObject(string dllFile)
        {
            assembly = Assembly.LoadFile(dllFile);
        }

        /// <summary>
        /// Real Time Compile
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="referAsm"></param>
        public ProxyObject(string[] sources, string[] referAsm)
        {
            Assembly[] asm = AppDomain.CurrentDomain.GetAssemblies();
            string frameworkRefRootDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
            //AppDomain.CurrentDomain.Load(frameworkRefRootDir + "System.dll");//throw an exception

            //Microsoft.CSharp.CSharpCodeProvider provider = new CSharpCodeProvider();
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");//VisualBasic

            CompilerParameters cp = new CompilerParameters(referAsm, null, false);
            // Generate an executable instead of  a class library.
            cp.GenerateExecutable = false;
            // Save the assembly in memory or as a physical file.
            cp.GenerateInMemory = true;
            // Set whether to treat all warnings as errors.
            cp.TreatWarningsAsErrors = false;

            // Invoke compilation.
            CompilerResults cr = provider.CompileAssemblyFromSource(cp, sources);
            if (cr.Errors.Count > 0)
            {
                string err = string.Empty;
                foreach (var ce in cr.Errors)
                {
                    err += string.Format("{0}\r\n", ce.ToString());
                }
                throw new Exception(string.Format("Compile failed, Tips:\r\n{0}", err));
            }
            this.assembly = cr.CompiledAssembly;
        }

        public object InvokeMethod(string fullClassName, string methodName, params Object[] args)
        {
            if (assembly == null)
            {
                throw new Exception("Null assembly reference");
            }

            Type type = assembly.GetType(fullClassName);
            if(type==null)
            {
                throw new Exception("Invalid class name");
            }

            MethodInfo method = type.GetMethod(methodName);
            if (method == null)
            {
                throw new Exception("Invalid method name");
            }

            Object obj = Activator.CreateInstance(type);
            return method.Invoke(obj, args);
        }

        public object GetProperty(string fullClassName, string propertyName)
        {
            if (assembly == null)
            {
                throw new Exception("Null assembly reference");
            }
            Type type = assembly.GetType(fullClassName);
            if (type == null)
            {
                throw new Exception("Invalid class name");
            }
            PropertyInfo property = type.GetProperty(propertyName);
            if (property == null)
            {
                throw new Exception("Invalid property name");
            }
            Object obj = Activator.CreateInstance(type);
            return property.GetValue(obj, null);
        }
    } 
}
