using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Devarc;
namespace Devarc.C2S
{
	public interface IStub
	{
		void RMI_C2S_Request_Move(HostID remote, Request_Move msg);
		void RMI_C2S_Request_Chat(HostID remote, Request_Chat msg);
	}
	public static class Stub
	{
		public static RECEIVE_RESULT OnReceive(IStub stub, NetBuffer _in_msg)
		{
			RMI_ID rmi_id = (RMI_ID)_in_msg.Rmi;
			switch (rmi_id)
			{
				case RMI_ID.Request_Move:
					try
					{
						Log.Debug("C2S.Stub.Request_Move");
						Request_Move msg = new Request_Move();
						Marshaler.Read(_in_msg, msg.look);
						Marshaler.Read(_in_msg, ref msg.move);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET_DOWNFLOW;
						stub.RMI_C2S_Request_Move(_in_msg.Hid, msg);
					}
					catch (NetException ex)
					{
						return ex.ERROR;
					}
					break;
				case RMI_ID.Request_Chat:
					try
					{
						Log.Debug("C2S.Stub.Request_Chat");
						Request_Chat msg = new Request_Chat();
						Marshaler.Read(_in_msg, ref msg.msg);
						Marshaler.Read(_in_msg, out msg.data);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET_DOWNFLOW;
						stub.RMI_C2S_Request_Chat(_in_msg.Hid, msg);
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
	}
	enum RMI_ID
	{
		Request_Move                   = 10010,
		Request_Chat                   = 10020,
	}
	public class Proxy : ProxyBase
	{
		public bool Send(NetBuffer msg)
		{
			if (mNetworker == null)
			{
				Log.Debug("{0} is not initialized.", typeof(Proxy));
				return false;
			}
			if (msg.IsError) return false;
			return mNetworker.Send(msg);
		}
		public bool Request_Move(HostID target, VECTOR3 look, DIRECTION move)
		{
			Log.Debug("C2S.Proxy.Request_Move");
			NetBuffer _out_msg = NetBufferPool.Instance.Pop();
			_out_msg.Init((Int16)RMI_ID.Request_Move, target);
			Marshaler.Write(_out_msg, look);
			Marshaler.Write(_out_msg, move);
			return Send(_out_msg);
		}
		public bool Request_Chat(HostID target, String msg, Byte[] data)
		{
			Log.Debug("C2S.Proxy.Request_Chat");
			NetBuffer _out_msg = NetBufferPool.Instance.Pop();
			_out_msg.Init((Int16)RMI_ID.Request_Chat, target);
			Marshaler.Write(_out_msg, msg);
			Marshaler.Write(_out_msg, data);
			return Send(_out_msg);
		}
	}
}
namespace Devarc.C2S
{
	[System.Serializable]
	[NetProtocolAttribute(RMI_ID = 10010)]
	public class Request_Move : IBasePacket
	{
		public short RMI_ID { get { return 10010; } }
		public bool WriteTo(NetBuffer _obj)
		{
			Marshaler.Write(_obj, look);
			Marshaler.Write(_obj, move);
			return true;
		}
		public VECTOR3             look = new VECTOR3();
		public DIRECTION           move;

		public Request_Move()
		{
		}
		public Request_Move(Request_Move obj)
		{
			Initialize(obj);
		}
		public Request_Move(PropTable obj)
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
			Request_Move obj = from as Request_Move;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:Request_Move");
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
			PropTable obj = new PropTable("Request_Move");
			obj.Attach_Class("look", "VECTOR3", look.ToTable());
			obj.Attach("move", "DIRECTION", CLASS_TYPE.VALUE, false, move.ToString());
			return obj;
		}
	}
	[System.Serializable]
	[NetProtocolAttribute(RMI_ID = 10020)]
	public class Request_Chat : IBasePacket
	{
		public short RMI_ID { get { return 10020; } }
		public bool WriteTo(NetBuffer _obj)
		{
			Marshaler.Write(_obj, msg);
			Marshaler.Write(_obj, data);
			return true;
		}
		public string              msg = "";
		public byte[]              data = null;

		public Request_Chat()
		{
		}
		public Request_Chat(Request_Chat obj)
		{
			Initialize(obj);
		}
		public Request_Chat(PropTable obj)
		{
			Initialize(obj);
		}
		public bool IsDefault
		{
			get
			{
				if (string.IsNullOrEmpty(msg) == false) return false;
				if (data != null && data.Length > 0) return false;
				return true;
			}
		}
		public void Initialize(IBaseObejct from)
		{
			Request_Chat obj = from as Request_Chat;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:Request_Chat");
				return;
			}
			msg                 = obj.msg;
			data = new byte[obj.data.Length];
			Array.Copy(obj.data, data, data.Length);
		}
		public void Initialize(PropTable obj)
		{
			msg                 = obj.GetStr("msg");
			data                = obj.GetBytes("data");
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("msg")) msg = obj["msg"].ToString(); else msg = default(string);
			data = Convert.FromBase64String(obj["data"].ToString());
		}
		public void Initialize(IBaseReader obj)
		{
			msg                 = obj.GetString("msg");
			data = Convert.FromBase64String(obj.GetString("data"));
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"msg\":"); sb.Append("\""); sb.Append(msg); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"data\":"); sb.Append("\""); sb.Append(FrameworkUtil.ToBase64String(data)); sb.Append("\"");
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    if (IsDefault) { return "{}"; }
		    StringBuilder sb = new StringBuilder();
		    int j = 0;
			sb.Append("{");
			if (string.IsNullOrEmpty(msg) == false) { if (j > 0) { sb.Append(", "); } j++;
			 sb.Append("\"msg\":"); sb.Append("\""); sb.Append(FrameworkUtil.JsonString(msg)); sb.Append("\""); }
			if (data != null && data.Length > 0) { if (j > 0) { sb.Append(", "); } j++;
			 sb.Append("\"data\":"); sb.Append(string.Format("\"{0}\"", FrameworkUtil.ToBase64String(data))); }
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("Request_Chat");
			obj.Attach("msg", "string", CLASS_TYPE.VALUE, false, msg);
			obj.Attach("data", "byte[]", CLASS_TYPE.VALUE, false, FrameworkUtil.ToBase64String(data));
			return obj;
		}
	}
}
