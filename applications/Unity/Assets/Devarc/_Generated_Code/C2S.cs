using System;
using Devarc;
namespace C2S
{
	namespace MSG
	{
		public class Move
		{
			public VECTOR3 look = new VECTOR3();
			public DIRECTION move;
		}
		public class Chat
		{
			public String msg;
		}
	}
	public interface IStub
	{
		void RMI_C2S_Move(HostID remote, C2S.MSG.Move msg);
		void RMI_C2S_Chat(HostID remote, C2S.MSG.Chat msg);
	}
	public static class Stub
	{
		public static RECEIVE_RESULT OnReceive(IStub stub, NetBuffer _in_msg)
		{
			RMI_ID rmi_id = (RMI_ID)_in_msg.Rmi;
			switch (rmi_id)
			{
				case RMI_ID.Move:
					{
						Log.Debug("C2S.Stub.Move");
						MSG.Move msg = new MSG.Move();
						Marshaler.Read(_in_msg, msg.look);
						Marshaler.Read(_in_msg, ref msg.move);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET;
						stub.RMI_C2S_Move(_in_msg.Hid, msg);
					}
					break;
				case RMI_ID.Chat:
					{
						Log.Debug("C2S.Stub.Chat");
						MSG.Chat msg = new MSG.Chat();
						Marshaler.Read(_in_msg, ref msg.msg);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET;
						stub.RMI_C2S_Chat(_in_msg.Hid, msg);
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
	}
	enum RMI_ID
	{
		Move                           = 0,
		Chat                           = 1,
	}
	public class Proxy : IProxyBase
	{
		private INetworker m_Networker = null;
		public void Init(INetworker mgr) { m_Networker = mgr; }
		public bool Move(HostID target, VECTOR3 look, DIRECTION move)
		{
			Log.Debug("C2S.Proxy.Move");
			NetBuffer _out_msg = NetBufferPool.Instance.Pop();
			if (m_Networker == null)
			{
				Log.Debug("{0} is not initialized.", typeof(Proxy));
				return false;
			}
			_out_msg.Init((Int16)RMI_ID.Move, target);
			Marshaler.Write(_out_msg, look);
			Marshaler.Write(_out_msg, move);
			if (_out_msg.IsError) return false;
			return m_Networker.Send(_out_msg);
		}
		public bool Chat(HostID target, String msg)
		{
			Log.Debug("C2S.Proxy.Chat");
			NetBuffer _out_msg = NetBufferPool.Instance.Pop();
			if (m_Networker == null)
			{
				Log.Debug("{0} is not initialized.", typeof(Proxy));
				return false;
			}
			_out_msg.Init((Int16)RMI_ID.Chat, target);
			Marshaler.Write(_out_msg, msg);
			if (_out_msg.IsError) return false;
			return m_Networker.Send(_out_msg);
		}
	}

}
