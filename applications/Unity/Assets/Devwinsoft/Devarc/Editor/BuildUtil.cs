using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Devarc;

public class BuildUtil
{
    Dictionary<string, string> listCurrent = new Dictionary<string, string>();
    Dictionary<string, string> listHistory = new Dictionary<string, string>();
    Dictionary<string, string> listBuilt = new Dictionary<string, string>();

    public bool BuildDataFile(DATA_FILE_TYPE _saveFileType, string[] _inFileList, string[] _outDirList)
    {
        if (_saveFileType == DATA_FILE_TYPE.EXCEL)
        {
            Log.Error("Cannot save excel file.");
            return false;
        }
        Assembly assem = null;
        Assembly[] _assems = System.AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < _assems.Length; i++)
        {
            if (_assems[i].FullName.Contains("Assembly") && _assems[i].FullName.Contains("Editor") == false)
            {
                assem = _assems[i];
                break;
            }
        }
        if (assem != null)
        {
            System.Type _type = typeof(TableManagerEx);
            foreach (MethodInfo m in _type.GetMethods())
            {
                if (m.Name.StartsWith("UnLoad"))
                {
                    m.Invoke(null, null);
                }
            }

            List<string> tmpSaveList = new List<string>();
            foreach (string tmpFile in _inFileList)
            {
                string tmpFilePath = Path.Combine(Application.dataPath, tmpFile);
                string tmpTableName = FrameworkUtil.GetClassNameEx(tmpFile);
                string tmpMethodName;
                switch (_saveFileType)
                {
                    case DATA_FILE_TYPE.JSON:
                        tmpMethodName = string.Format("Load_{0}_JsonFile", tmpTableName);
                        break;
                    case DATA_FILE_TYPE.SHEET:
                        tmpMethodName = string.Format("Load_{0}_SheetFile", tmpTableName);
                        break;
                    case DATA_FILE_TYPE.EXCEL:
                        tmpMethodName = string.Format("Load_{0}_ExcelFile", tmpTableName);
                        break;
                    default:
                        tmpMethodName = "";
                        break;
                }
                foreach (MethodInfo m in _type.GetMethods())
                {
                    if (m.Name.Equals(tmpMethodName))
                    {
                        m.Invoke(null, new object[] { tmpFilePath });
                        break;
                    }
                }
                if (tmpSaveList.Contains(tmpTableName) == false)
                    tmpSaveList.Add(tmpTableName);
            }
            foreach (string tmpTableName in tmpSaveList)
            {
                for (int i = 0; i < _outDirList.Length; i++)
                {
                    string subPath = System.IO.Path.Combine(Application.dataPath, _outDirList[i]);
                    string savePath;
                    string tmpMethodName;
                    switch (_saveFileType)
                    {
                        case DATA_FILE_TYPE.JSON:
                            savePath = System.IO.Path.Combine(subPath, tmpTableName + ".json");
                            tmpMethodName = string.Format("Save_{0}_JsonFile", tmpTableName);
                            break;
                        case DATA_FILE_TYPE.SHEET:
                            savePath = System.IO.Path.Combine(subPath, tmpTableName + ".xml");
                            tmpMethodName = string.Format("Save_{0}_SheetFile", tmpTableName);
                            break;
                        case DATA_FILE_TYPE.EXCEL:
                            savePath = System.IO.Path.Combine(subPath, tmpTableName + ".xlsx");
                            tmpMethodName = string.Format("Save_{0}_ExcelFile", tmpTableName);
                            break;
                        default:
                            subPath = "";
                            savePath = "";
                            tmpMethodName = "";
                            break;
                    }
                    foreach (MethodInfo m in _type.GetMethods())
                    {
                        if (m.Name.Equals(tmpMethodName))
                        {
                            m.Invoke(null, new object[] { savePath });
                        }
                    }
                }
            }
        }
        return true;
    }

    public void BuildLString(string[] _inputList, string _outputFolder)
    {
        listCurrent.Clear();
        listHistory.Clear();
        listBuilt.Clear();

        string outputDir = Path.Combine(Application.dataPath, _outputFolder);
        if (Directory.Exists(outputDir) == false)
        {
            if (Directory.CreateDirectory(outputDir) == null)
            {
                Log.Debug("Cannot Create Path: {0}", outputDir);
                return;
            }
        }

        using (ExcelReader readerCur = new ExcelReader())
        {
            readerCur.RegisterCallback_EveryLine(callbackCurrent);
            for (int i = 0; i < _inputList.Length; i++)
            {
                if (_inputList[i] == null)
                    continue;
                readerCur.ReadFile(Path.Combine(Application.dataPath, _inputList[i]));
            }
        }

        string tempPath = Path.Combine(outputDir, "LString_.xml"); // main language
        if (File.Exists(tempPath))
        {
            using (XmlSheetReader readerTemp = new XmlSheetReader())
            {
                readerTemp.RegisterCallback_EveryLine(callbackHistory);
                readerTemp.ReadFile(tempPath);
            }
        }

        using (XmlSheetReader readerBuilt = new XmlSheetReader())
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

                string destPath = Path.Combine(outputDir, string.Format("LString_{0}.xml", locales[i]));
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
                    if (T_LString.MAP.Contains(key))
                    {
                        Debug.Log(string.Format("Duplicated key: {0}", key));
                        continue;
                    }
                    LString obj = T_LString.MAP.Alloc(key);
                    obj.Key = key;
                    obj.Value = saveValue;
                }

                // save
                TableManagerEx.Save_LString_SheetFile(destPath);
            }
        }
    }

    void callbackCurrent(string sheet_name, PropTable tb)
    {
        string className = FrameworkUtil.GetClassName(sheet_name);
        for (int i = 0; i < tb.Length; i++)
        {
            if (tb.GetVarType(i) != VAR_TYPE.LSTRING)
                continue;

            string key = FrameworkUtil.MakeLStringKey(className, tb.GetVarName(i), tb.GetStr(tb.KeyIndex));
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

}
