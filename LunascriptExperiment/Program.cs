using Jint;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime.Interop;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LunascriptExperiment
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Program program = new Program();
            program.doScript();
        }
        Dictionary<string, ObjectInstance> api = new Dictionary<string, ObjectInstance>();
        Engine engine;

        public Program()
        {
        }
        void doScript()
        {
            engine = new Engine(opt=>
            {
                opt.AllowClr(typeof(MessageBox).Assembly);
                opt.AddRestrictedNamespace(
                    "System.IO",
                    "System.Net", 
                    "System.Reflection",
                    "System.Printing",
                    "System.Runtime",
                    "System.Web"
                );
                opt.LimitRecursion(100);
            });
            engine.SetValue("Utils", TypeReference.CreateTypeReference(engine, typeof(Utils)));
            engine.SetValue("testClass", TypeReference.CreateTypeReference(engine, typeof(testClass)));
            engine.SetValue("println", new Action<object>(write));
            engine.SetValue("register", new Action<string, JsValue>(AddToObjects));
            engine.Execute(readScript());
            InvokeMemberMethod("pony", "sayName");
            InvokeMemberMethod("pony", "sayType");
            ObjectInstance instance = api["test"];
            testClass obj = instance.ToObject() as testClass;
            if(obj!=null)
            {
                Console.WriteLine(obj.testField);
            }
            Console.Read();
        }

        string readScript()
        {
            string str = "";
            using (FileStream fs = new FileStream("test.js", FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    str = sr.ReadToEnd();
                }
            }
            return str;
        }

        void write(object obj)
        {
            Console.WriteLine(obj);
        }

        void AddToObjects(string id, JsValue instance)
        {
            api.Add(id, instance.AsObject());
        }

        void InvokeMemberMethod(string id, string name,params object[] args)
        {
            ObjectInstance instance = api[id];
            JsValue jvalue = instance.Get(name);
            if (jvalue != JsValue.Undefined)
            {
                jvalue.Invoke(instance, args.Select(x => JsValue.FromObject(engine, x)).ToArray());
            }
            else
            {
                Console.WriteLine($"{name} not exist");
            }
        }
    }

    class Utils
    {
        static Random random = new Random();
        public static int GetRand(int max)
        {
            return random.Next(max);
        }
    }
    interface IInterface
    {
        void getStuff();
    }

    class testClass
    {
        public int testField = 0;
    }
}