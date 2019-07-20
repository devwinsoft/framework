using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Devarc;
namespace Devarc.S2C
{
	public interface IStub
	{
		void RMI_S2C_Notify_Player(HostID remote, Notify_Player msg);
		void RMI_S2C_Notify_Move(HostID remote, Notify_Move msg);
		void RMI_S2C_Notify_Chat(HostID remote, Notify_Chat msg);
	}
	public static class Stub
	{
		public static RECEIVE_RESULT OnReceive(IStub stub, NetBuffer _in_msg)
		{
			RMI_ID rmi_id = (RMI_ID)_in_msg.Rmi;
			switch (rmi_id)
			{
				case RMI_ID.Notify_Player:
					try
					{
						Log.Debug("S2C.Stub.Notify_Player");
						Notify_Player msg = new Notify_Player();
						Marshaler.Read(_in_msg, ref msg.id);
						Marshaler.Read(_in_msg, msg.data);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET_DOWNFLOW;
						stub.RMI_S2C_Notify_Player(_in_msg.Hid, msg);
					}
					catch (NetException ex)
					{
						return ex.ERROR;
					}
					break;
				case RMI_ID.Notify_Move:
					try
					{
						Log.Debug("S2C.Stub.Notify_Move");
						Notify_Move msg = new Notify_Move();
						Marshaler.Read(_in_msg, msg.look);
						Marshaler.Read(_in_msg, ref msg.move);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET_DOWNFLOW;
						stub.RMI_S2C_Notify_Move(_in_msg.Hid, msg);
					}
					catch (NetException ex)
					{
						return ex.ERROR;
					}
					break;
				case RMI_ID.Notify_Chat:
					try
					{
						Log.Debug("S2C.Stub.Notify_Chat");
						Notify_Chat msg = new Notify_Chat();
						Marshaler.Read(_in_msg, ref msg._msg);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET_DOWNFLOW;
						stub.RMI_S2C_Notify_Chat(_in_msg.Hid, msg);
					}
					catch (NetException ex)
					{
						return ex.ERROR;
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
		NUMBER                         = 1,
	}
	enum RMI_ID
	{
		Notify_Player                  = 11010,
		Notify_Move                    = 11020,
		Notify_Chat                    = 11030,
	}
	public class Proxy : ProxyBase
	{
		public SEND_RESULT Send(NetBuffer msg)
		{
			if (mNetworker == null)
			{
				Log.Debug("{0} is not initialized.", typeof(Proxy));
				return SEND_RESULT.NOT_INITIALIZED;
			}
			return mNetworker.Send(msg);
		}
		public SEND_RESULT Notify_Player(HostID target, HostID id, DataPlayer data)
		{
			Log.Debug("S2C.Proxy.Notify_Player");
			NetBuffer _out_msg = NetBufferPool.Instance.Pop();
			_out_msg.Init((Int16)RMI_ID.Notify_Player, target);
			Marshaler.Write(_out_msg, id);
			Marshaler.Write(_out_msg, data);
			return Send(_out_msg);
		}
		public SEND_RESULT Notify_Move(HostID target, VECTOR3 look, DIRECTION move)
		{
			Log.Debug("S2C.Proxy.Notify_Move");
			NetBuffer _out_msg = NetBufferPool.Instance.Pop();
			_out_msg.Init((Int16)RMI_ID.Notify_Move, target);
			Marshaler.Write(_out_msg, look);
			Marshaler.Write(_out_msg, move);
			return Send(_out_msg);
		}
		public SEND_RESULT Notify_Chat(HostID target, String _msg)
		{
			Log.Debug("S2C.Proxy.Notify_Chat");
			NetBuffer _out_msg = NetBufferPool.Instance.Pop();
			_out_msg.Init((Int16)RMI_ID.Notify_Chat, target);
			Marshaler.Write(_out_msg, _msg);
			return Send(_out_msg);
		}
	}
}
namespace Devarc.S2C
{
	[System.Serializable]
	[NetProtocolAttribute(RMI_ID = 11010)]
	public class Notify_Player : IBasePacket
	{
		public short RMI_ID { get { return 11010; } }
		public bool WriteTo(NetBuffer _obj)
		{
			Marshaler.Write(_obj, id);
			Marshaler.Write(_obj, data);
			return true;
		}
		public string ToMessage(int _userSEQ, string _userKey)
		{
			string contents = string.Format("{{\"user_seq\":\"{0}\",\"user_key\":\"{1}\",\"rmi_id\":\"{2}\",\"time\":\"{3}\"}}", _userSEQ, _userKey, RMI_ID, DateTime.Now.Millisecond);
			StringBuilder sb = new StringBuilder();
			sb.Append("header = ");
			sb.Append(FrameworkUtil.ToBase64String(Cryptor.Encrypt(contents)));
			sb.Append("&body=");
			string encData = Cryptor.Encrypt(ToJson());
			sb.Append(FrameworkUtil.ToBase64String(encData));
			return sb.ToString();
		}
		public HostID              id;
		public DataPlayer          data = new DataPlayer();

		public Notify_Player()
		{
		}
		public Notify_Player(Notify_Player obj)
		{
			Initialize(obj);
		}
		public Notify_Player(PropTable obj)
		{
			Initialize(obj);
		}
		public bool IsDefault
		{
			get
			{
				if (id != 0) return false;
				if (data.IsDefault == false) return false;
				return true;
			}
		}
		public void Initialize(IBaseObejct from)
		{
			Notify_Player obj = from as Notify_Player;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:Notify_Player");
				return;
			}
			id                  = obj.id;
			data.Initialize(obj.data);
		}
		public void Initialize(PropTable obj)
		{
			id                  = obj.GetHostID("id");
			data.Initialize(obj.GetTable("data"));
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("id")) HostID.TryParse(obj["id"].ToString(), out id); else id = default(HostID);
			if (obj.Keys.Contains("data")) data.Initialize(obj["data"]);
		}
		public void Initialize(IBaseReader obj)
		{
			id                  = obj.GetInt16("id");
			data.Initialize(JsonMapper.ToObject(obj.GetString("data")));
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"id\":"); sb.Append("\""); sb.Append(id.ToString()); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"data\":"); sb.Append(data.ToString());
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    if (IsDefault) { return "{}"; }
		    StringBuilder sb = new StringBuilder();
		    int j = 0;
			sb.Append("{");
			if (default(HostID) != id) { if (j > 0) { sb.Append(", "); } j++;
			 sb.Append("\"id\":"); sb.Append(string.Format("\"{0}\"", id.ToString())); }
			if (data.IsDefault == false) { if (j > 0) { sb.Append(", "); } j++;
			 sb.Append("\"data\":"); sb.Append(data.ToJson()); }
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("Notify_Player");
			obj.Attach("id", "HostID", CLASS_TYPE.VALUE, false, id.ToString());
			obj.Attach_Class("data", "DataPlayer", data.ToTable());
			return obj;
		}
	}
	[System.Serializable]
	[NetProtocolAttribute(RMI_ID = 11020)]
	public class Notify_Move : IBasePacket
	{
		public short RMI_ID { get { return 11020; } }
		public bool WriteTo(NetBuffer _obj)
		{
			Marshaler.Write(_obj, look);
			Marshaler.Write(_obj, move);
			return true;
		}
		public string ToMessage(int _userSEQ, string _userKey)
		{
			string contents = string.Format("{{\"user_seq\":\"{0}\",\"user_key\":\"{1}\",\"rmi_id\":\"{2}\",\"time\":\"{3}\"}}", _userSEQ, _userKey, RMI_ID, DateTime.Now.Millisecond);
			StringBuilder sb = new StringBuilder();
			sb.Append("header = ");
			sb.Append(FrameworkUtil.ToBase64String(Cryptor.Encrypt(contents)));
			sb.Append("&body=");
			string encData = Cryptor.Encrypt(ToJson());
			sb.Append(FrameworkUtil.ToBase64String(encData));
			return sb.ToString();
		}
		public VECTOR3             look = new VECTOR3();
		public DIRECTION           move;

		public Notify_Move()
		{
		}
		public Notify_Move(Notify_Move obj)
		{
			Initialize(obj);
		}
		public Notify_Move(PropTable obj)
		{
			Initialize(obj);
		}
		public bool IsDefault
		{
			get
			{
				if (look.IsDefault == false) return false;
				if ((int)move != 0) return false;
				return true;
			}
		}
		public void Initialize(IBaseObejct from)
		{
			Notify_Move obj = from as Notify_Move;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:Notify_Move");
				return;
			}
			look.Initialize(obj.look);
			move                = obj.move;
		}
		public void Initialize(PropTable obj)
		{
			look.Initialize(obj.GetTable("look"));
			move                = FrameworkUtil.Parse<DIRECTION>(obj.GetStr("move"));
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("look")) look.Initialize(obj["look"]);
			if (obj.Keys.Contains("move")) move = FrameworkUtil.Parse<DIRECTION>(obj["move"].ToString()); else move = default(DIRECTION);
		}
		public void Initialize(IBaseReader obj)
		{
			look.Initialize(JsonMapper.ToObject(obj.GetString("look")));
			move                = FrameworkUtil.Parse<DIRECTION>(obj.GetString("move"));
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"look\":"); sb.Append(look.ToString());
		    sb.Append(","); sb.Append(" \"move\":"); sb.Append("\""); sb.Append(move.ToString()); sb.Append("\"");
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    if (IsDefault) { return "{}"; }
		    StringBuilder sb = new StringBuilder();
		    int j = 0;
			sb.Append("{");
			if (look.IsDefault == false) { if (j > 0) { sb.Append(", "); } j++;
			 sb.Append("\"look\":"); sb.Append(look.ToJson()); }
			if (default(DIRECTION) != move) { if (j > 0) { sb.Append(", "); } j++;
			 sb.Append("\"move\":"); sb.Append(string.Format("\"{0}\"", move.ToString())); }
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("Notify_Move");
			obj.Attach_Class("look", "VECTOR3", look.ToTable());
			obj.Attach("move", "DIRECTION", CLASS_TYPE.VALUE, false, move.ToString());
			return obj;
		}
	}
	[System.Serializable]
	[NetProtocolAttribute(RMI_ID = 11030)]
	public class Notify_Chat : IBasePacket
	{
		public short RMI_ID { get { return 11030; } }
		public bool WriteTo(NetBuffer _obj)
		{
			Marshaler.Write(_obj, _msg);
			return true;
		}
		public string ToMessage(int _userSEQ, string _userKey)
		{
			string contents = string.Format("{{\"user_seq\":\"{0}\",\"user_key\":\"{1}\",\"rmi_id\":\"{2}\",\"time\":\"{3}\"}}", _userSEQ, _userKey, RMI_ID, DateTime.Now.Millisecond);
			StringBuilder sb = new StringBuilder();
			sb.Append("header = ");
			sb.Append(FrameworkUtil.ToBase64String(Cryptor.Encrypt(contents)));
			sb.Append("&body=");
			string encData = Cryptor.Encrypt(ToJson());
			sb.Append(FrameworkUtil.ToBase64String(encData));
			return sb.ToString();
		}
		public string              _msg = "";

		public Notify_Chat()
		{
		}
		public Notify_Chat(Notify_Chat obj)
		{
			Initialize(obj);
		}
		public Notify_Chat(PropTable obj)
		{
			Initialize(obj);
		}
		public bool IsDefault
		{
			get
			{
				if (string.IsNullOrEmpty(_msg) == false) return false;
				return true;
			}
		}
		public void Initialize(IBaseObejct from)
		{
			Notify_Chat obj = from as Notify_Chat;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:Notify_Chat");
				return;
			}
			_msg                = obj._msg;
		}
		public void Initialize(PropTable obj)
		{
			_msg                = obj.GetStr("_msg");
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("_msg")) _msg = obj["_msg"].ToString(); else _msg = default(string);
		}
		public void Initialize(IBaseReader obj)
		{
			_msg                = obj.GetString("_msg");
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"_msg\":"); sb.Append("\""); sb.Append(_msg); sb.Append("\"");
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    if (IsDefault) { return "{}"; }
		    StringBuilder sb = new StringBuilder();
		    int j = 0;
			sb.Append("{");
			if (string.IsNullOrEmpty(_msg) == false) { if (j > 0) { sb.Append(", "); } j++;
			 sb.Append("\"_msg\":"); sb.Append("\""); sb.Append(FrameworkUtil.JsonString(_msg)); sb.Append("\""); }
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("Notify_Chat");
			obj.Attach("_msg", "string", CLASS_TYPE.VALUE, false, _msg);
			return obj;
		}
	}
}
