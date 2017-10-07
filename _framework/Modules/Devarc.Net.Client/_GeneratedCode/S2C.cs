using System;
using Devarc;
namespace S2C
{
	public interface IStub
	{
		void RMI_S2C_Notify_Player(HostID remote, HostID _id, DataPlayer _data);
		void RMI_S2C_Notify_Move(HostID remote, VECTOR3 _look, DIRECTION _move);
		void RMI_S2C_Notify_Chat(HostID remote, String _msg);
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
						Log.Debug("Stub(S2C): Notify_Player");
						Devarc.HostID _id = default(Devarc.HostID); Marshaler.Read(_in_msg, ref _id);
						Devarc.DataPlayer _data = new Devarc.DataPlayer(); Marshaler.Read(_in_msg, _data);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET;
						stub.RMI_S2C_Notify_Player(_in_msg.Hid, _id, _data);
					}
					break;
				case RMI_ID.Notify_Move:
					{
						Log.Debug("Stub(S2C): Notify_Move");
						Devarc.VECTOR3 _look = new Devarc.VECTOR3(); Marshaler.Read(_in_msg, _look);
						Devarc.DIRECTION _move = default(Devarc.DIRECTION); Marshaler.Read(_in_msg, ref _move);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET;
						stub.RMI_S2C_Notify_Move(_in_msg.Hid, _look, _move);
					}
					break;
				case RMI_ID.Notify_Chat:
					{
						Log.Debug("Stub(S2C): Notify_Chat");
						System.String _msg = default(System.String); Marshaler.Read(_in_msg, ref _msg);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET;
						stub.RMI_S2C_Notify_Chat(_in_msg.Hid, _msg);
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
		Notify_Player                  = 5000,
		Notify_Move                    = 5001,
		Notify_Chat                    = 5002,
	}
	public class Proxy
	{
		private IProxyBase m_Networker = null;
		public void Init(IProxyBase mgr) { m_Networker = mgr; }
		public bool Notify_Player(HostID target, HostID _id, DataPlayer _data)
		{
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Debug("{{0}} is not initialized.", typeof(Proxy));
				return false;
			}
			_out_msg.Init((Int16)RMI_ID.Notify_Player, target);
			Marshaler.Write(_out_msg, _id);
			Marshaler.Write(_out_msg, _data);
			if (_out_msg.IsError) return false;
			return m_Networker.Send(_out_msg);
		}
		public bool Notify_Move(HostID target, VECTOR3 _look, DIRECTION _move)
		{
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Debug("{{0}} is not initialized.", typeof(Proxy));
				return false;
			}
			_out_msg.Init((Int16)RMI_ID.Notify_Move, target);
			Marshaler.Write(_out_msg, _look);
			Marshaler.Write(_out_msg, _move);
			if (_out_msg.IsError) return false;
			return m_Networker.Send(_out_msg);
		}
		public bool Notify_Chat(HostID target, String _msg)
		{
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Debug("{{0}} is not initialized.", typeof(Proxy));
				return false;
			}
			_out_msg.Init((Int16)RMI_ID.Notify_Chat, target);
			Marshaler.Write(_out_msg, _msg);
			if (_out_msg.IsError) return false;
			return m_Networker.Send(_out_msg);
		}
	}

}
