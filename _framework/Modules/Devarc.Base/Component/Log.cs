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
    }

    public class Log
    {
        public delegate void CALLBACK_MESSAGE(LOG_TYPE tp, string msg);
        public delegate void CALLBACK_EXCEPTION(Exception e);

        public static Log Instance { get { if (ms_Instance == null) ms_Instance = new Log(); return ms_Instance; } }
        private static Log ms_Instance = null;

        private CALLBACK_MESSAGE callback_message = null;
        private CALLBACK_EXCEPTION callback_exception = null;

        public static void SetCallback(CALLBACK_MESSAGE func)
        {
            Log.Instance.callback_message = func;
        }

        public static void SetCallback(CALLBACK_EXCEPTION func)
        {
            Log.Instance.callback_exception = func;
        }

        public static void Info(string msg, params object[] args)
        {
            if (Log.Instance.callback_message != null)
            {
                if (args.Length > 0)
                    Log.Instance.callback_message(LOG_TYPE.INFO, System.String.Format(msg, args));
                else
                    Log.Instance.callback_message(LOG_TYPE.INFO, msg);
            }
        }
        public static void Debug(string msg, params object[] args)
        {
            if (Log.Instance.callback_message != null)
            {
                if (args.Length > 0)
                    Log.Instance.callback_message(LOG_TYPE.DEBUG, System.String.Format(msg, args));
                else
                    Log.Instance.callback_message(LOG_TYPE.DEBUG, msg);
            }
        }

        public static void Error(string msg, params object[] args)
        {
            if (Log.Instance.callback_message != null)
            {
                if (args.Length > 0)
                    Log.Instance.callback_message(LOG_TYPE.ERROR, System.String.Format(msg, args));
                else
                    Log.Instance.callback_message(LOG_TYPE.ERROR, msg);
            }
        }

        public static void Exception(Exception e)
        {
            if (Log.Instance.callback_exception != null)
            {
                Log.Instance.callback_exception(e);
            }
        }


    }
}
