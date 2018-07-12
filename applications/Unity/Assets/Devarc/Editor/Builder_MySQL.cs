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
//
using System;
using System.Collections.Generic;
using System.Text;

namespace Devarc
{
    class Builder_MySQL : Builder_Base, IDisposable
    {
        public void Dispose()
        {
        }

        void Callback_Header(string _sheetName, PropTable _prop)
        {
            if (_prop.KeyIndex < 0)
                return;
            string tableName = GetClassName(_sheetName);
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("DROP TABLE IF EXISTS {0};", tableName));
            sb.Append("CREATE TABLE ");
            sb.Append(tableName);
            for (int i = 0; i < _prop.Length; i++)
            {
                if (i == 0)
                    sb.Append(" (\n\t");
                else
                    sb.Append(",\n\t");
                sb.Append(string.Format("{0} varchar(50) NOT NULL", _prop.GetVarName(i)));
                if (_prop.KeyIndex == i)
                    sb.Append(" PRIMARY KEY");
            }
            sb.Append(");");
        }
    }
}
