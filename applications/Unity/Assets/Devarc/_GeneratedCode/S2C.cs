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
		public static bool OnReceive(IStub stub, int rid, HostID hid, NetBuffer _in_msg)
		{
			bool success = true;
			RMI_ID rmi_id = (RMI_ID)rid;
			switch (rmi_id)
			{
				case RMI_ID.Notify_Player:
					{
						Log.Debug("Stub(S2C): Notify_Player");
						Devarc.HostID _id = default(Devarc.HostID); success = success ? Marshaler.Read(_in_msg, ref _id) : false;
						Devarc.DataPlayer _data = new Devarc.DataPlayer(); success = success ? Marshaler.Read(_in_msg, _data) : false;
						if (_in_msg.Pos != _in_msg.Length) return false;
						if (success) stub.RMI_S2C_Notify_Player(hid, _id, _data);
					}
					break;
				case RMI_ID.Notify_Move:
					{
						Log.Debug("Stub(S2C): Notify_Move");
						Devarc.VECTOR3 _look = new Devarc.VECTOR3(); success = success ? Marshaler.Read(_in_msg, _look) : false;
						Devarc.DIRECTION _move = default(Devarc.DIRECTION); success = success ? Marshaler.Read(_in_msg, ref _move) : false;
						if (_in_msg.Pos != _in_msg.Length) return false;
						if (success) stub.RMI_S2C_Notify_Move(hid, _look, _move);
					}
					break;
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
		Notify_Player                  = 5000,
		Notify_Move                    = 5001,
		Notify_Chat                    = 5002,
	}
	public class Proxy
	{
		private INetworker m_Networker = null;
		public void SetNetworker(INetworker mgr) { m_Networker = mgr; }
		public bool Notify_Player(HostID target, HostID _id, DataPlayer _data)
		{
			Log.Debug("S2C.Proxy.Notify_Player");
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Debug(typeof(Proxy).ToString() + " is not initialized.");
				return false;
			}
			m_Networker.RmiHeader(m_Networker.GetMyHostID(), target, _out_msg);
			_out_msg.Write((Int32)RMI_ID.Notify_Player);
			Marshaler.Write(_out_msg, _id);
			Marshaler.Write(_out_msg, _data);
			return m_Networker.RmiSend(m_Networker.GetMyHostID(), target, _out_msg);
		}
		public bool Notify_Move(HostID target, VECTOR3 _look, DIRECTION _move)
		{
			Log.Debug("S2C.Proxy.Notify_Move");
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Debug(typeof(Proxy).ToString() + " is not initialized.");
				return false;
			}
			m_Networker.RmiHeader(m_Networker.GetMyHostID(), target, _out_msg);
			_out_msg.Write((Int32)RMI_ID.Notify_Move);
			Marshaler.Write(_out_msg, _look);
			Marshaler.Write(_out_msg, _move);
			return m_Networker.RmiSend(m_Networker.GetMyHostID(), target, _out_msg);
		}
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
