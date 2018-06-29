//
// Copyright (c) 2012 Hyoung Joon, Kim
// http://www.dev-arc.com/
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
// @author Hyoung Joon, Kim (maoshy@sogang.ac.kr)
// @version $Rev: 1, $Date: 2012-02-20
//

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Devarc.Util.Windows
{
    public class CmdReader
    {
        byte[] m_buffer = new byte[256];
        Stream m_stream;
        List<string> m_list = new List<string>();

        public void Init()
        {
            m_stream = Console.OpenStandardInput(256);
            m_stream.BeginRead(m_buffer, 0, m_buffer.Length, new AsyncCallback(Callbak_Keyboard), null);
        }

        public bool Read(out string cmd)
        {
            cmd = "";
            if (m_list.Count > 0)
            {
                cmd = m_list[0];
                m_list.RemoveAt(0);
                return true;
            }
            return false;
        }

        void Callbak_Keyboard(IAsyncResult ar)
        {
            int amtRead = m_stream.EndRead(ar);
            m_stream.BeginRead(m_buffer, 0, m_buffer.Length, new AsyncCallback(Callbak_Keyboard), null);
            if (amtRead > 0)
            {
                System.Text.UTF8Encoding encode = new System.Text.UTF8Encoding();
                m_list.Add(encode.GetString(m_buffer, 0, amtRead - 2));
            }
        }
    }
}
