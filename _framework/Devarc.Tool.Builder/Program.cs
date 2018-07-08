using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Devarc
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.SetCallback(Callback_Log);

            Builder_Object builder1 = new Builder_Object();
            Builder_Data builder2 = new Builder_Data();

            if (args.Length > 1)
            {
                if (args.Contains<string>("-idl"))
                {
                    string rawPath = args[1];
                    string outDir = args[2];
                    string srcPath;
                    if (rawPath.Length > 2 && rawPath[1] == ':')
                        srcPath = rawPath;
                    else
                        srcPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), rawPath));
                    Builder_Protocol compiler = new Builder_Protocol();
                    compiler.Build(srcPath, outDir);
                }
                else if (args.Contains<string>("-obj"))
                {
                    if (args.Length > 2)
                        builder1.Build(args[1], args[2]);
                    else
                        builder1.Build(args[1], Path.GetDirectoryName(args[1]));
                }
                else if (args.Contains<string>("-data"))
                {
                    if (args.Length > 2)
                        builder2.Build_ExcelFile(args[1], args[2]);
                    else
                        builder2.Build_ExcelFile(args[1], Path.GetDirectoryName(args[1]));
                }
                else
                {
                    usage();
                }
            }
            else
            {
                usage();
            }
        }

        static void usage()
        {
            Console.WriteLine("[command] [-idl,-obj,-data] [make_file] [out_directory]");
        }

        static void Callback_Log(LOG_TYPE tp, string msg)
        {
            System.Console.WriteLine(msg);
        }


    }
}
