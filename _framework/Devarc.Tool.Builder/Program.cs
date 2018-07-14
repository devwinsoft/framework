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
            Log.OnMessage += Callback_Log;

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
                    Builder_Object builder1 = new Builder_Object();
                    if (args.Length > 2)
                        builder1.Build(args[1], args[2]);
                    else
                        builder1.Build(args[1], Path.GetDirectoryName(args[1]));
                }
                else if (args.Contains<string>("-data"))
                {
                    Builder_Data builder2 = new Builder_Data();
                    if (args.Length > 2)
                        builder2.Build_ExcelFile(args[1], args[2]);
                    else
                        builder2.Build_ExcelFile(args[1], Path.GetDirectoryName(args[1]));
                }
                else if (args.Contains<string>("-sql"))
                {
                    Builder_SQL builder3 = new Builder_SQL();
                    if (args.Length > 2)
                        builder3.Build_ExcelFile(args[1], args[2]);
                    else
                        builder3.Build_ExcelFile(args[1], Path.GetDirectoryName(args[1]));
                }
                else if (args.Contains<string>("-mysql"))
                {
                    Builder_MySQL builder3 = new Builder_MySQL();
                    if (args.Length > 2)
                        builder3.Build_ExcelFile(args[1], args[2]);
                    else
                        builder3.Build_ExcelFile(args[1], Path.GetDirectoryName(args[1]));
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
