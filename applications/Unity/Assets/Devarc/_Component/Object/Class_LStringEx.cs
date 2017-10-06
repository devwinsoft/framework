using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data;
#if UNITY_5
using Mono.Data.Sqlite;
#else
using System.Data.SQLite;
using SqliteDataReader = System.Data.SQLite.SQLiteDataReader;
using SqliteConnection = System.Data.SQLite.SQLiteConnection;
using SqliteCommand = System.Data.SQLite.SQLiteCommand;
#endif
using LitJson;

namespace Devarc
{
    public partial class LString
    {
        public static implicit operator string(LString obj)
        {
            return obj.Value;
        }


        public LString(string _key)
        {
            Key = _key;
        }
    }
}
