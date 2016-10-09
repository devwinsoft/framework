using System;
using Devarc;
namespace Test2C
{
	public interface IStub
	{
		void RMI_Test2C_Notify_Chat(HostID remote, String _name, String _msg);
		void RMI_Test2C_Notify_UnitData(HostID remote, DataCharacter _data);
		void RMI_Test2C_Notify_SendFile_Result(HostID remote, Boolean _success);
	}
	public static class Stub
	{
		public static bool OnReceive(IStub stub, int rid, HostID hid, NetBuffer _in_msg)
		{
			bool success = true;
			RMI_ID rmi_id = (RMI_ID)rid;
			switch (rmi_id)
			{
				case RMI_ID.Notify_Chat:
					{
						Log.Message(LOG_TYPE.DEBUG, "Stub(Test2C): Notify_Chat");
						System.String _name = default(System.String); success = success ? Marshaler.Read(_in_msg, ref _name) : false;
						System.String _msg = default(System.String); success = success ? Marshaler.Read(_in_msg, ref _msg) : false;
						if (_in_msg.Pos != _in_msg.Length) return false;
						if (success) stub.RMI_Test2C_Notify_Chat(hid, _name, _msg);
					}
					break;
				case RMI_ID.Notify_UnitData:
					{
						Log.Message(LOG_TYPE.DEBUG, "Stub(Test2C): Notify_UnitData");
						Devarc.DataCharacter _data = new Devarc.DataCharacter(); success = success ? Marshaler.Read(_in_msg, _data) : false;
						if (_in_msg.Pos != _in_msg.Length) return false;
						if (success) stub.RMI_Test2C_Notify_UnitData(hid, _data);
					}
					break;
				case RMI_ID.Notify_SendFile_Result:
					{
						Log.Message(LOG_TYPE.DEBUG, "Stub(Test2C): Notify_SendFile_Result");
						System.Boolean _success = default(System.Boolean); success = success ? Marshaler.Read(_in_msg, ref _success) : false;
						if (_in_msg.Pos != _in_msg.Length) return false;
						if (success) stub.RMI_Test2C_Notify_SendFile_Result(hid, _success);
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
		Notify_Chat                    = 5000,
		Notify_UnitData                = 5001,
		Notify_SendFile_Result         = 5002,
	}
	public class Proxy
	{
		private INetworker m_Networker = null;
		public void SetNetworker(INetworker mgr) { m_Networker = mgr; }
		public bool Notify_Chat(HostID target, String _name, String _msg)
		{
			Log.Message(LOG_TYPE.DEBUG, "Test2C.Proxy.Notify_Chat");
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Message(LOG_TYPE.DEBUG, typeof(Proxy).ToString() + " is not initialized.");
				return false;
			}
			m_Networker.RmiHeader(m_Networker.GetMyHostID(), target, _out_msg);
			_out_msg.Write((Int32)RMI_ID.Notify_Chat);
			Marshaler.Write(_out_msg, _name);
			Marshaler.Write(_out_msg, _msg);
			return m_Networker.RmiSend(m_Networker.GetMyHostID(), target, _out_msg);
		}
		public bool Notify_UnitData(HostID target, DataCharacter _data)
		{
			Log.Message(LOG_TYPE.DEBUG, "Test2C.Proxy.Notify_UnitData");
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Message(LOG_TYPE.DEBUG, typeof(Proxy).ToString() + " is not initialized.");
				return false;
			}
			m_Networker.RmiHeader(m_Networker.GetMyHostID(), target, _out_msg);
			_out_msg.Write((Int32)RMI_ID.Notify_UnitData);
			Marshaler.Write(_out_msg, _data);
			return m_Networker.RmiSend(m_Networker.GetMyHostID(), target, _out_msg);
		}
		public bool Notify_SendFile_Result(HostID target, Boolean _success)
		{
			Log.Message(LOG_TYPE.DEBUG, "Test2C.Proxy.Notify_SendFile_Result");
			NetBuffer _out_msg = new NetBuffer();
			if (m_Networker == null)
			{
				Log.Message(LOG_TYPE.DEBUG, typeof(Proxy).ToString() + " is not initialized.");
				return false;
			}
			m_Networker.RmiHeader(m_Networker.GetMyHostID(), target, _out_msg);
			_out_msg.Write((Int32)RMI_ID.Notify_SendFile_Result);
			Marshaler.Write(_out_msg, _success);
			return m_Networker.RmiSend(m_Networker.GetMyHostID(), target, _out_msg);
		}
	}

}
