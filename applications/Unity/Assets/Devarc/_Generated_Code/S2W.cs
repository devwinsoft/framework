using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Devarc;
namespace S2W
{
	public interface IStub
	{
		void RMI_S2W_Request_Login(HostID remote, Request_Login msg);
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
						Log.Debug("S2W.Stub.Request_Login");
						Request_Login msg = new Request_Login();
						Marshaler.Read(_in_msg, ref msg.data);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET_DOWNFLOW;
						stub.RMI_S2W_Request_Login(_in_msg.Hid, msg);
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
		Request_Login                  = 10010,
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
		public bool Request_Login(HostID target, String data)
		{
			Log.Debug("S2W.Proxy.Request_Login");
			NetBuffer _out_msg = NetBufferPool.Instance.Pop();
			_out_msg.Init((Int16)RMI_ID.Request_Login, target);
			Marshaler.Write(_out_msg, data);
			return Send(_out_msg);
		}
	}
}
namespace Devarc
{
	[System.Serializable]
	[NetProtocolAttribute(RMI_ID = 10010)]
	public class Request_Login : IBaseObejct
	{
		public string              data = "";

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
				if (string.IsNullOrEmpty(data) == false) return false;
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
			data                = obj.data;
		}
		public void Initialize(PropTable obj)
		{
			data                = obj.GetStr("data");
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("data")) data = obj["data"].ToString(); else data = default(string);
		}
		public void Initialize(IBaseReader obj)
		{
			data                = obj.GetString("data");
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"data\":"); sb.Append("\""); sb.Append(data); sb.Append("\"");
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    if (IsDefault) { return "{}"; }
		    StringBuilder sb = new StringBuilder();
		    int j = 0;
			sb.Append("{");
			if (string.IsNullOrEmpty(data) == false) { if (j > 0) { sb.Append(", "); } j++;
			 sb.Append("\"data\":"); sb.Append("\""); sb.Append(FrameworkUtil.JsonString(data)); sb.Append("\""); }
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("Request_Login");
			obj.Attach("data", "string", CLASS_TYPE.VALUE, false, data);
			return obj;
		}
	}
}
