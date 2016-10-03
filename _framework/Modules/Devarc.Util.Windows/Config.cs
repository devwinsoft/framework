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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Devarc.Util.Windows
{
    public class Config
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);
        
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);

        public static string GetString(string category, string key, string file_name)
        {
            StringBuilder sb = new StringBuilder();
            string file_path = System.IO.Directory.GetCurrentDirectory() + @"\" + file_name;
            GetPrivateProfileString(category, key, "", sb, 256, file_path);
            return sb.ToString();
        }

        public static int GetInt(string category, string key, string file_name)
        {
            StringBuilder sb = new StringBuilder();
            string file_path = System.IO.Directory.GetCurrentDirectory() + @"\" + file_name;
            return GetPrivateProfileInt(category, key, 0, file_path);
        }
    }
}
