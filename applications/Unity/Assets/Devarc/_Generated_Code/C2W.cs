using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Devarc;
namespace Devarc.C2W
{
	public interface IStub
	{
		void RMI_C2W_Request_Login(HostID remote, Request_Login msg);
	}
	public static class Stub
	{
		public static RECEIVE_RESULT OnReceive(IStub stub, NetBuffer _in_msg)
		{
			RMI_ID rmi_id = (RMI_ID)_in_msg.Rmi;
			switch (rmi_id)
			{
				case RMI_ID.Request_Login:
					try
					{
						Log.Debug("C2W.Stub.Request_Login");
						Request_Login msg = new Request_Login();
						Marshaler.Read(_in_msg, ref msg.account_id);
						Marshaler.Read(_in_msg, ref msg.passwd);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET_DOWNFLOW;
						stub.RMI_C2W_Request_Login(_in_msg.Hid, msg);
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
		Request_Login                  = 30010,
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
		public bool Request_Login(HostID target, String account_id, String passwd)
		{
			Log.Debug("C2W.Proxy.Request_Login");
			NetBuffer _out_msg = NetBufferPool.Instance.Pop();
			_out_msg.Init((Int16)RMI_ID.Request_Login, target);
			Marshaler.Write(_out_msg, account_id);
			Marshaler.Write(_out_msg, passwd);
			return Send(_out_msg);
		}
	}
}
namespace Devarc.C2W
{
	[System.Serializable]
	[NetProtocolAttribute(RMI_ID = 30010)]
	public class Request_Login : IBaseObejct
	{
		public string              account_id = "";
		public string              passwd = "";

		public Request_Login()
		{
		}
		public Request_Login(Request_Login obj)
		{
			Initialize(obj);
		}
		public Request_Login(PropTable obj)
		{
			Initialize(obj);
		}
		public bool IsDefault
		{
			get
			{
				if (string.IsNullOrEmpty(account_id) == false) return false;
				if (string.IsNullOrEmpty(passwd) == false) return false;
				return true;
			}
		}
		public void Initialize(IBaseObejct from)
		{
			Request_Login obj = from as Request_Login;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:Request_Login");
				return;
			}
			account_id          = obj.account_id;
			passwd              = obj.passwd;
		}
		public void Initialize(PropTable obj)
		{
			account_id          = obj.GetStr("account_id");
			passwd              = obj.GetStr("passwd");
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("account_id")) account_id = obj["account_id"].ToString(); else account_id = default(string);
			if (obj.Keys.Contains("passwd")) passwd = obj["passwd"].ToString(); else passwd = default(string);
		}
		public void Initialize(IBaseReader obj)
		{
			account_id          = obj.GetString("account_id");
			passwd              = obj.GetString("passwd");
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"account_id\":"); sb.Append("\""); sb.Append(account_id); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"passwd\":"); sb.Append("\""); sb.Append(passwd); sb.Append("\"");
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    if (IsDefault) { return "{}"; }
		    StringBuilder sb = new StringBuilder();
		    int j = 0;
			sb.Append("{");
			if (string.IsNullOrEmpty(account_id) == false) { if (j > 0) { sb.Append(", "); } j++;
			 sb.Append("\"account_id\":"); sb.Append("\""); sb.Append(FrameworkUtil.JsonString(account_id)); sb.Append("\""); }
			if (string.IsNullOrEmpty(passwd) == false) { if (j > 0) { sb.Append(", "); } j++;
			 sb.Append("\"passwd\":"); sb.Append("\""); sb.Append(FrameworkUtil.JsonString(passwd)); sb.Append("\""); }
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("Request_Login");
			obj.Attach("account_id", "string", CLASS_TYPE.VALUE, false, account_id);
			obj.Attach("passwd", "string", CLASS_TYPE.VALUE, false, passwd);
			return obj;
		}
	}
}
