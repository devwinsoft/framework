using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Devarc;

public class UITableBuilder : EditorWindow
{
    [@MenuItem("Window/Devarc/Builder")]
    static void BakingCharacterTexture()
    {
        EditorWindow.GetWindowWithRect(typeof(UITableBuilder), new Rect(0, 0, 510f, 400f), false);
    }

    const string buildSettings = "Assets/Devwinsoft/Devarc/BuildSettings.asset";
    BuilderSaveData buildInfo = null;
    Assembly protocolAssem = null;
    string protocolNames = "";

    Builder_Object builderObject = new Builder_Object();
    Builder_Data builderData = new Builder_Data();
    Builder_Net builderNet = new Builder_Net();

    void OnEnable()
    {
        this.titleContent.text = "Builder";
    }

    void OnGUI()
    {
        if (buildInfo == null)
        {
            buildInfo = (BuilderSaveData)AssetDatabase.LoadAssetAtPath(buildSettings, typeof(BuilderSaveData));
        }

        if (buildInfo == null)
        {
            if (GUILayout.Button("Init"))
            {
                BuilderSaveData asset = new BuilderSaveData();
                AssetDatabase.CreateAsset(asset, buildSettings);
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;
            }
            buildInfo = (BuilderSaveData)AssetDatabase.LoadAssetAtPath(buildSettings, typeof(BuilderSaveData));
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

        if (buildInfo == null)
        {
            return;
        }

        {
            GUILayout.Label("Object Tables");
            int newTableCount = EditorGUILayout.IntField("Table Count", buildInfo.tables.Length);
            if (newTableCount != buildInfo.tables.Length)
            {
                TextAsset[] newTables = new TextAsset[newTableCount];
                if (newTableCount > 0)
                {
                    System.Array.Copy(buildInfo.tables, newTables, Mathf.Min(newTableCount, buildInfo.tables.Length));
                }
                buildInfo.tables = newTables;
            }
            for (int i = 0; i < buildInfo.tables.Length; i++)
            {
                buildInfo.tables[i] = (TextAsset)EditorGUILayout.ObjectField(string.Format("Table-{0}", i), buildInfo.tables[i], typeof(TextAsset));
            }

            if (GUILayout.Button("Compile Tables !!"))
            {
                for (int i = 0; i < buildInfo.tables.Length; i++)
                {
                    if (buildInfo.tables[i] == null)
                        continue;
                    builderObject.BuildFromData(buildInfo.tables[i].name, buildInfo.tables[i].text, Application.dataPath + @"/Devwinsoft/Devarc/_GeneratedCode");
                    builderData.BuildFromData(buildInfo.tables[i].name, buildInfo.tables[i].text, Application.dataPath + @"/Devwinsoft/Devarc/_GeneratedCode");
                }
                AssetDatabase.Refresh(ImportAssetOptions.Default);
                EditorUtility.DisplayDialog("Compile Tables", "Build Completed.", "Success");
            }
        }

        GUILayout.Space(20f);

        {
            GUILayout.Label("Data Tables");
            int newTableCount = EditorGUILayout.IntField("Table Count", buildInfo.data_tables.Length);
            if (newTableCount != buildInfo.data_tables.Length)
            {
                TextAsset[] newTableList = new TextAsset[newTableCount];
                if (newTableCount > 0)
                {
                    System.Array.Copy(buildInfo.data_tables, newTableList, Mathf.Min(newTableCount, buildInfo.data_tables.Length));
                }
                buildInfo.data_tables = newTableList;
            }
            for (int i = 0; i < buildInfo.data_tables.Length; i++)
            {
                buildInfo.data_tables[i] = (TextAsset)EditorGUILayout.ObjectField(string.Format("Table-{0}", i), buildInfo.data_tables[i], typeof(TextAsset));
            }

            if (GUILayout.Button("Make Json Files !!"))
            {
                TableData.UnLoad_TestSchema();
                foreach (TextAsset asset in buildInfo.data_tables)
                {
                    TableData.Load_TestSchema_XmlData(asset.text);
                }
                TableData.Save_TestSchema_JsonFile(Application.dataPath + @"/Devwinsoft/Devarc/_test/TestSchema.json");
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
