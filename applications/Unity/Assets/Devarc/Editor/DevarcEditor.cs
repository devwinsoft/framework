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
#if UNITY_5
using Mono.Data.Sqlite;
#else
using System.Data.SQLite;
using SqliteDataReader = System.Data.SQLite.SQLiteDataReader;
using SqliteConnection = System.Data.SQLite.SQLiteConnection;
using SqliteCommand = System.Data.SQLite.SQLiteCommand;
#endif

public class DevarcEditor : EditorWindow
{
    const string buildConfigPath = "Assets/Devarc/BuildSettings.asset";

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

    [@MenuItem("Tools/Devarc")]
    static void BakingCharacterTexture()
    {
        EditorWindow.GetWindowWithRect(typeof(DevarcEditor), new Rect(0, 0, 540f, 500f), false);
    }
    
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


    bool showSQLiteConfig = true;
    bool showSQLiteInput = true;

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
        this.titleContent.text = "Devarc";
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
        Rect tempRect;
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

        showObjectConfig = EditorGUILayout.Foldout(showObjectConfig, "Generate Source-Code");
        if (showObjectConfig)
        {
            EditorGUI.indentLevel++;
            {
                GUI.backgroundColor = Color.white;
                showObjectInput = EditorGUILayout.Foldout(showObjectInput, "Excel Files");

                EditorGUI.indentLevel++;
                if (showObjectInput)
                {
                    int newTableCount = EditorGUILayout.IntField("File Count", buildConfigData.inObjFiles.Length);
                    if (newTableCount != buildConfigData.inObjFiles.Length)
                    {
                        buildConfigData.inObjFiles = resize<string>(buildConfigData.inObjFiles, backupObjectInput, newTableCount);
                    }
                    for (int i = 0; i < buildConfigData.inObjFiles.Length; i++)
                    {
                        buildConfigData.inObjFiles[i] = EditorGUILayout.TextField(string.Format("File-{0}", i), buildConfigData.inObjFiles[i]);
                    }
                }
                EditorGUI.indentLevel--;

                GUILayout.Space(5f);

                GUI.backgroundColor = Color.white;
                showObjectOutput = EditorGUILayout.Foldout(showObjectOutput, "Output Source Code Directory");

                EditorGUI.indentLevel++;
                if (showObjectOutput)
                {
                    int newDirCount = EditorGUILayout.IntField("Directory Count", buildConfigData.outObjTables.Length);
                    if (newDirCount != buildConfigData.outObjTables.Length)
                    {
                        buildConfigData.outObjTables = resize<string>(buildConfigData.outObjTables, backupObjectOutput, newDirCount);
                    }
                    for (int i = 0; i < buildConfigData.outObjTables.Length; i++)
                    {
                        buildConfigData.outObjTables[i] = EditorGUILayout.TextField(string.Format("Directory-{0}", i), buildConfigData.outObjTables[i]);
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }

        GUI.backgroundColor = Color.white;
        tempRect = GUILayoutUtility.GetAspectRect(position.width / 18f);
        tempRect.xMin = 20f;
        tempRect.xMax = position.width - 20f;
        if (GUI.Button(tempRect, "Generate Source Code"))
        {
            Builder_Object builderObject = new Builder_Object();
            Builder_Data builderData = new Builder_Data();

            if (buildConfigData.outObjTables.Length == 0)
            {
                EditorUtility.DisplayDialog("Generate Source Code", "No Output Folder.", "Failed");
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
                        string tempExt = Path.GetExtension(tempInPath);
                        switch (tempExt.ToLower())
                        {
                            case ".xml":
                                builderObject.Build_SheetFile(tempInPath, tempOutDir);
                                builderData.Build_SheetFile(tempInPath, tempOutDir);
                                break;
                            case ".xls":
                            case ".xlsx":
                                builderObject.Build_ExcelFile(tempInPath, tempOutDir);
                                builderData.Build_ExcelFile(tempInPath, tempOutDir);
                                break;
                            default:
                                break;
                        }
                    }
                }
                EditorUtility.DisplayDialog("Generate Source Code", "Build Completed.", "Success");
            }
        }

        GUILayout.Space(20f);

        showStrConfig = EditorGUILayout.Foldout(showStrConfig, "Generate String Tables");
        EditorGUI.indentLevel++;
        if (showStrConfig)
        {
            showStrInput = EditorGUILayout.Foldout(showStrInput, "Excel Files");
            EditorGUI.indentLevel++;
            if (showStrInput)
            {
                int newTableCount = EditorGUILayout.IntField("File Count", buildConfigData.inStrTables.Length);
                if (newTableCount != buildConfigData.inStrTables.Length)
                {
                    buildConfigData.inStrTables = resize<string>(buildConfigData.inStrTables, backupStrInput, newTableCount);
                }
                for (int i = 0; i < buildConfigData.inStrTables.Length; i++)
                {
                    buildConfigData.inStrTables[i] = EditorGUILayout.TextField(string.Format("File-{0}", i), buildConfigData.inStrTables[i]);
                }
            }
            EditorGUI.indentLevel--;

            buildConfigData.outStrDir = EditorGUILayout.TextField("Output Directory", buildConfigData.outStrDir);
        }
        EditorGUI.indentLevel--;

        tempRect = GUILayoutUtility.GetAspectRect(position.width / 18f);
        tempRect.xMin = 20f;
        tempRect.xMax = position.width - 20f;
        if (GUI.Button(tempRect, "Build Localized String."))
        {
            BuildUtil util = new BuildUtil();
            util.BuildLString(buildConfigData.inStrTables, buildConfigData.outStrDir);
            EditorUtility.DisplayDialog("Build Localized String", "Build Completed.", "Success");
        }

        GUILayout.Space(20f);

        showDataConfig = EditorGUILayout.Foldout(showDataConfig, "Generate Data-Table");
        EditorGUI.indentLevel++;
        if (showDataConfig)
        {
            showDataInput = EditorGUILayout.Foldout(showDataInput, "Excel Files");
            EditorGUI.indentLevel++;
            if (showDataInput)
            {
                int newTableCount = EditorGUILayout.IntField("File Count", buildConfigData.inDataFiles.Length);
                if (newTableCount != buildConfigData.inDataFiles.Length)
                {
                    buildConfigData.inDataFiles = resize<string>(buildConfigData.inDataFiles, backupDataInput, newTableCount);
                }
                for (int i = 0; i < buildConfigData.inDataFiles.Length; i++)
                {
                    buildConfigData.inDataFiles[i] = EditorGUILayout.TextField(string.Format("File-{0}", i), buildConfigData.inDataFiles[i]);
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

        tempRect = GUILayoutUtility.GetAspectRect(position.width / 18f);
        tempRect.xMin = 20f;
        tempRect.xMax = position.width - 20f;
        if (GUI.Button(tempRect, "Generate Json Files"))
        {
            BuildUtil util = new BuildUtil();
            util.BuildDataFile(DATA_FILE_TYPE.JSON, buildConfigData.inDataFiles, buildConfigData.outDataTables);
            EditorUtility.DisplayDialog("Make Json Files", "Build Completed.", "Success");
        }

        GUILayout.Space(20f);

        showSQLiteConfig = EditorGUILayout.Foldout(showDataConfig, "Generate SQLite");
        EditorGUI.indentLevel++;
        if (showSQLiteConfig)
        {
            showSQLiteInput = EditorGUILayout.Foldout(showSQLiteInput, "Excel Files");
            EditorGUI.indentLevel++;
            if (showSQLiteInput)
            {
                int newTableCount = EditorGUILayout.IntField("File Count", buildConfigData.inSQLiteFiles.Length);
                if (newTableCount != buildConfigData.inSQLiteFiles.Length)
                {
                    buildConfigData.inSQLiteFiles = resize<string>(buildConfigData.inSQLiteFiles, backupDataInput, newTableCount);
                }
                for (int i = 0; i < buildConfigData.inSQLiteFiles.Length; i++)
                {
                    buildConfigData.inSQLiteFiles[i] = EditorGUILayout.TextField(string.Format("File-{0}", i), buildConfigData.inSQLiteFiles[i]);
                }
            }
            EditorGUI.indentLevel--;

            buildConfigData.outSQLitePath = EditorGUILayout.TextField("Output Path", buildConfigData.outSQLitePath);
        }
        EditorGUI.indentLevel--;

        tempRect = GUILayoutUtility.GetAspectRect(position.width / 18f);
        tempRect.xMin = 20f;
        tempRect.xMax = position.width - 20f;
        if (GUI.Button(tempRect, "Generate SQLite"))
        {
            string dbPath = Path.Combine(Application.dataPath, buildConfigData.outSQLitePath);
            using (Builder_SQLite builder = new Builder_SQLite(dbPath))
            {
                for (int i = 0; i < buildConfigData.inSQLiteFiles.Length; i++)
                {
                    string filePath = Path.Combine(Application.dataPath, buildConfigData.inSQLiteFiles[i]);
                    builder.Build(filePath);
                }
            }
            EditorUtility.DisplayDialog("Make Json Files", "Build Completed.", "Success");
        }

        GUILayout.EndScrollView();
    }
}
