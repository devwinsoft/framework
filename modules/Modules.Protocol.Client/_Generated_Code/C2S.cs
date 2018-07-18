using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Devarc;
namespace C2S
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
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET;
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
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET;
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
		RMI_VERSION                    = 1,
	}
	enum RMI_ID
	{
		Request_Move                   = 6000,
		Request_Chat                   = 6001,
	}
	public class Proxy : IProxyBase
	{
		private INetworker m_Networker = null;
		public void Init(INetworker mgr) { m_Networker = mgr; }
		public bool Request_Move(HostID target, VECTOR3 look, DIRECTION move)
		{
			if (m_Networker == null)
			{
				Log.Debug("{0} is not initialized.", typeof(Proxy));
				return false;
			}
			Log.Debug("C2S.Proxy.Request_Move");
			NetBuffer _out_msg = NetBufferPool.Instance.Pop();
			_out_msg.Init((Int16)RMI_ID.Request_Move, target);
			Marshaler.Write(_out_msg, look);
			Marshaler.Write(_out_msg, move);
			if (_out_msg.IsError) return false;
			return m_Networker.Send(_out_msg);
		}
		public bool Request_Chat(HostID target, String msg)
		{
			if (m_Networker == null)
			{
				Log.Debug("{0} is not initialized.", typeof(Proxy));
				return false;
			}
			Log.Debug("C2S.Proxy.Request_Chat");
			NetBuffer _out_msg = NetBufferPool.Instance.Pop();
			_out_msg.Init((Int16)RMI_ID.Request_Chat, target);
			Marshaler.Write(_out_msg, msg);
			if (_out_msg.IsError) return false;
			return m_Networker.Send(_out_msg);
		}
	}

}
namespace Devarc
{
	[System.Serializable]
	public class Request_Move : IBaseObejct
	{
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
		    sb.Append("{"); sb.Append(" \"look\":"); sb.Append(look.IsDefault == false ? look.ToString() : "{}");
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
	public class Request_Chat : IBaseObejct
	{
		public string              msg = "";

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
		}
		public void Initialize(PropTable obj)
		{
			msg                 = obj.GetStr("msg");
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("msg")) msg = obj["msg"].ToString(); else msg = default(string);
		}
		public void Initialize(IBaseReader obj)
		{
			msg                 = obj.GetString("msg");
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"msg\":"); sb.Append("\""); sb.Append(msg); sb.Append("\"");
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
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("Request_Chat");
			obj.Attach("msg", "string", CLASS_TYPE.VALUE, false, msg);
			return obj;
		}
	}
}
