using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

    [@MenuItem("Tools/Devarc Framework")]
    static void BakingCharacterTexture()
    {
        EditorWindow.GetWindowWithRect(typeof(DevarcEditor), new Rect(0, 0, 540f, 500f), false);
    }
    
    const string buildConfigPath = "Assets/Devwinsoft/Devarc/BuildSettings.asset";
    BuilderSaveData buildConfigData = null;
    //Assembly protocolAssem = null;
    string protocolNames = "";

    Builder_Object builderObject = new Builder_Object();
    Builder_Data builderData = new Builder_Data();
    Builder_Net builderNet = new Builder_Net();

    Vector2 scrollPos = Vector2.zero;

    bool showObjectConfig = true;
    bool showObjectInput = false;
    bool showObjectOutput = false;
    List<TextAsset> backupObjectInput = new List<TextAsset>();
    List<string> backupObjectOutput = new List<string>();

    bool showDataConfig = true;
    bool showDataInput = false;
    bool showDataOutput = false;
    List<TextAsset> backupDataInput = new List<TextAsset>();
    List<string> backupDataOutput = new List<string>();

    bool showStrConfig = true;
    bool showStrInput = false;
    List<TextAsset> backupStrInput = new List<TextAsset>();

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
        if (buildConfigData == null)
        {
            buildConfigData = (BuilderSaveData)AssetDatabase.LoadAssetAtPath(buildConfigPath, typeof(BuilderSaveData));
            if (buildConfigData == null)
            {
                buildConfigData = new BuilderSaveData();
                AssetDatabase.CreateAsset(buildConfigData, buildConfigPath);
            }
        }

        //if (protocolAssem == null)
        //{
        //    Assembly[] _assems = System.AppDomain.CurrentDomain.GetAssemblies();
        //    for (int i = 0; i < _assems.Length && protocolAssem == null; i++)
        //    {
        //        if (_assems[i].FullName.Contains("Assembly")
        //            && _assems[i].FullName.Contains("Editor"))
        //        {
        //            protocolAssem = _assems[i];
        //            break;
        //        }
        //    }
        //    if (protocolAssem != null)
        //    {
        //        System.Text.StringBuilder sbuilder = new System.Text.StringBuilder();
        //        foreach (System.Type _type in protocolAssem.GetTypes())
        //        {
        //            if (builderNet.IsProtocol(_type) == false)
        //                continue;
        //            sbuilder.AppendLine(_type.ToString());
        //        }
        //        this.protocolNames = sbuilder.ToString();
        //    }
        //}
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

                GUILayout.Space(10f);

                GUI.backgroundColor = Color.white;
                showObjectInput = EditorGUILayout.Foldout(showObjectInput, "Input Tables");

                EditorGUI.indentLevel++;
                if (showObjectInput)
                {
                    int newTableCount = EditorGUILayout.IntField("Table Count", buildConfigData.inObjTables.Length);
                    if (newTableCount != buildConfigData.inObjTables.Length)
                    {
                        buildConfigData.inObjTables = resize<TextAsset>(buildConfigData.inObjTables, backupObjectInput, newTableCount);
                    }
                    for (int i = 0; i < buildConfigData.inObjTables.Length; i++)
                    {
                        buildConfigData.inObjTables[i] = (TextAsset)EditorGUILayout.ObjectField(string.Format("Table-{0}", i), buildConfigData.inObjTables[i], typeof(TextAsset));
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }

        GUI.backgroundColor = Color.white;
        if (GUILayout.Button("Compile Tables !!"))
        {
            if (buildConfigData.outObjTables.Length == 0)
            {
                EditorUtility.DisplayDialog("Compile Tables", "No Output Folder.", "Failed");
            }
            else
            {
                for (int k = 0; k < buildConfigData.outObjTables.Length; k++)
                {
                    string tempDir = System.IO.Path.Combine(Application.dataPath, buildConfigData.outObjTables[k]);
                    for (int i = 0; i < buildConfigData.inObjTables.Length; i++)
                    {
                        if (buildConfigData.inObjTables[i] == null)
                            continue;
                        builderObject.BuildFromData(buildConfigData.inObjTables[i].name, buildConfigData.inObjTables[i].text, tempDir);
                        builderData.BuildFromData(buildConfigData.inObjTables[i].name, buildConfigData.inObjTables[i].text, tempDir);
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

            GUILayout.Space(10f);

            showDataInput = EditorGUILayout.Foldout(showDataInput, "Input Tables");
            EditorGUI.indentLevel++;
            if (showDataInput)
            {
                int newTableCount = EditorGUILayout.IntField("Table Count", buildConfigData.inDataTables.Length);
                if (newTableCount != buildConfigData.inDataTables.Length)
                {
                    buildConfigData.inDataTables = resize<TextAsset>(buildConfigData.inDataTables, backupDataInput, newTableCount);
                }
                for (int i = 0; i < buildConfigData.inDataTables.Length; i++)
                {
                    buildConfigData.inDataTables[i] = (TextAsset)EditorGUILayout.ObjectField(string.Format("Table-{0}", i), buildConfigData.inDataTables[i], typeof(TextAsset));
                }
            }
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;

        //if (GUILayout.Button("Make Json Files !!"))
        //{
        //    LoadInfo[] tmpList = new LoadInfo[]
        //    { new LoadInfo("TestSchema", DataManager.Load_TestSchema_XmlData, DataManager.Save_TestSchema_JsonFile)
        //    , new LoadInfo("Character", DataManager.Load_Character_XmlData, DataManager.Save_Character_JsonFile)
        //    , new LoadInfo("Skill", DataManager.Load_Skill_XmlData, DataManager.Save_Skill_JsonFile)
        //    };
        //    DataManager.UnLoad_TestSchema();
        //    foreach (LoadInfo tmp in tmpList)
        //    {
        //        foreach (TextAsset asset in buildConfigData.inObjTables)
        //        {
        //            if (string.Compare(asset.name.ToLower(), tmp.name.ToLower()) != 0)
        //            {
        //                continue;
        //            }
        //            tmp.load(asset.text);
        //            tmp.save(System.IO.Path.Combine(Application.dataPath, string.Format("Bundles/TableJson/{0}.json", asset.name)));
        //        }
        //    }
        //    //DataManager.Save_TestSchema_JsonFile(Application.dataPath + @"/Devwinsoft/Devarc/_test/TestSchema.json");

        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh(ImportAssetOptions.Default);
        //    EditorUtility.DisplayDialog("Make Json Files", "Build Completed.", "Success");
        //}

        GUILayout.Space(20f);

        showStrConfig = EditorGUILayout.Foldout(showStrConfig, "String Tables");
        EditorGUI.indentLevel++;
        if (showStrConfig)
        {
            buildConfigData.workingPath = EditorGUILayout.TextField("Working Path", buildConfigData.workingPath);

            showStrInput = EditorGUILayout.Foldout(showStrInput, "Input Tables");
            EditorGUI.indentLevel++;
            if (showStrInput)
            {
                int newTableCount = EditorGUILayout.IntField("Table Count", buildConfigData.localizeTables.Length);
                if (newTableCount != buildConfigData.localizeTables.Length)
                {
                    buildConfigData.localizeTables = resize<TextAsset>(buildConfigData.localizeTables, backupStrInput, newTableCount);
                }
                for (int i = 0; i < buildConfigData.localizeTables.Length; i++)
                {
                    buildConfigData.localizeTables[i] = (TextAsset)EditorGUILayout.ObjectField(string.Format("Table-{0}", i), buildConfigData.localizeTables[i], typeof(TextAsset));
                }
            }
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;

        if (GUILayout.Button("Build Localized String."))
        {
            //BuildUtil util = new BuildUtil();
            //util.BuildLString(buildConfigData.workingPath, buildConfigData.localizeTables);
            //EditorUtility.DisplayDialog("Build Localized String", "Build Completed.", "Success");
        }

        GUILayout.EndScrollView();
    }
}
