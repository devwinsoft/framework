using System;
using Devarc;
namespace S2S
{
	namespace Message
	{
		public class Ping
		{
			public TEST_VECTOR pos = new TEST_VECTOR();
			public Byte[] data = null;
		}
	}
	public interface IStub
	{
		void RMI_S2S_Ping(HostID remote, Message.Ping msg);
	}
	public static class Stub
	{
		public static RECEIVE_RESULT OnReceive(IStub stub, NetBuffer _in_msg)
		{
			RMI_ID rmi_id = (RMI_ID)_in_msg.Rmi;
			switch (rmi_id)
			{
				case RMI_ID.Ping:
					{
						Log.Debug("S2S.Stub.Ping");
						Message.Ping msg = new Message.Ping();
						Marshaler.Read(_in_msg, msg.pos);
						Marshaler.Read(_in_msg, out msg.data);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET;
						stub.RMI_S2S_Ping(_in_msg.Hid, msg);
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
		Ping                           = 5000,
	}
	public class Proxy : IProxyBase
	{
		private INetworker m_Networker = null;
		public void Init(INetworker mgr) { m_Networker = mgr; }
		public bool Ping(HostID target, TEST_VECTOR pos, Byte[] data)
		{
			Log.Debug("S2S.Proxy.Ping");
			NetBuffer _out_msg = NetBufferPool.Instance.Pop();
			if (m_Networker == null)
			{
				Log.Debug("{0} is not initialized.", typeof(Proxy));
				return false;
			}
			Marshaler.Write(_out_msg, pos);
			Marshaler.Write(_out_msg, data);
			if (_out_msg.IsError) return false;
			return m_Networker.Send(_out_msg);
		}
	}

}
