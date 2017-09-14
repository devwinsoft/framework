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

[XmlRoot("MonsterCollection")]
public class MonsterContainer
{
    //[XmlArray("Monsters")]
    //[XmlArrayItem("Monster")]
    //public List<Monster> Monsters = new List<Monster>();
    [XmlArray("Monsters"), XmlArrayItem("Monster")]
    public Monster[] Monsters;
}
public class Monster
{
    [XmlAttribute("Name")]
    public string Name;

    public int Health;
}

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

        if (GUILayout.Button("test"))
        {
            MonsterContainer data = new MonsterContainer();
            data.Monsters = new Monster[] { new Monster() };
            var serializer = new XmlSerializer(typeof(MonsterContainer));
            using (var stream = new FileStream(Path.Combine(Application.dataPath, "test.xml"), FileMode.Create))
            {
                serializer.Serialize(stream, data);
            }
        }

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
        if (GUILayout.Button("Make Json Files !!"))
        {
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
                System.Type _type = typeof(TableManager);
                foreach (MethodInfo m in _type.GetMethods())
                {
                    if (m.Name.StartsWith("UnLoad"))
                    {
                        m.Invoke(null, null);
                    }
                }

                List<string> tmpSaveList = new List<string>();
                foreach (TextAsset asset in buildConfigData.inObjTables)
                {
                    string tmpTableName = _get_table_name(asset.name);
                    string tmpMethodName = string.Format("Load_{0}_XmlData", tmpTableName);
                    foreach (MethodInfo m in _type.GetMethods())
                    {
                        if (m.Name.Equals(tmpMethodName))
                        {
                            m.Invoke(null, new object[] { asset.text });
                        }
                    }
                    if (tmpSaveList.Contains(tmpTableName) == false)
                        tmpSaveList.Add(tmpTableName);
                }
                foreach (string tmpTableName in tmpSaveList)
                {
                    for (int i = 0; i < buildConfigData.outDataTables.Length; i++)
                    {
                        string subPath = System.IO.Path.Combine(buildConfigData.outDataTables[i], tmpTableName + ".json");
                        string savePath = System.IO.Path.Combine(Application.dataPath, subPath);
                        string tmpMethodName = string.Format("Save_{0}_JsonFile", tmpTableName);
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

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.Default);
            EditorUtility.DisplayDialog("Make Json Files", "Build Completed.", "Success");
        }

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
            BuildUtil util = new BuildUtil();
            util.BuildLString(buildConfigData.workingPath, buildConfigData.localizeTables);
            EditorUtility.DisplayDialog("Build Localized String", "Build Completed.", "Success");
        }

        GUILayout.EndScrollView();
    }
}
