using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devarc;

namespace TestClient
{
    class Stub_S2C : IStubBase, S2C.IStub
    {
        public void RMI_S2C_Notify_Player(HostID remote, S2C.MSG.Notify_Player msg)
        {

        }

        public void RMI_S2C_Notify_Move(HostID remote, S2C.MSG.Notify_Move msg)
        {

        }

        public void RMI_S2C_Notify_Chat(HostID remote, S2C.MSG.Notify_Chat msg)
        {
            Log.Info("[{0}] {1}", remote, msg._msg);
        }

        public bool OnReceiveData(object sender, NetBuffer msg)
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
