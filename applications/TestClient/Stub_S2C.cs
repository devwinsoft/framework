using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devarc;

namespace TestClient
{
    class Stub_S2C : StubBase, S2C.IStub
    {
        public void RMI_S2C_Notify_Player(HostID remote, Notify_Player msg)
        {

        }

        public void RMI_S2C_Notify_Move(HostID remote, Notify_Move msg)
        {

        }

        public void RMI_S2C_Notify_Chat(HostID remote, Notify_Chat msg)
        {
            Log.Info("[{0}] {1}", remote, msg._msg);
        }

        public override bool OnReceiveData(object sender, NetBuffer msg)
        {
            switch (S2C.Stub.OnReceive(this, msg))
            {
                case RECEIVE_RESULT.SUCCESS:
                    return true;
                case RECEIVE_RESULT.INVALID_PACKET_DOWNFLOW:
                case RECEIVE_RESULT.INVALID_PACKET_OVERFLOW:
                    break;
                default:
                    break;
            }
            return false;
        }
    }
}
