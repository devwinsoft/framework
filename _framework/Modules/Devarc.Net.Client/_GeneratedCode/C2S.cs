using System;
using Devarc;
namespace C2S
{
	public interface IStub
	{
		void RMI_C2S_Move(HostID remote, VECTOR3 _look, DIRECTION _move);
		void RMI_C2S_Chat(HostID remote, String _msg);
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
						Log.Debug("Stub(C2S): Move");
						Devarc.VECTOR3 _look = new Devarc.VECTOR3(); Marshaler.Read(_in_msg, _look);
						Devarc.DIRECTION _move = default(Devarc.DIRECTION); Marshaler.Read(_in_msg, ref _move);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET;
						stub.RMI_C2S_Move(_in_msg.Hid, _look, _move);
					}
					break;
				case RMI_ID.Chat:
					{
						Log.Debug("Stub(C2S): Chat");
						System.String _msg = default(System.String); Marshaler.Read(_in_msg, ref _msg);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET;
						stub.RMI_C2S_Chat(_in_msg.Hid, _msg);
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
		Move                           = 6000,
		Chat                           = 6001,
	}
	public class Proxy
	{
		private IProxyBase m_Networker = null;
		public void Init(IProxyBase mgr) { m_Networker = mgr; }
		public bool Move(HostID target, VECTOR3 _look, DIRECTION _move)
		{
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Debug("{{0}} is not initialized.", typeof(Proxy));
				return false;
			}
			_out_msg.Init((Int16)RMI_ID.Move, target);
			Marshaler.Write(_out_msg, _look);
			Marshaler.Write(_out_msg, _move);
			if (_out_msg.IsError) return false;
			return m_Networker.Send(_out_msg);
		}
		public bool Chat(HostID target, String _msg)
		{
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Debug("{{0}} is not initialized.", typeof(Proxy));
				return false;
			}
			_out_msg.Init((Int16)RMI_ID.Chat, target);
			Marshaler.Write(_out_msg, _msg);
			if (_out_msg.IsError) return false;
			return m_Networker.Send(_out_msg);
		}
	}

}
