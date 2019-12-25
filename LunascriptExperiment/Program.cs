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

namespace LunascriptExperiment
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Program program = new Program();
            program.doScript();
        }

        Assembly[] assemblies;
        Dictionary<string, ObjectInstance> api = new Dictionary<string, ObjectInstance>();
        Engine engine;

        public Program()
        {
            assemblies = new Assembly[]
            {
                
            };
        }
        void doScript()
        {
            engine = new Engine(opt=>
            {
                opt.AllowClr();
                opt.LimitRecursion(100);
                //opt.LimitMemory(1048576);   //Allow only 1 MB
            });
            engine.SetValue("Utils", TypeReference.CreateTypeReference(engine, typeof(Utils)));
            engine.SetValue("println", new Action<object>(write));
            engine.SetValue("register", new Action<string, JsValue>(AddToObjects));
            engine.Execute(readScript());
            //InvokeMethod("pony", "sayName");
            Console.Read();
        }

        string readScript()
        {
            string str = "";
            using(StreamReader sr = new StreamReader("test.js"))
            {
                str = sr.ReadToEnd();
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

        void InvokeMethod(string id, string name,params object[] args)
        {
            Console.WriteLine(api[id].Get("name").AsString());
            ObjectInstance instance = api[id];
            instance.Get(name).Invoke(instance, args.Select(x => JsValue.FromObject(engine, x)).ToArray());
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
}