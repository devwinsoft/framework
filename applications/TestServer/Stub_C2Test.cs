using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Devarc;

namespace TestServer
{
    class Stub_C2Test : IServerStub, C2Test.IStub
    {
        public void OnNotifyUserConnect(HostID host_hid)
        {
            using(User.LIST.WRITE_LOCK())
            {
                User.LIST.Alloc(host_hid);
            } // unlock
        }
        public void OnNotifyUserDisonnect(HostID host_hid)
        {
            using (User.LIST.WRITE_LOCK())
            {
                User.LIST.Free1(host_hid);
            } // unlock
        }

        public void RMI_C2Test_Chat(HostID remote, String _name, String _msg)
        {
            using (User.LIST.READ_LOCK())
            {
                foreach(User obj in User.LIST.ToArray())
                {
                    TestServer.Test2C.Notify_Chat(obj.hid, _name, _msg);
                }
            } // unlock
        }

        public void RMI_C2Test_Request_UnitData(HostID remote, UNIT _type)
        {

        }


        public void RMI_C2Test_SendFile(HostID remote, String file_name, Byte[] data)
        {
            string dir_path = string.Format("{0}\\save", Directory.GetCurrentDirectory(), file_name);
            if (Directory.Exists(dir_path) == false)
                Directory.CreateDirectory(dir_path);
            string full_path = string.Format("{0}\\save\\{1}", Directory.GetCurrentDirectory(), file_name);
            FileStream fs = File.Create(full_path, data.Length, FileOptions.None);
            if (fs != null)
            {
                fs.Write(data, 0, data.Length);
                fs.Close();
                TestServer.Test2C.Notify_SendFile_Result(remote, true);
                Log.Message(LOG_TYPE.INFO, full_path);
            }
            else
            {
                TestServer.Test2C.Notify_SendFile_Result(remote, false);
            }
        }

        public bool OnReceive(int rid, HostID hid, NetBuffer msg)
        {
            return C2Test.Stub.OnReceive(this, rid, hid, msg);
        }
    }
}
