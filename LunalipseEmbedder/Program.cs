using Lunalipse.Resource.Generic.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lunalipse.Resource.Generic.Delegates;

namespace LunalipseEmbedder
{
    class Program
    {
        static int magic = 0;
        static string signature, dest, pwd;
        static bool IsSealMode = false;
        static string[] dirs;
        static string directory;
        static bool silenceMode = false;
        static void Main(string[] args)
        {
            OnSingleEndpointReached += SingleReachingEndpoint;
            OnEndpointReached += ReachingEndpoint;
            OnChuckOperated += OnChuckUpdate;
            for(int i = 0; i < args.Length;i+=2)
            {
                string command = args[i];
                string body = args[i + 1];
                switch (command)
                {
                    case "-i":
                        dirs = body.Split(',');
                        break;
                    case "-dir":
                        directory = body;
                        break;
                    case "-mode":
                        if(body=="seal")
                        {
                            IsSealMode = true;
                        }
                        else IsSealMode = false;
                        break;
                    case "-pwd":
                        pwd = body;
                        break;
                    case "-o":
                        dest = body;
                        break;
                    case "-mg":
                        magic = int.Parse(body,System.Globalization.NumberStyles.HexNumber);
                        break;
                    case "-sign":
                        signature = body;
                        break;
                    case "--slience":
                        silenceMode = true;
                        break;
                }
            }
            Export();
            Console.WriteLine("\npress any key to countinue...");
            Console.ReadKey();
        }

        static void Export()
        {
            if (IsSealMode)
            {
                if (dirs == null)
                {
                    dirs = Directory.GetFiles(directory);
                }
                Writer wr = new Writer(dirs);
                if (string.IsNullOrEmpty(dest))
                {
                    Console.WriteLine("Error: missing output");
                    return;
                }
                wr.MagicNumber = magic == 0 ? 0x4C554E41 : magic;
                wr.outPutPath = dest;
                wr.Passwd = string.IsNullOrEmpty(pwd) ? null : Encoding.ASCII.GetBytes(pwd);
                wr.Signature = signature;
                Console.WriteLine("Perpearing files....");
                wr.Prepare();
                Console.WriteLine("=== You are using Seal Mode , which means you will encapsulate the following files in LRss:");
                foreach (LrssIndex index in wr.GetResources())
                {
                    Console.WriteLine("    + {0}", index.Name);
                    Console.WriteLine("         > Size: {0}B", index.Size);
                }
                if (!silenceMode)
                {
                    Console.Write("Comfirm and proceed? (y/n):");
                    if (Console.ReadKey().KeyChar != 'y')
                    {
                        Console.WriteLine("Rolling back....");
                        return;
                    }
                }
                Console.WriteLine("\nExporting....");
                wr.DoSeal().Wait();
            }
            else
            {
                ReadFile();
            }
        }

        static void ReadFile()
        {
            if (dirs.Length == 0)
            {
                Console.WriteLine("Error: missing input");
                return;
            }
            Reader r = new Reader(dirs[0]);
            Console.WriteLine(" === LRss file loaded ===");
            Console.WriteLine("Signature: {0}", r.Signature);
            Console.WriteLine("Magic: {0}", r.IsPasswordRequired() ? "[* Encrypted *]" : "0x" + r.MagicNumber.ToString("X4"));
            if (r.IsPasswordRequired())
            {
                Console.Write("Enter the password to unlock this file: ");
                InputPwd();
                r.Passwd = Encoding.ASCII.GetBytes(pwd);
                if (!r.Verify())
                {
                    Console.WriteLine("\nError to proceed decryption: Wrong password.");
                    return;
                }
                Console.WriteLine("\n ===== Access Gain =====");
                Console.WriteLine("Signature: {0}", r.Signature);
                Console.WriteLine("Magic: {0}", "0x" + r.MagicNumber.ToString("X4"));

            }
            List<LrssIndex> index = r.Indexes();
            bool Iscontinue = true;
            while (Iscontinue)
            {
                Console.WriteLine();
                Console.WriteLine(" >>>>> Your options:");
                Console.WriteLine("[0] Display file list.");
                Console.WriteLine("[1] Extract file from lrss.");
                Console.WriteLine("[2] Quit.");
                Console.Write("What's your option:");
                switch (InputID())
                {
                    case 0:
                        Display(index);
                        break;
                    case 1:
                        Extract(index, r);
                        break;
                    case 2:
                        Iscontinue = false;
                        break;
                }
            }
        }

        static void Display(List<LrssIndex> index)
        {
            Console.WriteLine("\n === file(s) under {0} ===", Path.GetFileName(dirs[0]));
            for (int j = 0; j < index.Count; j++)
            {
                LrssIndex i = index[j];
                Console.WriteLine("    [{1}] {0}", i.Name, j);
                Console.WriteLine("         > Size    : {0}B", i.Size);
                Console.WriteLine("         > Blocks  : {0}", i.Occupied);
                Console.WriteLine("         > Address : 0x{0}", i.Address.ToString("X16"));
            }
        }

        static bool Extract(List<LrssIndex> index, Reader r)
        {
            Console.Write("Enter the id of file to extract: ");
            int id = InputID();
            while (id < 0 || id > index.Count)
            {
                Console.WriteLine("Invalid Id!");
                Console.Write("Enter the id of file to extract: ");
                id = InputID();
            }
            Console.Write("Select file \"{0}\", want to continue? (y/n): ", index[id].Name);
            if (Console.ReadKey().KeyChar != 'y') return false;
            Console.WriteLine();
            r.OutputResource(index[id]).Wait();
            Console.Write("\nContinue? (y/n):");
            if (Console.ReadKey().KeyChar != 'y') return false;
            return true;
        }

        static void InputPwd()
        {
            pwd = "";
            while (true)
            {
                ConsoleKeyInfo ki = Console.ReadKey(true);
                if (ki.Key == ConsoleKey.Enter) break;
                pwd += ki.KeyChar;
            }
        }
        static int InputID()
        {
            string id = "";
            while (true)
            {
                ConsoleKeyInfo ki = Console.ReadKey();
                if (ki.Key == ConsoleKey.Enter) break;
                id += ki.KeyChar;
            }
            return int.Parse(id);
        }

        static void SingleReachingEndpoint(string args)
        {
            Console.WriteLine("Starting for {0}", args);
        }
        static void ReachingEndpoint(params object[] args)
        {
            Console.WriteLine("\nEncapsulated {0} files", args[0]);
        }

        static void OnChuckUpdate(int current, int total)
        {
            Console.Write("\r   Proceed {0} of {1} chuck(s)   ", current, total);
        }
    }
}
