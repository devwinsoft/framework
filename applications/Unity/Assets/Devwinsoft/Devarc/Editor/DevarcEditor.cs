using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Text;
using Devarc;

public class DevarcEditor : EditorWindow
{
    delegate bool callback_load(string _data);
    delegate void callback_save(string _path);
    class LoadInfo
    {
        public LoadInfo(string _name, callback_load _load, callback_save _save)
        {
            name = _name;
            load = _load;
            save = _save;
        }
        public string name;
        public callback_load load;
        public callback_save save;
    }

    [@MenuItem("Tools/Devarc Build Tool")]
    static void BakingCharacterTexture()
    {
        EditorWindow.GetWindowWithRect(typeof(DevarcEditor), new Rect(0, 0, 540f, 500f), false);
    }
    
    const string buildConfigPath = "Assets/Devwinsoft/Devarc/BuildSettings.asset";
    BuilderSaveData buildConfigData = null;

    Vector2 scrollPos = Vector2.zero;

    bool showObjectConfig = true;
    bool showObjectInput = false;
    bool showObjectOutput = false;
    List<string> backupObjectInput = new List<string>();
    List<string> backupObjectOutput = new List<string>();

    bool showDataConfig = true;
    bool showDataInput = false;
    bool showDataOutput = false;
    List<string> backupDataInput = new List<string>();
    List<string> backupDataOutput = new List<string>();

    bool showStrConfig = true;
    bool showStrInput = false;
    List<string> backupStrInput = new List<string>();

    string _get_table_name(string _raw_name)
    {
        int tmpIndex = _raw_name.IndexOf('@');
        if (tmpIndex >= 0)
            return _raw_name.Substring(0, tmpIndex);
        else
            return _raw_name;
    }

    void OnEnable()
    {
        this.titleContent.text = "Devarc Build";
    }

    T[] resize<T>(T[] _data, List<T> _backup, int _count)
    {
        for (int i = _backup.Count - 1; i < _count; i++)
        {
            _backup.Add(default(T));
        }
        for (int i = 0; i < _data.Length; i++)
        {
            _backup[i] = _data[i];
        }
        T[] returnList = new T[_count];
        for (int i = 0; i < _count; i++)
        {
            returnList[i] = _backup[i];
        }
        return returnList;
    }

