using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Devarc;

public class DevarcEditor : EditorWindow
{
    [@MenuItem("Window/Devarc Framework")]
    static void BakingCharacterTexture()
    {
        EditorWindow.GetWindowWithRect(typeof(DevarcEditor), new Rect(0, 0, 510f, 400f), false);
    }

    const string buildConfigPath = "Assets/Devwinsoft/Devarc/BuildSettings.asset";
    BuilderSaveData buildConfigData = null;
    Assembly protocolAssem = null;
    string protocolNames = "";

    Builder_Object builderObject = new Builder_Object();
    Builder_Data builderData = new Builder_Data();
    Builder_Net builderNet = new Builder_Net();

    void OnEnable()
    {
        this.titleContent.text = "Devarc";
    }

    void OnGUI()
    {
        if (buildConfigData == null)
        {
            buildConfigData = (BuilderSaveData)AssetDatabase.LoadAssetAtPath(buildConfigPath, typeof(BuilderSaveData));
        }

        if (buildConfigData == null)
        {
            if (GUILayout.Button("Init"))
            {
                BuilderSaveData asset = new BuilderSaveData();
                AssetDatabase.CreateAsset(asset, buildConfigPath);
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;
            }
            buildConfigData = (BuilderSaveData)AssetDatabase.LoadAssetAtPath(buildConfigPath, typeof(BuilderSaveData));
        }

        if (protocolAssem == null)
        {
            Assembly[] _assems = System.AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < _assems.Length && protocolAssem == null; i++)
            {
                if (_assems[i].FullName.Contains("Assembly")
                    && _assems[i].FullName.Contains("Editor"))
                {
                    protocolAssem = _assems[i];
                    break;
                }
            }
            if (protocolAssem != null)
            {
                System.Text.StringBuilder sbuilder = new System.Text.StringBuilder();
                foreach (System.Type _type in protocolAssem.GetTypes())
                {
                    if (builderNet.IsProtocol(_type) == false)
                        continue;
                    sbuilder.AppendLine(_type.ToString());
                }
                this.protocolNames = sbuilder.ToString();
            }
        }

        if (buildConfigData == null)
        {
            return;
        }

        {
            GUILayout.Label("Object Tables");
            int newTableCount = EditorGUILayout.IntField("Table Count", buildConfigData.tables.Length);
            if (newTableCount != buildConfigData.tables.Length)
            {
                TextAsset[] newTables = new TextAsset[newTableCount];
                if (newTableCount > 0)
                {
                    System.Array.Copy(buildConfigData.tables, newTables, Mathf.Min(newTableCount, buildConfigData.tables.Length));
                }
                buildConfigData.tables = newTables;
            }
            for (int i = 0; i < buildConfigData.tables.Length; i++)
            {
                buildConfigData.tables[i] = (TextAsset)EditorGUILayout.ObjectField(string.Format("Table-{0}", i), buildConfigData.tables[i], typeof(TextAsset));
            }

            if (GUILayout.Button("Compile Tables !!"))
            {
                for (int i = 0; i < buildConfigData.tables.Length; i++)
                {
                    if (buildConfigData.tables[i] == null)
                        continue;
                    builderObject.BuildFromData(buildConfigData.tables[i].name, buildConfigData.tables[i].text, Application.dataPath + @"/Devwinsoft/Devarc/_GeneratedCode");
                    builderData.BuildFromData(buildConfigData.tables[i].name, buildConfigData.tables[i].text, Application.dataPath + @"/Devwinsoft/Devarc/_GeneratedCode");
                }
                AssetDatabase.Refresh(ImportAssetOptions.Default);
                EditorUtility.DisplayDialog("Compile Tables", "Build Completed.", "Success");
            }
        }

        GUILayout.Space(20f);

        {
            GUILayout.Label("Data Tables");
            int newTableCount = EditorGUILayout.IntField("Table Count", buildConfigData.data_tables.Length);
            if (newTableCount != buildConfigData.data_tables.Length)
            {
                TextAsset[] newTableList = new TextAsset[newTableCount];
                if (newTableCount > 0)
                {
                    System.Array.Copy(buildConfigData.data_tables, newTableList, Mathf.Min(newTableCount, buildConfigData.data_tables.Length));
                }
                buildConfigData.data_tables = newTableList;
            }
            for (int i = 0; i < buildConfigData.data_tables.Length; i++)
            {
                buildConfigData.data_tables[i] = (TextAsset)EditorGUILayout.ObjectField(string.Format("Table-{0}", i), buildConfigData.data_tables[i], typeof(TextAsset));
            }

            if (GUILayout.Button("Make Json Files !!"))
            {
                DataManager.UnLoad_TestSchema();
                foreach (TextAsset asset in buildConfigData.data_tables)
                {
                    DataManager.Load_TestSchema_XmlData(asset.text);
                }
                DataManager.Save_TestSchema_JsonFile(Application.dataPath + @"/Devwinsoft/Devarc/_test/TestSchema.json");
                AssetDatabase.Refresh(ImportAssetOptions.Default);
                EditorUtility.DisplayDialog("Make Json Files", "Build Completed.", "Success");
            }
        }

        GUILayout.Space(20f);

        if (protocolAssem != null)
        {
            GUILayout.Label("Protocols");
            GUILayout.TextArea(this.protocolNames);
            if (GUILayout.Button("Compile Protocols !!"))
            {
                builderNet.BuildFromAssem(protocolAssem, Application.dataPath + @"/Devwinsoft/Devarc/_GeneratedCode");
                AssetDatabase.Refresh(ImportAssetOptions.Default);
                EditorUtility.DisplayDialog("Compile Protocols", "Build Completed.", "Success");
            }
        }
    }
}
