using System.Collections;
using System.Collections.Generic;

namespace Devarc
{
    public partial class Table
    {
        public static SQLite_Session Session { get { return msSession; } }
        static SQLite_Session msSession = null;

        public static bool Open(string _filePath)
        {
            if (msSession == null)
            {
                msSession = new SQLite_Session();
                msSession.Open(_filePath);
            }
            return true;
        }

        public static void Close()
        {
            if (msSession != null)
            {
                msSession.Close();
                msSession = null;
            }
        }
    }
}
