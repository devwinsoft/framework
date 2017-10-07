using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devarc;

namespace TestClient
{
    class Stub_S2C : S2C.IStub
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

        public void RMI_S2C_Notify_Player(HostID remote, HostID _id, DataPlayer _data)
        {

        }

        public void RMI_S2C_Notify_Move(HostID remote, VECTOR3 _look, DIRECTION _move)
        {

        }

        public void RMI_S2C_Notify_Chat(HostID remote, String _msg)
        {
            Log.Info("[{0}] {1}", remote, _msg);
        }

        public bool OnReceive(object sender, NetBuffer msg)
        {
            switch (S2C.Stub.OnReceive(this, msg))
            {
                case RECEIVE_RESULT.INVALID_PACKET:
                    break;
                default:
                    break;
            }
            return true;
        }
    }
}
