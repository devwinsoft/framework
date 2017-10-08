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
		public static RECEIVE_RESULT OnReceive(IStub stub, NetBuffer _in_msg)
		{
			RMI_ID rmi_id = (RMI_ID)_in_msg.Rmi;
			switch (rmi_id)
			{
				case RMI_ID.Test:
					{
						Log.Debug("Stub(S2S): Test");
						System.String _name = default(System.String); Marshaler.Read(_in_msg, ref _name);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET;
						stub.RMI_S2S_Test(_in_msg.Hid, _name);
					}
					break;
				default:
					return RECEIVE_RESULT.NOT_IMPLEMENTED;
			}
			return RECEIVE_RESULT.SUCCESS;
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
	public class Proxy : IProxyBase
	{
		private INetworker m_Networker = null;
		public void Init(INetworker mgr) { m_Networker = mgr; }
		public bool Test(HostID target, String _name)
		{
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Debug("{{0}} is not initialized.", typeof(Proxy));
				return false;
			}
			_out_msg.Init((Int16)RMI_ID.Test, target);
			Marshaler.Write(_out_msg, _name);
			if (_out_msg.IsError) return false;
			return m_Networker.Send(_out_msg);
		}
	}

}
