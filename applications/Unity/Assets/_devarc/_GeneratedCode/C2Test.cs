using System;
using Devarc;
namespace C2Test
{
	public interface IStub
	{
		void RMI_C2Test_Chat(HostID remote, String _name, String _msg);
		void RMI_C2Test_Request_UnitData(HostID remote, UNIT _type);
		void RMI_C2Test_SendFile(HostID remote, String _file_name, Byte[] _data);
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
						Log.Message(LOG_TYPE.DEBUG, "Stub(C2Test): Chat");
						System.String _name = default(System.String); success = success ? Marshaler.Read(_in_msg, ref _name) : false;
						System.String _msg = default(System.String); success = success ? Marshaler.Read(_in_msg, ref _msg) : false;
						if (_in_msg.Pos != _in_msg.Length) return false;
						if (success) stub.RMI_C2Test_Chat(hid, _name, _msg);
					}
					break;
				case RMI_ID.Request_UnitData:
					{
						Log.Message(LOG_TYPE.DEBUG, "Stub(C2Test): Request_UnitData");
						Devarc.UNIT _type = default(Devarc.UNIT); success = success ? Marshaler.Read(_in_msg, ref _type) : false;
						if (_in_msg.Pos != _in_msg.Length) return false;
						if (success) stub.RMI_C2Test_Request_UnitData(hid, _type);
					}
					break;
				case RMI_ID.SendFile:
					{
						Log.Message(LOG_TYPE.DEBUG, "Stub(C2Test): SendFile");
						System.String _file_name = default(System.String); success = success ? Marshaler.Read(_in_msg, ref _file_name) : false;
						System.Byte[] _data; _data = success ? null : new System.Byte[0]; success = success ? Marshaler.Read(_in_msg, out _data) : false;
						if (_in_msg.Pos != _in_msg.Length) return false;
						if (success) stub.RMI_C2Test_SendFile(hid, _file_name, _data);
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
		Request_UnitData               = 6001,
		SendFile                       = 6002,
	}
	public class Proxy
	{
		private INetworker m_Networker = null;
		public void SetNetworker(INetworker mgr) { m_Networker = mgr; }
		public bool Chat(HostID target, String _name, String _msg)
		{
			Log.Message(LOG_TYPE.DEBUG, "C2Test.Proxy.Chat");
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Message(LOG_TYPE.DEBUG, typeof(Proxy).ToString() + " is not initialized.");
				return false;
			}
			m_Networker.RmiHeader(m_Networker.GetMyHostID(), target, _out_msg);
			_out_msg.Write((Int32)RMI_ID.Chat);
			Marshaler.Write(_out_msg, _name);
			Marshaler.Write(_out_msg, _msg);
			return m_Networker.RmiSend(m_Networker.GetMyHostID(), target, _out_msg);
		}
		public bool Request_UnitData(HostID target, UNIT _type)
		{
			Log.Message(LOG_TYPE.DEBUG, "C2Test.Proxy.Request_UnitData");
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Message(LOG_TYPE.DEBUG, typeof(Proxy).ToString() + " is not initialized.");
				return false;
			}
			m_Networker.RmiHeader(m_Networker.GetMyHostID(), target, _out_msg);
			_out_msg.Write((Int32)RMI_ID.Request_UnitData);
			Marshaler.Write(_out_msg, _type);
			return m_Networker.RmiSend(m_Networker.GetMyHostID(), target, _out_msg);
		}
		public bool SendFile(HostID target, String _file_name, Byte[] _data)
		{
			Log.Message(LOG_TYPE.DEBUG, "C2Test.Proxy.SendFile");
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Message(LOG_TYPE.DEBUG, typeof(Proxy).ToString() + " is not initialized.");
				return false;
			}
			m_Networker.RmiHeader(m_Networker.GetMyHostID(), target, _out_msg);
			_out_msg.Write((Int32)RMI_ID.SendFile);
			Marshaler.Write(_out_msg, _file_name);
			Marshaler.Write(_out_msg, _data);
			return m_Networker.RmiSend(m_Networker.GetMyHostID(), target, _out_msg);
		}
	}

}
