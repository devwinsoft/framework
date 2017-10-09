﻿//
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
using System.Reflection;

namespace Devarc
{
    class Builder_Net
    {
        int RMI_START = 0;

        bool IsValid(MethodInfo _minfo)
        {
            try
            {
                _minfo.GetParameters();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsProtocol(Type tp)
        {
            foreach (FieldInfo fi in tp.GetFields())
            {
                if (fi.IsLiteral && !fi.IsInitOnly && fi.Name == "RMI_START")
                    return true;
            }
            return false;
        }

        public void BuildFromFile(string _inputFile, string _outDir)
        {
            if (File.Exists(_inputFile) == false)
            {
                Console.WriteLine("Cannot find file: ");
                return;
            }

            string full_name = System.IO.Path.GetFullPath(_inputFile);
            Assembly assem = Assembly.LoadFile(full_name);
            if (assem == null)
            {
                Console.WriteLine("Cannot open file: ");
                return;
            }
            _build(assem, _outDir);
        }

        public void BuildFromAssem(Assembly _assem, string _outDir)
        {
            _build(_assem, _outDir);
        }

        void _build(Assembly _assem, string _outDir)
        {
            MethodInfo[] _methods;
            foreach (Type tp in _assem.GetTypes())
            {
                if (this.IsProtocol(tp) == false)
                    continue;

                using (TextWriter sw = new StreamWriter(_outDir + "\\" + tp.Name + ".cs"))
                {
                    foreach (FieldInfo finfo in tp.GetFields())
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

                    sw.WriteLine("\tpublic interface IStub"); // start of stub
                    sw.WriteLine("\t{");
                    _methods = tp.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    foreach (MethodInfo minfo in _methods)
                    {
                        if (IsValid(minfo) == false)
                            continue;
                        if (minfo.Name == "ToString" || minfo.Name == "Equals" || minfo.Name == "GetHashCode" || minfo.Name == "GetType")
                        {
                            continue;
                        }
                        sw.Write("\t\tvoid RMI_{0}_{1}(HostID remote", tp.Name, minfo.Name);
                        foreach (ParameterInfo pinfo in minfo.GetParameters())
                        {
                            sw.Write(", " + pinfo.ParameterType.Name + " " + pinfo.Name);
                        }
                        sw.WriteLine(");");
                    }
                    sw.WriteLine("\t}"); // end of stub

                    // OnReceive
                    sw.WriteLine("\tpublic static class Stub");
                    sw.WriteLine("\t{");
                    sw.WriteLine("\t\tpublic static RECEIVE_RESULT OnReceive(IStub stub, NetBuffer _in_msg)");
                    MakeOnRecvCode(sw, tp);
                    sw.WriteLine("\t}"); // end of OnReceive
                    sw.WriteLine("");

                    sw.WriteLine("\tpublic enum RMI_VERSION"); // start of version
                    sw.WriteLine("\t{");
                    foreach (FieldInfo finfo in tp.GetFields())
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
                    _methods = tp.GetMethods();
                    foreach (MethodInfo minfo in _methods)
                    {
                        if (IsValid(minfo) == false)
                            continue;
                        if (minfo.Name == "ToString" || minfo.Name == "Equals" || minfo.Name == "GetHashCode" || minfo.Name == "GetType")
                        {
                            continue;
                        }
                        sw.WriteLine("\t\t{0,-30} = {1},", minfo.Name, RMI_START++);
                    }
                    sw.WriteLine("\t}"); // end of enum

                    sw.WriteLine("\tpublic class Proxy : IProxyBase"); // start of proxy
                    sw.WriteLine("\t{");
                    sw.WriteLine("\t\tprivate INetworker m_Networker = null;");
                    sw.WriteLine("\t\tpublic void Init(INetworker mgr) { m_Networker = mgr; }");
                    _methods = tp.GetMethods();
                    foreach (MethodInfo minfo in _methods)
                    {
                        if (IsValid(minfo) == false)
                            continue;
                        if (minfo.Name == "ToString" || minfo.Name == "Equals" || minfo.Name == "GetHashCode" || minfo.Name == "GetType")
                        {
                            continue;
                        }
                        sw.Write("\t\tpublic bool {0}(HostID target", minfo.Name);
                        foreach (ParameterInfo pinfo in minfo.GetParameters())
                        {
                            sw.Write(", " + pinfo.ParameterType.Name + " " + pinfo.Name);
                        }
                        sw.WriteLine(")");

                        sw.WriteLine("\t\t{");
                        //sw.WriteLine("\t\t\tLog.Debug(\"{0}.Proxy.{1}\");", tp.Name, minfo.Name);
                        sw.WriteLine("\t\t\tNetBuffer _out_msg = NetBufferPool.Instance.Pop();");
                        sw.WriteLine("\t\t\tif (m_Networker == null)");
                        sw.WriteLine("\t\t\t{");
                        sw.WriteLine("\t\t\t\tLog.Debug(\"{{0}} is not initialized.\", typeof(Proxy));");
                        sw.WriteLine("\t\t\t\treturn false;");
                        sw.WriteLine("\t\t\t}");
                        sw.WriteLine("\t\t\t_out_msg.Init((Int16)RMI_ID.{0}, target);", minfo.Name);

                        foreach (ParameterInfo pinfo in minfo.GetParameters())
                        {
                            sw.WriteLine("\t\t\tMarshaler.Write(_out_msg, {0});", pinfo.Name);
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

            return;
        } // end of Export

        private void MakeOnRecvCode(TextWriter sw, Type tp)
        {
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tRMI_ID rmi_id = (RMI_ID)_in_msg.Rmi;");
            sw.WriteLine("\t\t\tswitch (rmi_id)");
            sw.WriteLine("\t\t\t{");
            MethodInfo[] _methods = tp.GetMethods();
            foreach (MethodInfo minfo in _methods)
            {
                if (IsValid(minfo) == false)
                    continue;
                if (minfo.Name == "ToString" || minfo.Name == "Equals" || minfo.Name == "GetHashCode" || minfo.Name == "GetType")
                {
                    continue;
                }
                sw.WriteLine("\t\t\t\tcase RMI_ID.{0}:", minfo.Name);
                sw.WriteLine("\t\t\t\t\t{");
                sw.WriteLine("\t\t\t\t\t\tLog.Debug(\"Stub({0}): {1}\");", tp.Name, minfo.Name);
                foreach (ParameterInfo pinfo in minfo.GetParameters())
                {
                    if (pinfo.ParameterType.Name.EndsWith("[]"))
                    {
                        string type_name = pinfo.ParameterType.ToString().Substring(0, pinfo.ParameterType.ToString().Length - 2);
                        sw.WriteLine("\t\t\t\t\t\t{0} {1}; Marshaler.Read(_in_msg, out {1});", pinfo.ParameterType, pinfo.Name, type_name);
                    }
                    else if (pinfo.ParameterType.IsClass && pinfo.ParameterType.Name.ToLower() != "string")
                    {
                        sw.WriteLine("\t\t\t\t\t\t{0} {1} = new {0}(); Marshaler.Read(_in_msg, {1});", pinfo.ParameterType, pinfo.Name);
                    }
                    else
                    {
                        sw.WriteLine("\t\t\t\t\t\t{0} {1} = default({0}); Marshaler.Read(_in_msg, ref {1});", pinfo.ParameterType, pinfo.Name);
                    }
                }

                sw.WriteLine("\t\t\t\t\t\tif (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET;");
                sw.Write("\t\t\t\t\t\tstub.RMI_{0}_{1}(_in_msg.Hid", tp.Name, minfo.Name);
                foreach (ParameterInfo pinfo in minfo.GetParameters())
                {
                    sw.Write(", {0}", pinfo.Name);
                }
                sw.WriteLine(");");

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
