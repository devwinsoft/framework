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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Devarc
{
    public class XmlSheetReader : BaseDataReader
    {
        public override bool ReadFile(string _filePath)
        {
            if (System.IO.File.Exists(_filePath) == false)
            {
                Log.Debug("Cannot find file: " + _filePath);
                return false;
            }
            XmlDocument doc = new XmlDocument();
            XmlNodeReader reader = null;
            try
            {
                doc.Load(_filePath);
                reader = new XmlNodeReader(doc);
                _Parse(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return true;
        }

        public override bool ReadData(string _data)
        {
            XmlDocument doc = new XmlDocument();
            XmlNodeReader reader = null;
            try
            {
                doc.LoadXml(_data);
                reader = new XmlNodeReader(doc);
                _Parse(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return true;
        }

        void _Parse(XmlNodeReader xrd)
        {
            PropTable   temp_table = null;
	        int			line_count = 0;
	        int			index = 0;
	        bool		function_called = false;;
	        string		tempString;

            while (xrd.Read())
            {
                if (xrd.NodeType == XmlNodeType.Element)
                {
                    if ("Worksheet" == xrd.Name)
                    {
                        string tmpSheetName = xrd.GetAttribute("ss:Name");
                        int tmpIndex = tmpSheetName.IndexOf('@');
                        if (tmpIndex >= 0)
                            m_SheetName = tmpSheetName.Substring(0, tmpIndex);
                        else
                            m_SheetName = tmpSheetName;
                    }
                    if ("Table" == xrd.Name)
                    {
                        temp_table = new PropTable();
                        //temp_table.ClearAll();
                        index = 0;
                        line_count = 0;
                        function_called = false; ;
                    }
                    else if ("Row" == xrd.Name)
                    {
                        // Line 인덱스 업데이트
                        if (xrd.MoveToAttribute("ss:Index"))
                        {
                            tempString = xrd.Value;
                            line_count = System.Int32.Parse(tempString);
                        }
                        else
                        {
                            line_count++;
                        }

                        if (line_count >= (int)ROW_TYPE.DATA_FIELD)
                        {
                            if (function_called == false)
                            {
                                function_called = true;
                                if (callback_every_header != null)
                                {
                                    callback_every_header(m_SheetName, temp_table);
                                }
                            }
                        }

                        index = 0;
                        temp_table.ClearData();
                    }
                    else if ("Cell" == xrd.Name)
                    {
                        // Column 인덱스 업데이트
                        if (xrd.MoveToAttribute("ss:Index"))
                        {
                            tempString = xrd.Value;
                            index = System.Int32.Parse(tempString);
                        }
                        else
                        {
                            index++;
                        }
                    }
                    else if ("Data" == xrd.Name)
                    {
                        // Data 다음에는 무조건 Text 데이타가 오는 것으로 간주.
                        xrd.Read();

                        if (line_count == (int)ROW_TYPE.VAR_NAME)
                        {
                            temp_table.initVarName(index - 1, xrd.Value);
                        }
                        else if (line_count == (int)ROW_TYPE.TYPE_NAME)
                        {
                            temp_table.initVarType(index - 1, xrd.Value);
                        }
                        else if (line_count == (int)ROW_TYPE.CLASS_TYPE)
                        {
                            temp_table.initClassType(index - 1, xrd.Value);
                        }
                        else if (line_count == (int)ROW_TYPE.KEY_TYPE)
                        {
                            temp_table.initKeyType(index - 1, xrd.Value);
                        }
                        else
                        {
                            temp_table.SetData(index - 1, xrd.Value);
                        }
                    }
                }
                else if (xrd.NodeType == XmlNodeType.EndElement)
                {
                    // 예외 처리
                    if ("Worksheet" == xrd.Name)
                    {
                        if (function_called == false)
                        {
                            if (callback_every_header != null)
                            {
                                callback_every_header(m_SheetName, temp_table);
                            }
                            function_called = true;
                        }
                    }
                    else if ("Table" == xrd.Name)
                    {
                        if (function_called == false && line_count > 1)
                        {
                            if (callback_every_header != null)
                            {
                                callback_every_header(m_SheetName, temp_table);
                            }
                            function_called = true;
                        }
                        temp_table = null;
                    }
                    else if ("Row" == xrd.Name)
                    {
                        if (line_count >= (int)ROW_TYPE.DATA_FIELD)
                        {
                            invoke_callback_line(m_SheetName, temp_table);
                        }
                    }
                }
            }
        }
    }
}
