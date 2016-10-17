using System;
using Devarc;
namespace C2S
{
	public interface IStub
	{
		void RMI_C2S_Chat(HostID remote, String _msg);
		void RMI_C2S_Move(HostID remote, VECTOR3 _look, DIRECTION _move);
	}
	public static class Stub
	{
		public static bool OnReceive(IStub stub, int rid, HostID hid, NetBuffer _in_msg)
		{
			bool success = true;
			RMI_ID rmi_id = (RMI_ID)rid;
			switch (rmi_id)
			{
				case RMI_ID.Chat:
					{
						Log.Message(LOG_TYPE.DEBUG, "Stub(C2S): Chat");
						System.String _msg = default(System.String); success = success ? Marshaler.Read(_in_msg, ref _msg) : false;
						if (_in_msg.Pos != _in_msg.Length) return false;
						if (success) stub.RMI_C2S_Chat(hid, _msg);
					}
					break;
				case RMI_ID.Move:
					{
						Log.Message(LOG_TYPE.DEBUG, "Stub(C2S): Move");
						Devarc.VECTOR3 _look = new Devarc.VECTOR3(); success = success ? Marshaler.Read(_in_msg, _look) : false;
						Devarc.DIRECTION _move = default(Devarc.DIRECTION); success = success ? Marshaler.Read(_in_msg, ref _move) : false;
						if (_in_msg.Pos != _in_msg.Length) return false;
						if (success) stub.RMI_C2S_Move(hid, _look, _move);
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
		Move                           = 6001,
	}
	public class Proxy
	{
		private INetworker m_Networker = null;
		public void SetNetworker(INetworker mgr) { m_Networker = mgr; }
		public bool Chat(HostID target, String _msg)
		{
			Log.Message(LOG_TYPE.DEBUG, "C2S.Proxy.Chat");
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Message(LOG_TYPE.DEBUG, typeof(Proxy).ToString() + " is not initialized.");
				return false;
			}
			m_Networker.RmiHeader(m_Networker.GetMyHostID(), target, _out_msg);
			_out_msg.Write((Int32)RMI_ID.Chat);
			Marshaler.Write(_out_msg, _msg);
			return m_Networker.RmiSend(m_Networker.GetMyHostID(), target, _out_msg);
		}
		public bool Move(HostID target, VECTOR3 _look, DIRECTION _move)
		{
			Log.Message(LOG_TYPE.DEBUG, "C2S.Proxy.Move");
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Message(LOG_TYPE.DEBUG, typeof(Proxy).ToString() + " is not initialized.");
				return false;
			}
			m_Networker.RmiHeader(m_Networker.GetMyHostID(), target, _out_msg);
			_out_msg.Write((Int32)RMI_ID.Move);
			Marshaler.Write(_out_msg, _look);
			Marshaler.Write(_out_msg, _move);
			return m_Networker.RmiSend(m_Networker.GetMyHostID(), target, _out_msg);
		}
	}

}
