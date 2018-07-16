using System;
using Devarc;
namespace S2C
{
	namespace MSG
	{
		public class Notify_Player
		{
			public HostID id;
			public DataPlayer data = new DataPlayer();
		}
		public class Notify_Move
		{
			public VECTOR3 look = new VECTOR3();
			public DIRECTION move;
		}
		public class Notify_Chat
		{
			public String _msg;
		}
	}
	public interface IStub
	{
		void RMI_S2C_Notify_Player(HostID remote, S2C.MSG.Notify_Player msg);
		void RMI_S2C_Notify_Move(HostID remote, S2C.MSG.Notify_Move msg);
		void RMI_S2C_Notify_Chat(HostID remote, S2C.MSG.Notify_Chat msg);
	}
	public static class Stub
	{
		public static RECEIVE_RESULT OnReceive(IStub stub, NetBuffer _in_msg)
		{
			RMI_ID rmi_id = (RMI_ID)_in_msg.Rmi;
			switch (rmi_id)
			{
				case RMI_ID.Notify_Player:
					{
						Log.Debug("S2C.Stub.Notify_Player");
						MSG.Notify_Player msg = new MSG.Notify_Player();
						Marshaler.Read(_in_msg, ref msg.id);
						Marshaler.Read(_in_msg, msg.data);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET;
						stub.RMI_S2C_Notify_Player(_in_msg.Hid, msg);
					}
					break;
				case RMI_ID.Notify_Move:
					{
						Log.Debug("S2C.Stub.Notify_Move");
						MSG.Notify_Move msg = new MSG.Notify_Move();
						Marshaler.Read(_in_msg, msg.look);
						Marshaler.Read(_in_msg, ref msg.move);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET;
						stub.RMI_S2C_Notify_Move(_in_msg.Hid, msg);
					}
					break;
				case RMI_ID.Notify_Chat:
					{
						Log.Debug("S2C.Stub.Notify_Chat");
						MSG.Notify_Chat msg = new MSG.Notify_Chat();
						Marshaler.Read(_in_msg, ref msg._msg);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET;
						stub.RMI_S2C_Notify_Chat(_in_msg.Hid, msg);
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
		Notify_Player                  = 2,
		Notify_Move                    = 3,
		Notify_Chat                    = 4,
	}
	public class Proxy : IProxyBase
	{
		private INetworker m_Networker = null;
		public void Init(INetworker mgr) { m_Networker = mgr; }
		public bool Notify_Player(HostID target, HostID id, DataPlayer data)
		{
			Log.Debug("S2C.Proxy.Notify_Player");
			NetBuffer _out_msg = NetBufferPool.Instance.Pop();
			if (m_Networker == null)
			{
				Log.Debug("{0} is not initialized.", typeof(Proxy));
				return false;
			}
			_out_msg.Init((Int16)RMI_ID.Notify_Player, target);
			Marshaler.Write(_out_msg, id);
			Marshaler.Write(_out_msg, data);
			if (_out_msg.IsError) return false;
			return m_Networker.Send(_out_msg);
		}
		public bool Notify_Move(HostID target, VECTOR3 look, DIRECTION move)
		{
			Log.Debug("S2C.Proxy.Notify_Move");
			NetBuffer _out_msg = NetBufferPool.Instance.Pop();
			if (m_Networker == null)
			{
				Log.Debug("{0} is not initialized.", typeof(Proxy));
				return false;
			}
			_out_msg.Init((Int16)RMI_ID.Notify_Move, target);
			Marshaler.Write(_out_msg, look);
			Marshaler.Write(_out_msg, move);
			if (_out_msg.IsError) return false;
			return m_Networker.Send(_out_msg);
		}
		public bool Notify_Chat(HostID target, String _msg)
		{
			Log.Debug("S2C.Proxy.Notify_Chat");
			NetBuffer _out_msg = NetBufferPool.Instance.Pop();
			if (m_Networker == null)
			{
				Log.Debug("{0} is not initialized.", typeof(Proxy));
				return false;
			}
			_out_msg.Init((Int16)RMI_ID.Notify_Chat, target);
			Marshaler.Write(_out_msg, _msg);
			if (_out_msg.IsError) return false;
			return m_Networker.Send(_out_msg);
		}
	}

}
