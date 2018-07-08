using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Devarc
{
    public partial class Table
    {
        public static SQLite_Session Connection { get { return msConnection; } }
        static SQLite_Session msConnection = null;

        public static bool Open(string _filePath)
        {
            if (msConnection == null)
            {
                msConnection = new SQLite_Session();
                msConnection.Open(_filePath);
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
