//
// Copyright (c) 2012 Hyoung Joon, Kim
// http://www.devwinsoft.com/
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

//
// @author Hyoung Joon, Kim (maoshy@nate.com)
// @version $Rev: 1, $Date: 2012-02-20
//

using System;
using System.Collections.Generic;
using System.Text;

namespace Devarc
{
    public enum LOG_TYPE
    {
        INFO,
        DEBUG,
        ERROR,
        EXCEPTION,
    }

    public class Log
    {
        public delegate void CALLBACK_MESSAGE(LOG_TYPE tp, string msg);

        private static Log Instance { get { if (msInstance == null) msInstance = new Log(); return msInstance; } }
        private static Log msInstance = null;

        public static CALLBACK_MESSAGE OnMessage
        {
            get { return Instance.message; }
            set { Instance.message = value; }
        }
        private event CALLBACK_MESSAGE message;

        public static void Info(string msg, params object[] args)
        {
            if (Log.Instance.message != null)
            {
                if (args.Length > 0)
                    Log.Instance.message.Invoke(LOG_TYPE.INFO, System.String.Format(msg, args));
                else
                    Log.Instance.message.Invoke(LOG_TYPE.INFO, msg);
            }
        }

        public static void Debug(string msg, params object[] args)
        {
#if DEBUG
            if (Log.Instance.message != null)
            {
                if (args.Length > 0)
                    Log.Instance.message.Invoke(LOG_TYPE.DEBUG, System.String.Format(msg, args));
                else
                    Log.Instance.message.Invoke(LOG_TYPE.DEBUG, msg);
            }
#endif
        }

        public static void Error(string msg, params object[] args)
        {
            if (Log.Instance.message != null)
            {
                if (args.Length > 0)
                    Log.Instance.message.Invoke(LOG_TYPE.ERROR, System.String.Format(msg + "\r\n" + Environment.StackTrace, args));
                else
                    Log.Instance.message.Invoke(LOG_TYPE.ERROR, msg + "\r\n" + Environment.StackTrace);
            }
        }

        public static void Exception(Exception e)
        {
            if (Log.Instance.message != null)
            {
                if (e.StackTrace != null)
                {
                    Log.Instance.message.Invoke(LOG_TYPE.EXCEPTION, e.StackTrace.ToString());
                }
                Log.Instance.message.Invoke(LOG_TYPE.EXCEPTION, e.Message);
            }
        }
    }
}
