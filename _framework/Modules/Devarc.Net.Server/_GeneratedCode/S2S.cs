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
		public static bool OnReceive(IStub stub, NetBuffer _in_msg)
		{
			bool success = true;
			RMI_ID rmi_id = (RMI_ID)_in_msg.Rmi;
			switch (rmi_id)
			{
				case RMI_ID.Test:
					{
						Log.Debug("Stub(S2S): Test");
						System.String _name = default(System.String); success = success ? Marshaler.Read(_in_msg, ref _name) : false;
						if (_in_msg.Pos != _in_msg.Length) return false;
						if (success) stub.RMI_S2S_Test(_in_msg.Hid, _name);
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
		private IProxyBase m_Networker = null;
		public void Init(IProxyBase mgr) { m_Networker = mgr; }
		public bool Test(HostID target, String _name)
		{
			Log.Debug("S2S.Proxy.Test");
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Debug("{{0}} is not initialized.", typeof(Proxy));
				return false;
			}
			_out_msg.Init((Int16)RMI_ID.Test, target);
			Marshaler.Write(_out_msg, _name);
			return m_Networker.Send(_out_msg.Hid, _out_msg.Data);
		}
	}

}
