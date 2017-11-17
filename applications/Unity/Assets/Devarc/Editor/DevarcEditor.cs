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

    [@MenuItem("Tools/Devarc")]
    static void BakingCharacterTexture()
    {
        EditorWindow.GetWindowWithRect(typeof(DevarcEditor), new Rect(0, 0, 540f, 500f), false);
    }
    
    BuilderSaveData config = null;
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
    bool[] showSQLiteItem = new bool[32];
    List<BuildData> backupSQLite = new List<BuildData>();

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
        for (int i = 0; i < _data.Length && i < _backup.Count; i++)
        {
            _backup[i] = _data[i];
        }
        T[] returnList = new T[_count];
        for (int i = 0; i < _count && i < _backup.Count; i++)
        {
            returnList[i] = _backup[i];
        }
        return returnList;
    }

    void OnGUI()
    {
        Rect tempRect;
        if (config == null)
        {
            config = (BuilderSaveData)AssetDatabase.LoadAssetAtPath(buildConfigPath, typeof(BuilderSaveData));
            if (config == null)
            {
                config = new BuilderSaveData();
                AssetDatabase.CreateAsset(config, buildConfigPath);
            }
        }

        if (config == null)
        {
            return;
        }
        EditorUtility.SetDirty(config);

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
                    int newTableCount = EditorGUILayout.IntField("File Count", config.inObjFiles.Length);
                    if (newTableCount != config.inObjFiles.Length)
                    {
                        config.inObjFiles = resize<string>(config.inObjFiles, backupObjectInput, newTableCount);
                    }
                    for (int i = 0; i < config.inObjFiles.Length; i++)
                    {
                        config.inObjFiles[i] = EditorGUILayout.TextField(string.Format("File-{0}", i), config.inObjFiles[i]);
                    }
                }
                EditorGUI.indentLevel--;

                GUILayout.Space(5f);

                GUI.backgroundColor = Color.white;
                showObjectOutput = EditorGUILayout.Foldout(showObjectOutput, "Output Source Code Directory");

                EditorGUI.indentLevel++;
                if (showObjectOutput)
                {
                    int newDirCount = EditorGUILayout.IntField("Directory Count", config.outObjTables.Length);
                    if (newDirCount != config.outObjTables.Length)
                    {
                        config.outObjTables = resize<string>(config.outObjTables, backupObjectOutput, newDirCount);
                    }
                    for (int i = 0; i < config.outObjTables.Length; i++)
                    {
                        config.outObjTables[i] = EditorGUILayout.TextField(string.Format("Directory-{0}", i), config.outObjTables[i]);
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

            if (config.outObjTables.Length == 0)
            {
                EditorUtility.DisplayDialog("Generate Source Code", "No Output Folder.", "Failed");
            }
            else
            {
                for (int k = 0; k < config.outObjTables.Length; k++)
                {
                    string tempOutDir = System.IO.Path.Combine(Application.dataPath, config.outObjTables[k]);
                    for (int i = 0; i < config.inObjFiles.Length; i++)
                    {
                        if (string.IsNullOrEmpty(config.inObjFiles[i]))
                            continue;
                        string tempInPath = System.IO.Path.Combine(Application.dataPath, config.inObjFiles[i]);
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
                int newTableCount = EditorGUILayout.IntField("File Count", config.inStrTables.Length);
                if (newTableCount != config.inStrTables.Length)
                {
                    config.inStrTables = resize<string>(config.inStrTables, backupStrInput, newTableCount);
                }
                for (int i = 0; i < config.inStrTables.Length; i++)
                {
                    config.inStrTables[i] = EditorGUILayout.TextField(string.Format("File-{0}", i), config.inStrTables[i]);
                }
            }
            EditorGUI.indentLevel--;

            config.outStrDir = EditorGUILayout.TextField("Output Directory", config.outStrDir);
        }
        EditorGUI.indentLevel--;

        tempRect = GUILayoutUtility.GetAspectRect(position.width / 18f);
        tempRect.xMin = 20f;
        tempRect.xMax = position.width - 20f;
        if (GUI.Button(tempRect, "Build Localized String."))
        {
            BuildUtil util = new BuildUtil();
            util.BuildStrTable(config.inStrTables, config.outStrDir);
            EditorUtility.DisplayDialog("Build Localized String", "Build Completed.", "Success");
        }

        GUILayout.Space(20f);

        showDataConfig = EditorGUILayout.Foldout(showDataConfig, "Generate JSON");
        EditorGUI.indentLevel++;
        if (showDataConfig)
        {
            showDataInput = EditorGUILayout.Foldout(showDataInput, "Excel Files");
            EditorGUI.indentLevel++;
            if (showDataInput)
            {
                int newTableCount = EditorGUILayout.IntField("File Count", config.inDataFiles.Length);
                if (newTableCount != config.inDataFiles.Length)
                {
                    config.inDataFiles = resize<string>(config.inDataFiles, backupDataInput, newTableCount);
                }
                for (int i = 0; i < config.inDataFiles.Length; i++)
                {
                    config.inDataFiles[i] = EditorGUILayout.TextField(string.Format("File-{0}", i), config.inDataFiles[i]);
                }
            }
            EditorGUI.indentLevel--;

            showDataOutput = EditorGUILayout.Foldout(showDataOutput, "Output Directory");
            EditorGUI.indentLevel++;
            if (showDataOutput)
            {
                int newDirCount = EditorGUILayout.IntField("Output Directory Count", config.outDataTables.Length);
                if (newDirCount != config.outDataTables.Length)
                {
                    config.outDataTables = resize<string>(config.outDataTables, backupDataOutput, newDirCount);
                }
                for (int i = 0; i < config.outDataTables.Length; i++)
                {
                    config.outDataTables[i] = EditorGUILayout.TextField(string.Format("Output Directory-{0}", i), config.outDataTables[i]);
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
            util.BuildDataFile(DATA_FILE_TYPE.JSON, config.inDataFiles, config.outDataTables);
            EditorUtility.DisplayDialog("Make Json Files", "Build Completed.", "Success");
        }

        GUILayout.Space(20f);

        showSQLiteConfig = EditorGUILayout.Foldout(showDataConfig, "Generate SQLite");
        EditorGUI.indentLevel++;
        if (showSQLiteConfig)
        {
            if (config.dataSQLite == null)
            {
                config.dataSQLite = new BuildData[0];
            }

            int newDataCount = EditorGUILayout.IntField("Config Count", config.dataSQLite.Length);
            if (newDataCount != config.dataSQLite.Length)
            {
                config.dataSQLite = resize<BuildData>(config.dataSQLite, backupSQLite, newDataCount);
                EditorUtility.SetDirty(config);
            }

            for (int a = 0; a < config.dataSQLite.Length; a++)
            {
                if (config.dataSQLite[a] == null)
                    config.dataSQLite[a] = new BuildData();
                BuildData data = config.dataSQLite[a];

                showSQLiteItem[a] = EditorGUILayout.Foldout(showSQLiteItem[a], string.Format("Config-{0}", a));
                if (showSQLiteItem[a] == false)
                    continue;

                EditorGUI.indentLevel++;
                int newFileCount = EditorGUILayout.IntField("Input Count", data.inFiles.Length);
                if (newFileCount != data.inFiles.Length)
                {
                    data.inFiles = resize<string>(data.inFiles, data.backupFiles, newFileCount);
                    EditorUtility.SetDirty(config);
                }
                EditorGUI.indentLevel++;
                for (int b = 0; b < data.inFiles.Length; b++)
                {
                    data.inFiles[b] = EditorGUILayout.TextField(string.Format("Input-{0}", b), data.inFiles[b]);
                }
                EditorGUI.indentLevel--;
                data.outFile = EditorGUILayout.TextField("Output Path", data.outFile);

                EditorGUI.indentLevel--;
            }
        }
        EditorGUI.indentLevel--;

        tempRect = GUILayoutUtility.GetAspectRect(position.width / 18f);
        tempRect.xMin = 20f;
        tempRect.xMax = position.width - 20f;
        if (GUI.Button(tempRect, "Generate SQLite"))
        {
            for (int a = 0; a < config.dataSQLite.Length; a++)
            {
                BuildData data = config.dataSQLite[a];
                string dbPath = Path.Combine(Application.dataPath, data.outFile);
                using (Builder_SQLite builder = new Builder_SQLite(dbPath))
                {
                    for (int b = 0; b < data.inFiles.Length; b++)
                    {
                        string filePath = Path.Combine(Application.dataPath, data.inFiles[b]);
                        builder.Build(filePath);
                    }
                    builder.Commit();
                }
            }
            EditorUtility.DisplayDialog("Make Json Files", "Build Completed.", "Success");
        }

        GUILayout.EndScrollView();
    }
}
