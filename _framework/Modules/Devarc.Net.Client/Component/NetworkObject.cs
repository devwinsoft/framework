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
using System.Net;
using System.Collections;
using System.Collections.Generic;
using SuperSocket;

namespace Devarc
{
    public delegate bool NET_RECEIVER(object sender, NetBuffer msg);

    public enum DISCONNECTION_REASON
    {
        CONNECTION_FAIL,
        ERROR,
        BY_USER,
        BY_SERVER,
    }

    public enum RECEIVE_RESULT
    {
        SUCCESS,
        NOT_IMPLEMENTED,
        INVALID_PACKET,
    }

    public interface IProxyBase
    {
        short GetCurrentSeq(HostID hid);
        bool Send(NetBuffer msg);
    }
}
