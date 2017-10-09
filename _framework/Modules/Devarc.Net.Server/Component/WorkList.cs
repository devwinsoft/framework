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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace Devarc
{
    public class WorkList
    {
        private List<NetBuffer> m_WorkList = new List<NetBuffer>();

        public void Push(NetBuffer msg)
        {
            m_WorkList.Add(msg);
        }

        public NetBuffer Pop(HashSet<HostID> excludeList)
        {
            NetBuffer obj = null;
            for (int i = 0; i < m_WorkList.Count; i++)
            {
                NetBuffer temp = m_WorkList[i];
                if (excludeList.Contains(temp.Hid))
                    continue;
                obj = temp;
                m_WorkList.Remove(obj);
            }
            return obj;
        }
    }
}
