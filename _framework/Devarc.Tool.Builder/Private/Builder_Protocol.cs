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
using System.Collections.Generic;
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
        public string mCfgDir;
        public string mBakCurDir;

        public bool IsProtocol(Type tp)
        {
            foreach (FieldInfo fi in tp.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (fi.IsLiteral && !fi.IsInitOnly && string.Equals(fi.Name, "RMI_VERSION"))
                    return true;
            }
            return false;
        }

        public void Build(string _cfgPath, string _outDir, bool _php)
        {
            string cfgFullPath;
            if (_cfgPath.Length > 2 && _cfgPath[1] == ':')
                cfgFullPath = _cfgPath;
            else
                cfgFullPath = Path.Combine(Directory.GetCurrentDirectory(), _cfgPath);

            mBakCurDir = Directory.GetCurrentDirectory();
            mAppDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            mCfgDir = Path.GetDirectoryName(cfgFullPath);

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
                    dataFiles[i] = Path.Combine(mCfgDir, tmpPath);
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
                srcFullPath = Path.Combine(mCfgDir, srcFile);
            if (File.Exists(srcFullPath) == false)
            {
                Console.WriteLine("Cannot find source file: {0}", srcFile);
                Directory.SetCurrentDirectory(mBakCurDir);
                return;
            }

            // Read Source(IDL)
            string srcData = File.ReadAllText(srcFullPath);
            var protocolCP = new System.CodeDom.Compiler.CompilerParameters();
            protocolCP.GenerateExecutable = false;
            protocolCP.GenerateInMemory = true;
            try
            {
                protocolCP.ReferencedAssemblies.Add("Devarc.Base.dll");
                protocolCP.ReferencedAssemblies.Add("temp.dll");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
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
                Log.Error(ex.Message);
                Directory.SetCurrentDirectory(mBakCurDir);
                return;
            }

            foreach (Type protocolType in assem.GetTypes())
            {
                if (this.IsProtocol(protocolType) == false)
                    continue;

                MakeCSharp(protocolType, _outDir + "\\" + protocolType.Name + ".cs");
                if (_php)
                {
                    MakePHP(protocolType, _outDir + "\\" + protocolType.Name + ".php");
                }
            }

            Directory.SetCurrentDirectory(mBakCurDir);
        } // end of Export


        private bool MakeDLL(string[] _dataFiles)
        {
            string outputPath = Path.Combine(mAppDir, "temp.dll");

            // Initialize CompilerParameters
            var cp = new System.CodeDom.Compiler.CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = false;
            cp.ReferencedAssemblies.Add("Devarc.Base.dll");
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
                Log.Error(ex.Message);
                return false;
            }
        }

        private void MakePHP(Type _protocolType, string _filePath)
        {
            Dictionary<Type, short> rmiList = new Dictionary<Type, short>();
            using (TextWriter sw = new StreamWriter(_filePath))
            {
                Type[] msgClasses = _protocolType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (Type msgType in msgClasses)
                {
                    NetProtocolAttribute attribute = msgType.GetCustomAttribute<NetProtocolAttribute>();
                    if (attribute != null)
                    {
                        rmiList.Add(msgType, attribute.RMI_ID);
                    }
                }

                sw.WriteLine("<?php namespace Devarc\\Protocol;");
                sw.WriteLine("class {0}", _protocolType.Name);
                sw.WriteLine("{");
                sw.WriteLine("\tvar $message = null;");
                sw.WriteLine("\tfunction __construct($_header, $_body)");
                sw.WriteLine("\t{");
                sw.WriteLine("\t\t$this->message = new \\Devarc\\Component\\RMIMessage($_header, $_body);");
                sw.WriteLine("\t}");
                sw.WriteLine("\tpublic function dispatch()");
                sw.WriteLine("\t{");
                sw.WriteLine("\t\tswitch($this->message->rmi_id)");
                sw.WriteLine("\t\t{");
                foreach (Type msgType in msgClasses)
                {
                    short rmi_id = 0;
                    if (rmiList.TryGetValue(msgType, out rmi_id) == false)
                    {
                        //Log.Warning("[{0}] has not NetProtocolAttribute.", msgType.Name);
                    }
                    sw.WriteLine("\t\t\tcase {0}: // {1}", rmi_id, msgType.Name);
                    sw.WriteLine("\t\t\t\t$proc = new \\Devarc\\Protocol\\{0}($this->message);", msgType.Name);
                    sw.WriteLine("\t\t\t\t$proc->dispatch();");
                    sw.WriteLine("\t\t\t\tbreak;");
                }
                sw.WriteLine("\t\t\tdefault:");
                sw.WriteLine("\t\t\t\techo '{\"Error\":1 ,\"Message\":\"Not Implemented.\"}'; ");
                sw.WriteLine("\t\t\t\tbreak;");
                sw.WriteLine("\t\t}");
                sw.WriteLine("\t}");
                sw.WriteLine("}");
                sw.WriteLine("?>");
            }
        }

        private void MakeCSharp(Type _type, string _filePath)
        {
            Dictionary<Type, short> rmiList = new Dictionary<Type, short>();
            using (TextWriter sw = new StreamWriter(_filePath))
            {
                Type[] msgClasses = _type.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                sw.WriteLine("using System;");
                sw.WriteLine("using System.Text;");
                sw.WriteLine("using System.Collections;");
                sw.WriteLine("using System.Collections.Generic;");
                sw.WriteLine("using LitJson;");
                sw.WriteLine("using Devarc;");

                sw.WriteLine("namespace Devarc.{0}", _type.Name); // start of namespace
                sw.WriteLine("{");

                sw.WriteLine("\tpublic interface IStub"); // start of stub
                sw.WriteLine("\t{");
                foreach (Type msgType in msgClasses)
                {
                    sw.WriteLine("\t\tvoid RMI_{0}_{1}(HostID remote, {1} msg);", _type.Name, msgType.Name);
                }
                sw.WriteLine("\t}"); // end of stub

                // OnReceive
                sw.WriteLine("\tpublic static class Stub");
                sw.WriteLine("\t{");
                sw.WriteLine("\t\tpublic static RECEIVE_RESULT OnReceive(IStub stub, NetBuffer _in_msg)");
                MakeCSharp_OnRecv(sw, _type);
                sw.WriteLine("\t}"); // end of OnReceive
                sw.WriteLine("");

                sw.WriteLine("\tpublic enum RMI_VERSION"); // start of version
                sw.WriteLine("\t{");
                foreach (FieldInfo finfo in _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (finfo.Name == "RMI_VERSION")
                    {
                        object obj = finfo.GetRawConstantValue();
                        sw.WriteLine("\t\t{0,-30} = {1},", "NUMBER", (int)obj);
                        break;
                    }
                }
                sw.WriteLine("\t}"); // end of version

                sw.WriteLine("\tenum RMI_ID"); // start of enum
                sw.WriteLine("\t{");
                foreach (Type msgType in msgClasses)
                {
                    NetProtocolAttribute attribute = msgType.GetCustomAttribute<NetProtocolAttribute>();
                    if (attribute != null)
                    {
                        sw.WriteLine("\t\t{0,-30} = {1},", msgType.Name, attribute.RMI_ID);
                        rmiList.Add(msgType, attribute.RMI_ID);
                    }
                    else
                    {
                        sw.WriteLine("\t\t{0,-30} = {1},", msgType.Name, 0);
                    }
                }
                sw.WriteLine("\t}"); // end of enum

                sw.WriteLine("\tpublic class Proxy : ProxyBase"); // start of proxy
                sw.WriteLine("\t{");
                sw.WriteLine("\t\tpublic SEND_RESULT Send(NetBuffer msg)");
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tif (mNetworker == null)");
                sw.WriteLine("\t\t\t{");
                sw.WriteLine("\t\t\t\tLog.Debug(\"{0} is not initialized.\", typeof(Proxy));");
                sw.WriteLine("\t\t\t\treturn SEND_RESULT.NOT_INITIALIZED;");
                sw.WriteLine("\t\t\t}");
                sw.WriteLine("\t\t\treturn mNetworker.Send(msg);");
                sw.WriteLine("\t\t}");
                foreach (Type msgType in msgClasses)
                {
                    sw.Write("\t\tpublic SEND_RESULT {0}(HostID target", msgType.Name);
                    foreach (FieldInfo finfo in msgType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        sw.Write(", " + finfo.FieldType.Name + " " + finfo.Name);
                    }
                    sw.WriteLine(")");

                    sw.WriteLine("\t\t{");
                    sw.WriteLine("\t\t\tLog.Debug(\"{0}.Proxy.{1}\");", _type.Name, msgType.Name);
                    sw.WriteLine("\t\t\tNetBuffer _out_msg = NetBufferPool.Instance.Pop();");
                    sw.WriteLine("\t\t\t_out_msg.Init((Int16)RMI_ID.{0}, target);", msgType.Name);
                    foreach (FieldInfo finfo in msgType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        sw.WriteLine("\t\t\tMarshaler.Write(_out_msg, {0});", finfo.Name);
                    }
                    sw.WriteLine("\t\t\treturn Send(_out_msg);");
                    sw.WriteLine("\t\t}");
                }
                sw.WriteLine("\t}"); // end of proxy
                sw.WriteLine("}"); // end of namespace


                sw.WriteLine("namespace Devarc.{0}", _type.Name);
                sw.WriteLine("{");
                foreach (Type msgType in msgClasses)
                {
                    PropTable tb = Builder_Util.ToTable(msgType);
                    short rmi_id = 0;
                    if (rmiList.TryGetValue(msgType, out rmi_id) == false)
                    {
                        Log.Warning("[{0}] has not NetProtocolAttribute.", msgType.Name);
                    }
                    Builder_Util.Make_Class_Code(tb, sw, rmi_id);
                }
                sw.WriteLine("}");
            } // close file
        }

        private void MakeCSharp_OnRecv(TextWriter sw, Type tp)
        {
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tRMI_ID rmi_id = (RMI_ID)_in_msg.Rmi;");
            sw.WriteLine("\t\t\tswitch (rmi_id)");
            sw.WriteLine("\t\t\t{");

            Type[] msgClasses = tp.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (Type msgType in msgClasses)
            {
                sw.WriteLine("\t\t\t\tcase RMI_ID.{0}:", msgType.Name);
                sw.WriteLine("\t\t\t\t\ttry");
                sw.WriteLine("\t\t\t\t\t{");
                sw.WriteLine("\t\t\t\t\t\tLog.Debug(\"{0}.Stub.{1}\");", tp.Name, msgType.Name);
                sw.WriteLine("\t\t\t\t\t\t{0} msg = new {0}();", msgType.Name);
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
                sw.WriteLine("\t\t\t\t\t\tif (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET_DOWNFLOW;");
                sw.WriteLine("\t\t\t\t\t\tstub.RMI_{0}_{1}(_in_msg.Hid, msg);", tp.Name, msgType.Name);
                sw.WriteLine("\t\t\t\t\t}");
                sw.WriteLine("\t\t\t\t\tcatch (NetException ex)");
                sw.WriteLine("\t\t\t\t\t{");
                sw.WriteLine("\t\t\t\t\t\treturn ex.ERROR;");
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
