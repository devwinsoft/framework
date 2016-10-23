using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devarc;

namespace TestClient
{
    class Stub_S2C : IClientStub, S2C.IStub
    {
        public void OnNotifyConnecting()
        {

        }

        public void OnNotifyConnected(HostID host_hid)
        {
        }

        public void OnNotifyDisConnected(DISCONNECTION_REASON reason)
        {
        }

        public void RMI_S2C_Notify_Chat(HostID remote, String _msg)
        {
            Log.Info("[{0}] {1}", remote, _msg);
        }

        public void RMI_Test2C_Notify_SendFile_Result(HostID remote, Boolean _success)
        {
            if (_success)
                Log.Info("Success!");
            else
                Log.Info("Failed!");
        }


        public bool OnReceive(int rid, HostID hid, NetBuffer msg)
        {
            return S2C.Stub.OnReceive(this, rid, hid, msg);
        }
    }
}
