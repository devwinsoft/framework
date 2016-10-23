using System;
using Devarc;
namespace S2C
{
	public interface IStub
	{
		void RMI_S2C_Notify_Chat(HostID remote, String _msg);
	}
	public static class Stub
	{
		public static bool OnReceive(IStub stub, int rid, HostID hid, NetBuffer _in_msg)
		{
			bool success = true;
			RMI_ID rmi_id = (RMI_ID)rid;
			switch (rmi_id)
			{
				case RMI_ID.Notify_Chat:
					{
						Log.Debug("Stub(S2C): Notify_Chat");
						System.String _msg = default(System.String); success = success ? Marshaler.Read(_in_msg, ref _msg) : false;
						if (_in_msg.Pos != _in_msg.Length) return false;
						if (success) stub.RMI_S2C_Notify_Chat(hid, _msg);
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
		Notify_Chat                    = 5000,
	}
	public class Proxy
	{
		private INetworker m_Networker = null;
		public void SetNetworker(INetworker mgr) { m_Networker = mgr; }
		public bool Notify_Chat(HostID target, String _msg)
		{
			Log.Debug("S2C.Proxy.Notify_Chat");
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Debug(typeof(Proxy).ToString() + " is not initialized.");
				return false;
			}
			m_Networker.RmiHeader(m_Networker.GetMyHostID(), target, _out_msg);
			_out_msg.Write((Int32)RMI_ID.Notify_Chat);
			Marshaler.Write(_out_msg, _msg);
			return m_Networker.RmiSend(m_Networker.GetMyHostID(), target, _out_msg);
		}
	}

}
