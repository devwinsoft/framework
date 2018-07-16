//
// Copyright (c) 2012 Hyoung Joon, Kim
// http://www.devwinsoft.com/
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

//
// @author Hyoung Joon, Kim (maoshy@nate.com)
// @version $Rev: 1, $Date: 2012-02-20
//

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CSharp;
using LitJson;

namespace Devarc
{
    internal class Builder_Protocol
    {
        public string mAppDir;
        public string mBakCurDir;

        int RMI_START = 0;

        public bool IsProtocol(Type tp)
        {
            foreach (FieldInfo fi in tp.GetFields())
            {
                if (fi.IsLiteral && !fi.IsInitOnly && string.Equals(fi.Name, "RMI_START"))
                    return true;
            }
            return false;
        }

        public void Build(string _cfgPath, string _outDir)
        {
            string cfgFullPath;
            if (_cfgPath.Length > 2 && _cfgPath[1] == ':')
                cfgFullPath = _cfgPath;
            else
                cfgFullPath = Path.Combine(Directory.GetCurrentDirectory(), _cfgPath);

            mBakCurDir = Directory.GetCurrentDirectory();
            mAppDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            if (System.Diagnostics.Debugger.IsAttached)
                mAppDir = Directory.GetCurrentDirectory();
            else
                mAppDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            Directory.SetCurrentDirectory(mAppDir);

            if (File.Exists(cfgFullPath) == false)
            {
                Console.WriteLine("Cannot find config file: {0}", cfgFullPath);
                return;
            }

            // Read Config
            StreamReader cfgReader = File.OpenText(cfgFullPath);
            string cfgData = cfgReader.ReadToEnd();
            cfgReader.Close();

            JsonData jsonRoot = JsonMapper.ToObject(cfgData);
            string srcFile = jsonRoot["SOURCE"].ToString();
            JsonData dataRoot = jsonRoot["IMPORTS"];
            string[] dataFiles = new string[dataRoot.Count];
            for (int i = 0; i < dataFiles.Length; i++)
            {
                string tmpPath = dataRoot[i].ToString();
                if (tmpPath.Length > 2 && tmpPath[1] == ':')
                    dataFiles[i] = tmpPath;
                else
                    dataFiles[i] = Path.Combine(mAppDir, tmpPath);
            }

            // Make temp.dll
            if (MakeDLL(dataFiles) == false)
            {
                Console.WriteLine("Cannot compile data files.");
                Directory.SetCurrentDirectory(mBakCurDir);
                return;
            }

            // Find Source(IDL) File
            string srcFullPath;
            if (srcFile.Length > 2 && srcFile[1] == ':')
                srcFullPath = srcFile;
            else
                srcFullPath = Path.Combine(mBakCurDir, srcFile);
            if (File.Exists(srcFullPath) == false)
            {
                Console.WriteLine("Cannot find source file: {0}", srcFile);
                Directory.SetCurrentDirectory(mBakCurDir);
                return;
            }

            // Read Source(IDL)
            string srcData = File.ReadAllText(srcFullPath);
            var protocolCP = new System.CodeDom.Compiler.CompilerParameters() { GenerateInMemory = true };
            try
            {
                protocolCP.ReferencedAssemblies.Add("Devarc.Base.dll");
                protocolCP.ReferencedAssemblies.Add("temp.dll");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Directory.SetCurrentDirectory(mBakCurDir);
                return;
            }

            CSharpCodeProvider provider = new CSharpCodeProvider();
            var res = provider.CompileAssemblyFromSource(protocolCP, srcData);
            Assembly assem = null;
            try
            {
                assem = res.CompiledAssembly;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Directory.SetCurrentDirectory(mBakCurDir);
                return;
            }

            foreach (Type tp in assem.GetTypes())
            {
                if (this.IsProtocol(tp) == false)
                    continue;

                using (TextWriter sw = new StreamWriter(_outDir + "\\" + tp.Name + ".cs"))
                {
                    Type[] msgClasses = tp.GetNestedTypes(BindingFlags.Public);
                    foreach (FieldInfo finfo in tp.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (finfo.Name == "RMI_START")
                        {
                            object obj = finfo.GetRawConstantValue();
                            RMI_START = (int)obj;
                        }
                    }

                    sw.WriteLine("using System;");
                    sw.WriteLine("using Devarc;");
                    sw.WriteLine("namespace {0}", tp.Name); // start of namespace
                    sw.WriteLine("{");

                    sw.WriteLine("\tnamespace MSG");
                    sw.WriteLine("\t{");
                    foreach (Type msgType in msgClasses)
                    {
                        sw.WriteLine("\t\tpublic class {0}", msgType.Name);
                        sw.WriteLine("\t\t{");
                        foreach (FieldInfo finfo in msgType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                        {
                            if (finfo.FieldType.Name.EndsWith("[]"))
                            {
                                sw.WriteLine("\t\t\tpublic {0} {1} = null;", finfo.FieldType.Name, finfo.Name);
                            }
                            else if (finfo.FieldType.IsClass && finfo.FieldType.Name.ToLower() != "string")
                            {
                                sw.WriteLine("\t\t\tpublic {0} {1} = new {0}();", finfo.FieldType.Name, finfo.Name);
                            }
                            else
                            {
                                sw.WriteLine("\t\t\tpublic {0} {1};", finfo.FieldType.Name, finfo.Name);
                            }
                        }
                        sw.WriteLine("\t\t}");
                    }
                    sw.WriteLine("\t}");

                    sw.WriteLine("\tpublic interface IStub"); // start of stub
                    sw.WriteLine("\t{");
                    foreach (Type msgType in msgClasses)
                    {
                        sw.WriteLine("\t\tvoid RMI_{0}_{1}(HostID remote, {0}.MSG.{1} msg);", tp.Name, msgType.Name);
                    }
                    sw.WriteLine("\t}"); // end of stub

                    // OnReceive
                    sw.WriteLine("\tpublic static class Stub");
                    sw.WriteLine("\t{");
                    sw.WriteLine("\t\tpublic static RECEIVE_RESULT OnReceive(IStub stub, NetBuffer _in_msg)");
                    MakeCode_OnRecv(sw, tp);
                    sw.WriteLine("\t}"); // end of OnReceive
                    sw.WriteLine("");

                    sw.WriteLine("\tpublic enum RMI_VERSION"); // start of version
                    sw.WriteLine("\t{");
                    foreach (FieldInfo finfo in tp.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (finfo.Name == "RMI_VERSION")
                        {
                            object obj = finfo.GetRawConstantValue();
                            sw.WriteLine("\t\t{0,-30} = {1},", "RMI_VERSION", (int)obj);
                            break;
                        }
                    }
                    sw.WriteLine("\t}"); // end of version

                    sw.WriteLine("\tenum RMI_ID"); // start of enum
                    sw.WriteLine("\t{");
                    foreach (Type msgType in msgClasses)
                    {
                        sw.WriteLine("\t\t{0,-30} = {1},", msgType.Name, RMI_START++);
                    }
                    sw.WriteLine("\t}"); // end of enum

                    sw.WriteLine("\tpublic class Proxy : IProxyBase"); // start of proxy
                    sw.WriteLine("\t{");
                    sw.WriteLine("\t\tprivate INetworker m_Networker = null;");
                    sw.WriteLine("\t\tpublic void Init(INetworker mgr) { m_Networker = mgr; }");

                    foreach (Type msgType in msgClasses)
                    {
                        sw.Write("\t\tpublic bool {0}(HostID target", msgType.Name);
                        foreach (FieldInfo finfo in msgType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                        {
                            sw.Write(", " + finfo.FieldType.Name + " " + finfo.Name);
                        }
                        sw.WriteLine(")");

                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tLog.Debug(\"{0}.Proxy.{1}\");", tp.Name, msgType.Name);
                        sw.WriteLine("\t\t\tNetBuffer _out_msg = NetBufferPool.Instance.Pop();");
                        sw.WriteLine("\t\t\tif (m_Networker == null)");
                        sw.WriteLine("\t\t\t{");
                        sw.WriteLine("\t\t\t\tLog.Debug(\"{0} is not initialized.\", typeof(Proxy));");
                        sw.WriteLine("\t\t\t\treturn false;");
                        sw.WriteLine("\t\t\t}");

                        sw.WriteLine("\t\t\t_out_msg.Init((Int16)RMI_ID.{0}, target);", msgType.Name);
                        foreach (FieldInfo finfo in msgType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                        {
                            sw.WriteLine("\t\t\tMarshaler.Write(_out_msg, {0});", finfo.Name);
                        }
                        sw.WriteLine("\t\t\tif (_out_msg.IsError) return false;");
                        sw.WriteLine("\t\t\treturn m_Networker.Send(_out_msg);");
                        sw.WriteLine("\t\t}");
                    }
                    sw.WriteLine("\t}"); // end of proxy
                    sw.WriteLine("");

                    sw.WriteLine("}"); // end of namespace
                } // close file
            }

            Directory.SetCurrentDirectory(mBakCurDir);
        } // end of Export


        private bool MakeDLL(string[] _dataFiles)
        {
            string outputPath = Path.Combine(mAppDir, "temp.dll");
            Log.Info(outputPath);

            // Initialize CompilerParameters
            var cp = new System.CodeDom.Compiler.CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = false;
            cp.OutputAssembly = outputPath;

            // Generate Source
            Builder_SimpleCode builder = new Builder_SimpleCode();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _dataFiles.Length; i++)
            {
                string ext = Path.GetExtension(_dataFiles[i]).ToLower();
                switch(ext)
                {
                    case ".xlsx":
                    case ".xls":
                        builder.Read(_dataFiles[i]);
                        break;
                    default:
                        break;
                }
            }
            sb.Append(builder.ToString());

            for (int i = 0; i < _dataFiles.Length; i++)
            {
                string ext = Path.GetExtension(_dataFiles[i]).ToLower();
                switch (ext)
                {
                    case ".cs":
                    case ".schema":
                        string tmpData = File.ReadAllText(_dataFiles[i]);
                        sb.Append(tmpData);
                        break;
                    default:
                        break;
                }
            }

            // Generate DLL
            try
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                var cr = provider.CompileAssemblyFromSource(cp, sb.ToString());
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private void MakeCode_OnRecv(TextWriter sw, Type tp)
        {
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tRMI_ID rmi_id = (RMI_ID)_in_msg.Rmi;");
            sw.WriteLine("\t\t\tswitch (rmi_id)");
            sw.WriteLine("\t\t\t{");

            Type[] msgClasses = tp.GetNestedTypes(BindingFlags.Public);
            foreach (Type msgType in msgClasses)
            {
                sw.WriteLine("\t\t\t\tcase RMI_ID.{0}:", msgType.Name);
                sw.WriteLine("\t\t\t\t\t{");
                sw.WriteLine("\t\t\t\t\t\tLog.Debug(\"{0}.Stub.{1}\");", tp.Name, msgType.Name);
                sw.WriteLine("\t\t\t\t\t\tMSG.{0} msg = new MSG.{0}();", msgType.Name);
                foreach (FieldInfo finfo in msgType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (finfo.FieldType.Name.EndsWith("[]"))
                    {
                        string type_name = finfo.FieldType.ToString().Substring(0, finfo.FieldType.ToString().Length - 2);
                        sw.WriteLine("\t\t\t\t\t\tMarshaler.Read(_in_msg, out msg.{1});", finfo.FieldType, finfo.Name, type_name);
                    }
                    else if (finfo.FieldType.IsClass && finfo.FieldType.Name.ToLower() != "string")
                    {
                        sw.WriteLine("\t\t\t\t\t\tMarshaler.Read(_in_msg, msg.{1});", finfo.FieldType, finfo.Name);
                    }
                    else
                    {
                        sw.WriteLine("\t\t\t\t\t\tMarshaler.Read(_in_msg, ref msg.{1});", finfo.FieldType, finfo.Name);
                    }
                }
                sw.WriteLine("\t\t\t\t\t\tif (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET;");
                sw.WriteLine("\t\t\t\t\t\tstub.RMI_{0}_{1}(_in_msg.Hid, msg);", tp.Name, msgType.Name);
                sw.WriteLine("\t\t\t\t\t}");
                sw.WriteLine("\t\t\t\t\tbreak;");
            }
            sw.WriteLine("\t\t\t\tdefault:");
            sw.WriteLine("\t\t\t\t\treturn RECEIVE_RESULT.NOT_IMPLEMENTED;");
            sw.WriteLine("\t\t\t}");
            sw.WriteLine("\t\t\treturn RECEIVE_RESULT.SUCCESS;");
            sw.WriteLine("\t\t}");
        }

    } // end of class
}
