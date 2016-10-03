using System;
using Devarc;
namespace S2S
{
	public interface IStub
	{
		void RMI_S2S_Test(HostID remote, String _name);
	}
	public static class Stub
	{
		public static bool OnReceive(IStub stub, int rid, HostID hid, NetBuffer _in_msg)
		{
			bool success = true;
			RMI_ID rmi_id = (RMI_ID)rid;
			switch (rmi_id)
			{
				case RMI_ID.Test:
					{
						Log.Message(LOG_TYPE.DEBUG, "Stub(S2S): Test");
						System.String _name = default(System.String); success = success ? Marshaler.Read(_in_msg, ref _name) : false;
						if (_in_msg.Pos != _in_msg.Length) return false;
						if (success) stub.RMI_S2S_Test(hid, _name);
					}
					break;
				default:
					return false;
			}
			return success;
		}
	}

	public enum RMI_VERSION
	{
		RMI_VERSION                    = 1,
	}
	enum RMI_ID
	{
		Test                           = 5000,
	}
	public class Proxy
	{
		private INetworker m_Networker = null;
		public void SetNetworker(INetworker mgr) { m_Networker = mgr; }
		public bool Test(HostID target, String _name)
		{
			Log.Message(LOG_TYPE.DEBUG, "S2S.Proxy.Test");
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Message(LOG_TYPE.DEBUG, typeof(Proxy).ToString() + " is not initialized.");
				return false;
			}
			m_Networker.RmiHeader(m_Networker.GetMyHostID(), target, _out_msg);
			_out_msg.Write((Int32)RMI_ID.Test);
			Marshaler.Write(_out_msg, _name);
			return m_Networker.RmiSend(m_Networker.GetMyHostID(), target, _out_msg);
		}
	}

}
