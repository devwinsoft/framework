using System.Collections;
using System.Collections.Generic;
using System.Data;
#if UNITY_5
using UnityEngine;
using Mono.Data.Sqlite;
#else
using System.Data.SQLite;
using SqliteDataReader = System.Data.SQLite.SQLiteDataReader;
using SqliteConnection = System.Data.SQLite.SQLiteConnection;
using SqliteCommand = System.Data.SQLite.SQLiteCommand;
#endif

namespace Devarc
{
    public partial class TableManager
    {
        public static SqliteConnection Connection { get { return msConnection; } }
        static SqliteConnection msConnection = null;

        public static bool Open(string _filePath)
        {
            string conn = "URI=file:" + _filePath;
            if (msConnection == null)
            {
                msConnection = new SqliteConnection(conn);
                msConnection.Open();
            }
            return true;
        }

        public static void Close()
        {
            if (msConnection != null)
            {
                msConnection.Close();
                msConnection = null;
            }
        }
    }
}
