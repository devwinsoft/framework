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
		public static bool OnReceive(IStub stub, NetBuffer _in_msg)
		{
			bool success = true;
			RMI_ID rmi_id = (RMI_ID)_in_msg.Rmi;
			switch (rmi_id)
			{
				case RMI_ID.Notify_Chat:
					{
						Log.Debug("Stub(S2C): Notify_Chat");
						System.String _msg = default(System.String); success = success ? Marshaler.Read(_in_msg, ref _msg) : false;
						if (_in_msg.Pos != _in_msg.Length) return false;
						if (success) stub.RMI_S2C_Notify_Chat(_in_msg.Hid, _msg);
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
		private IProxyBase m_Networker = null;
		public void Init(IProxyBase mgr) { m_Networker = mgr; }
		public bool Notify_Chat(HostID target, String _msg)
		{
			Log.Debug("S2C.Proxy.Notify_Chat");
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Debug("{{0}} is not initialized.", typeof(Proxy));
				return false;
			}
			_out_msg.Init((Int16)RMI_ID.Notify_Chat, target);
			Marshaler.Write(_out_msg, _msg);
			return m_Networker.Send(_out_msg.Hid, _out_msg.Data);
		}
	}

}
