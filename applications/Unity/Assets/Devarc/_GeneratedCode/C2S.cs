using System;
using Devarc;
namespace C2S
{
	public interface IStub
	{
		void RMI_C2S_Chat(HostID remote, String _msg);
	}
	public static class Stub
	{
		public static bool OnReceive(IStub stub, NetBuffer _in_msg)
		{
			bool success = true;
			RMI_ID rmi_id = (RMI_ID)_in_msg.Rmi;
			switch (rmi_id)
			{
				case RMI_ID.Chat:
					{
						Log.Debug("Stub(C2S): Chat");
						System.String _msg = default(System.String); success = success ? Marshaler.Read(_in_msg, ref _msg) : false;
						if (_in_msg.Pos != _in_msg.Length) return false;
						if (success) stub.RMI_C2S_Chat(_in_msg.Hid, _msg);
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
		Chat                           = 6000,
	}
	public class Proxy
	{
		private IProxyBase m_Networker = null;
		public void Init(IProxyBase mgr) { m_Networker = mgr; }
		public bool Chat(HostID target, String _msg)
		{
			Log.Debug("C2S.Proxy.Chat");
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Debug("{{0}} is not initialized.", typeof(Proxy));
				return false;
			}
			_out_msg.Init((Int16)RMI_ID.Chat, target);
			Marshaler.Write(_out_msg, _msg);
			return m_Networker.Send(_out_msg.Hid, _out_msg.Data);
		}
	}

}
