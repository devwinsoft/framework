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
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using LitJson;

namespace Devarc
{
    public interface IBaseObejct
    {
        void Initialize(IBaseObejct obj);
        void Initialize(PropTable obj);
        void Initialize(IBaseReader obj);
        void Initialize(LitJson.JsonData obj);
    }

    public interface IBaseObejct<T> : IBaseObejct
    {
        T GetKey();
        string GetSelectQuery(T _key);
    }

    public interface IContents<KEY1, KEY2>
    {
        void OnAlloc(KEY1 k1, KEY2 k2);
        void OnFree();
        KEY1 GetKey1();
        KEY2 GetKey2();
    }

    public interface IBaseReader
    {
        void Close();
        bool Read();

        short GetInt16(string _name);
        int GetInt32(string _name);
        long GetInt64(string _name);
        uint GetUInt32(string _name);
        float GetFloat(string _name);
        string GetString(string _name);
    }
}
