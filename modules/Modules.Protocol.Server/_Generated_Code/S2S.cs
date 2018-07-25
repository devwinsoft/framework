using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Devarc;
namespace Devarc.S2S
{
	public interface IStub
	{
		void RMI_S2S_Ping(HostID remote, Ping msg);
	}
	public static class Stub
	{
		public static RECEIVE_RESULT OnReceive(IStub stub, NetBuffer _in_msg)
		{
			RMI_ID rmi_id = (RMI_ID)_in_msg.Rmi;
			switch (rmi_id)
			{
				case RMI_ID.Ping:
					try
					{
						Log.Debug("S2S.Stub.Ping");
						Ping msg = new Ping();
						Marshaler.Read(_in_msg, msg.pos);
						Marshaler.Read(_in_msg, out msg.data);
						if (_in_msg.IsCompleted == false) return RECEIVE_RESULT.INVALID_PACKET_DOWNFLOW;
						stub.RMI_S2S_Ping(_in_msg.Hid, msg);
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
		Ping                           = 20010,
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
		public SEND_RESULT Ping(HostID target, TEST_VECTOR pos, Byte[] data)
		{
			Log.Debug("S2S.Proxy.Ping");
			NetBuffer _out_msg = NetBufferPool.Instance.Pop();
			_out_msg.Init((Int16)RMI_ID.Ping, target);
			Marshaler.Write(_out_msg, pos);
			Marshaler.Write(_out_msg, data);
			return Send(_out_msg);
		}
	}
}
namespace Devarc.S2S
{
	[System.Serializable]
	[NetProtocolAttribute(RMI_ID = 20010)]
	public class Ping : IBasePacket
	{
		public short RMI_ID { get { return 20010; } }
		public bool WriteTo(NetBuffer _obj)
		{
			Marshaler.Write(_obj, pos);
			Marshaler.Write(_obj, data);
			return true;
		}
		public TEST_VECTOR         pos = new TEST_VECTOR();
		public byte[]              data = null;

		public Ping()
		{
		}
		public Ping(Ping obj)
		{
			Initialize(obj);
		}
		public Ping(PropTable obj)
		{
			Initialize(obj);
		}
		public bool IsDefault
		{
			get
			{
				if (pos.IsDefault == false) return false;
				if (data != null && data.Length > 0) return false;
				return true;
			}
		}
		public void Initialize(IBaseObejct from)
		{
			Ping obj = from as Ping;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:Ping");
				return;
			}
			pos.Initialize(obj.pos);
			data = new byte[obj.data.Length];
			Array.Copy(obj.data, data, data.Length);
		}
		public void Initialize(PropTable obj)
		{
			pos.Initialize(obj.GetTable("pos"));
			data                = obj.GetBytes("data");
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("pos")) pos.Initialize(obj["pos"]);
			data = Convert.FromBase64String(obj["data"].ToString());
		}
		public void Initialize(IBaseReader obj)
		{
			pos.Initialize(JsonMapper.ToObject(obj.GetString("pos")));
			data = Convert.FromBase64String(obj.GetString("data"));
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"pos\":"); sb.Append(pos.ToString());
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
			if (pos.IsDefault == false) { if (j > 0) { sb.Append(", "); } j++;
			 sb.Append("\"pos\":"); sb.Append(pos.ToJson()); }
			if (data != null && data.Length > 0) { if (j > 0) { sb.Append(", "); } j++;
			 sb.Append("\"data\":"); sb.Append(string.Format("\"{0}\"", FrameworkUtil.ToBase64String(data))); }
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("Ping");
			obj.Attach_Class("pos", "TEST_VECTOR", pos.ToTable());
			obj.Attach("data", "byte[]", CLASS_TYPE.VALUE, false, FrameworkUtil.ToBase64String(data));
			return obj;
		}
	}
}
