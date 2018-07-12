using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using StackExchange.Redis;

namespace Devarc
{
    public class Redis_Session : IDisposable
    {
        ConnectionMultiplexer mConnection;
        IDatabase mDatabase;

        public void Dispose()
        {
            Close();
        }

        public bool Connect(string _host, int _port)
        {
            try
            {
                mConnection = ConnectionMultiplexer.Connect(_host + ":" + _port);
                if (mConnection.IsConnected)
                {
                    mDatabase = mConnection.GetDatabase();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public void Close()
        {
            if (mConnection != null)
            {
                mConnection.Close();
            }
            mConnection = null;
            mDatabase = null;
        }

        public string GetString(string _key)
        {
            return mDatabase.StringGet(_key);
        }

        public string[] GetString(string[] _keys)
        {
            RedisKey[] tmpKeys = new RedisKey[_keys.Length];
            for (int i = 0; i < tmpKeys.Length; i++)
            {
                tmpKeys[i] = _keys[i];
            }

            RedisValue[] tmpValues = mDatabase.StringGet(tmpKeys);
            string[] retValues = new string[tmpValues.Length];
            for (int i = 0; i < tmpValues.Length; i++)
            {
                retValues[i] = tmpValues[i];
            }
            return retValues;
        }

        public bool SetString(string _key, string _value)
        {
            try
            {
                mDatabase.StringSet(_key, _value);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }
    }
}
