using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Devarc;

public class BuildUtil
{
    public void BuildLString(string _workingFolder, TextAsset[] _inputList)
    {
        listCurrent.Clear();
        listHistory.Clear();
        listBuilt.Clear();

        string workingPath = Path.Combine(Application.dataPath, _workingFolder);
        using (XmlReader readerCur = new XmlReader())
        {
            readerCur.RegisterCallback_EveryLine(callbackCurrent);
            for (int i = 0; i < _inputList.Length; i++)
            {
                if (_inputList[i] == null)
                    continue;
                readerCur.ReadData(_inputList[i].text);
            }
        }

        string tempPath = Path.Combine(workingPath, "LString_.xml");
        if (File.Exists(tempPath))
        {
            using (XmlReader readerTemp = new XmlReader())
            {
                readerTemp.RegisterCallback_EveryLine(callbackHistory);
                readerTemp.ReadFile(tempPath);
            }
        }

        using (XmlReader readerBuilt = new XmlReader())
        {
            string[] locales = new string[]
            { "" // main language
            , "ENG"
            };
            readerBuilt.RegisterCallback_EveryLine(callbackBuilt);

            for (int i = 0; i < locales.Length; i++)
            {
                listBuilt.Clear();
                TableManager.UnLoad_LString();

                string destPath = Path.Combine(workingPath, string.Format("LString_{0}.xml", locales[i]));
                // read old data
                if (File.Exists(destPath))
                {
                    readerBuilt.ReadFile(destPath);
                }

                // build new data
                foreach(string key in listCurrent.Keys)
                {
                    string newValue = listCurrent[key];
                    string oldValue;
                    string saveValue = "";
                    if (i == 0)
                    {
                        saveValue = newValue;
                    }
                    else if (listHistory.TryGetValue(key, out oldValue))
                    {
                        if (string.Compare(oldValue, newValue) == 0)
                        {
                            listBuilt.TryGetValue(key, out saveValue);
                        }
                    }
                    if (T_LString.LIST.Contains1(key))
                    {
                        Debug.Log(string.Format("Duplicated key: {0}", key));
                        continue;
                    }
                    LString obj = T_LString.LIST.Alloc(key);
                    obj.Key = key;
                    obj.Value = saveValue;
                }

                // save
                TableManager.Save_LString_XmlFile(destPath);
            }
        }
    }

    void callbackCurrent(string sheet_name, PropTable tb)
    {
        for (int i = 0; i < tb.Length; i++)
        {
            if (tb.GetVarType(i) != VAR_TYPE.LSTRING)
                continue;

            string key = FrameworkUtil.MakeLStringKey("LString", "Key", tb.GetStr(tb.KeyIndex));
            if (listCurrent.ContainsKey(key))
                continue;

            listCurrent.Add(key, tb.GetStr(i));
        }
    }

    void callbackHistory(string sheet_name, PropTable tb)
    {
        string key = tb.GetStr("Key");
        string value = tb.GetStr("Value");
        if (listHistory.ContainsKey(key))
            return;
        listHistory.Add(key, value);
    }

    void callbackBuilt(string sheet_name, PropTable tb)
    {
        string key = tb.GetStr("Key");
        string value = tb.GetStr("Value");
        if (listBuilt.ContainsKey(key))
            return;
        listBuilt.Add(key, value);
    }

    Dictionary<string, string> listCurrent = new Dictionary<string, string>();
    Dictionary<string, string> listHistory = new Dictionary<string, string>();
    Dictionary<string, string> listBuilt = new Dictionary<string, string>();
}