    void OnGUI()
    {
        if (buildConfigData == null)
        {
            buildConfigData = (BuilderSaveData)AssetDatabase.LoadAssetAtPath(buildConfigPath, typeof(BuilderSaveData));
            if (buildConfigData == null)
            {
                buildConfigData = new BuilderSaveData();
                AssetDatabase.CreateAsset(buildConfigData, buildConfigPath);
            }
        }

        if (buildConfigData == null)
        {
            return;
        }
        EditorUtility.SetDirty(buildConfigData);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        showObjectConfig = EditorGUILayout.Foldout(showObjectConfig, "Object Tables");
        if (showObjectConfig)
        {
            EditorGUI.indentLevel++;
            {
                GUI.backgroundColor = Color.white;
                showObjectInput = EditorGUILayout.Foldout(showObjectInput, "Input Tables");

                EditorGUI.indentLevel++;
                if (showObjectInput)
                {
                    int newTableCount = EditorGUILayout.IntField("Excel Count", buildConfigData.inObjFiles.Length);
                    if (newTableCount != buildConfigData.inObjFiles.Length)
                    {
                        buildConfigData.inObjFiles = resize<string>(buildConfigData.inObjFiles, backupObjectInput, newTableCount);
                    }
                    for (int i = 0; i < buildConfigData.inObjFiles.Length; i++)
                    {
                        buildConfigData.inObjFiles[i] = EditorGUILayout.TextField(string.Format("Table-{0}", i), buildConfigData.inObjFiles[i]);
                    }
                }
                EditorGUI.indentLevel--;

                GUI.backgroundColor = Color.white;
                showObjectOutput = EditorGUILayout.Foldout(showObjectOutput, "Output Directory");

                EditorGUI.indentLevel++;
                if (showObjectOutput)
                {
                    int newDirCount = EditorGUILayout.IntField("Output Directory Count", buildConfigData.outObjTables.Length);
                    if (newDirCount != buildConfigData.outObjTables.Length)
                    {
                        buildConfigData.outObjTables = resize<string>(buildConfigData.outObjTables, backupObjectOutput, newDirCount);
                    }
                    for (int i = 0; i < buildConfigData.outObjTables.Length; i++)
                    {
                        buildConfigData.outObjTables[i] = EditorGUILayout.TextField(string.Format("Output Directory-{0}", i), buildConfigData.outObjTables[i]);
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }

        GUI.backgroundColor = Color.white;
        if (GUILayout.Button("Compile Tables !!"))
        {
            Builder_Object builderObject = new Builder_Object();
            Builder_Data builderData = new Builder_Data();

            if (buildConfigData.outObjTables.Length == 0)
            {
                EditorUtility.DisplayDialog("Compile Tables", "No Output Folder.", "Failed");
            }
            else
            {
                for (int k = 0; k < buildConfigData.outObjTables.Length; k++)
                {
                    string tempOutDir = System.IO.Path.Combine(Application.dataPath, buildConfigData.outObjTables[k]);
                    for (int i = 0; i < buildConfigData.inObjFiles.Length; i++)
                    {
                        if (string.IsNullOrEmpty(buildConfigData.inObjFiles[i]))
                            continue;
                        string tempInPath = System.IO.Path.Combine(Application.dataPath, buildConfigData.inObjFiles[i]);
                        builderObject.Build_ExcelFile(tempInPath, tempOutDir);
                        builderData.Build_ExcelFile(tempInPath, tempOutDir);
                    }
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.Default);
                EditorUtility.DisplayDialog("Compile Tables", "Build Completed.", "Success");
            }
        }

        GUILayout.Space(20f);

        showDataConfig = EditorGUILayout.Foldout(showDataConfig, "Data Tables");
        EditorGUI.indentLevel++;
        if (showDataConfig)
        {
            showDataInput = EditorGUILayout.Foldout(showDataInput, "Input Tables");
            EditorGUI.indentLevel++;
            if (showDataInput)
            {
                int newTableCount = EditorGUILayout.IntField("Table Count", buildConfigData.inDataFiles.Length);
                if (newTableCount != buildConfigData.inDataFiles.Length)
                {
                    buildConfigData.inDataFiles = resize<string>(buildConfigData.inDataFiles, backupDataInput, newTableCount);
                }
                for (int i = 0; i < buildConfigData.inDataFiles.Length; i++)
                {
                    buildConfigData.inDataFiles[i] = EditorGUILayout.TextField(string.Format("Table-{0}", i), buildConfigData.inDataFiles[i]);
                }
            }
            EditorGUI.indentLevel--;

            showDataOutput = EditorGUILayout.Foldout(showDataOutput, "Output Directory");
            EditorGUI.indentLevel++;
            if (showDataOutput)
            {
                int newDirCount = EditorGUILayout.IntField("Output Directory Count", buildConfigData.outDataTables.Length);
                if (newDirCount != buildConfigData.outDataTables.Length)
                {
                    buildConfigData.outDataTables = resize<string>(buildConfigData.outDataTables, backupDataOutput, newDirCount);
                }
                for (int i = 0; i < buildConfigData.outDataTables.Length; i++)
                {
                    buildConfigData.outDataTables[i] = EditorGUILayout.TextField(string.Format("Output Directory-{0}", i), buildConfigData.outDataTables[i]);
                }
            }
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;
        if (GUILayout.Button("Make Spread Sheet Files !!"))
        {
            BuildUtil util = new BuildUtil();
            util.BuildDataFile(DATA_FILE_TYPE.SHEET, buildConfigData.inDataFiles, buildConfigData.outDataTables);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.Default);
            EditorUtility.DisplayDialog("Make Spread Sheet Files", "Build Completed.", "Success");
        }
        if (GUILayout.Button("Make Json Files !!"))
        {
            BuildUtil util = new BuildUtil();
            util.BuildDataFile(DATA_FILE_TYPE.JSON, buildConfigData.inDataFiles, buildConfigData.outDataTables);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.Default);
            EditorUtility.DisplayDialog("Make Json Files", "Build Completed.", "Success");
        }

        GUILayout.Space(20f);

        showStrConfig = EditorGUILayout.Foldout(showStrConfig, "String Tables");
        EditorGUI.indentLevel++;
        if (showStrConfig)
        {
            showStrInput = EditorGUILayout.Foldout(showStrInput, "Input Tables");
            EditorGUI.indentLevel++;
            if (showStrInput)
            {
                int newTableCount = EditorGUILayout.IntField("Table Count", buildConfigData.inStrTables.Length);
                if (newTableCount != buildConfigData.inStrTables.Length)
                {
                    buildConfigData.inStrTables = resize<string>(buildConfigData.inStrTables, backupStrInput, newTableCount);
                }
                for (int i = 0; i < buildConfigData.inStrTables.Length; i++)
                {
                    buildConfigData.inStrTables[i] = EditorGUILayout.TextField(string.Format("Table-{0}", i), buildConfigData.inStrTables[i]);
                }
            }
            EditorGUI.indentLevel--;

            buildConfigData.outStrDir = EditorGUILayout.TextField("Output Directory", buildConfigData.outStrDir);
        }
        EditorGUI.indentLevel--;

        if (GUILayout.Button("Build Localized String."))
        {
            BuildUtil util = new BuildUtil();
            util.BuildLString(buildConfigData.inStrTables, buildConfigData.outStrDir);
            EditorUtility.DisplayDialog("Build Localized String", "Build Completed.", "Success");
        }

        GUILayout.EndScrollView();
    }
}
