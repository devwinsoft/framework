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

    const string build_settings = "Assets/Devwinsoft/Devarc/BuildSettings.asset";
    BuilderSaveData build_info = null;

    void OnEnable()
    {
        this.titleContent.text = "Builder";
    }

    void OnGUI()
    {
        if (build_info == null)
        {
            build_info = (BuilderSaveData)AssetDatabase.LoadAssetAtPath(build_settings, typeof(BuilderSaveData));
        }

        if (build_info == null)
        {
            if (GUILayout.Button("Init"))
            {
                BuilderSaveData asset = new BuilderSaveData();
                AssetDatabase.CreateAsset(asset, build_settings);
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;
            }
            build_info = (BuilderSaveData)AssetDatabase.LoadAssetAtPath(build_settings, typeof(BuilderSaveData));
        }

        if (build_info == null)
        {
            return;
        }

        {
            GUILayout.Label("Object Tables");
            int newTableCount = EditorGUILayout.IntField("Table Count", build_info.tables.Length);
            if (newTableCount != build_info.tables.Length)
            {
                TextAsset[] newTables = new TextAsset[newTableCount];
                if (newTableCount > 0)
                {
                    System.Array.Copy(build_info.tables, newTables, Mathf.Min(newTableCount, build_info.tables.Length));
                }
                build_info.tables = newTables;
            }
            for (int i = 0; i < build_info.tables.Length; i++)
            {
                build_info.tables[i] = (TextAsset)EditorGUILayout.ObjectField(string.Format("Table-{0}", i), build_info.tables[i], typeof(TextAsset));
            }

            if (GUILayout.Button("Compile Tables !!"))
            {
                Builder_Object builder1 = new Builder_Object();
                Builder_Data builder2 = new Builder_Data();
                for (int i = 0; i < build_info.tables.Length; i++)
                {
                    if (build_info.tables[i] == null)
                        continue;
                    builder1.BuildFromData(build_info.tables[i].name, build_info.tables[i].text, Application.dataPath + @"/Devwinsoft/Devarc/_GeneratedCode");
                    builder2.BuildFromData(build_info.tables[i].name, build_info.tables[i].text, Application.dataPath + @"/Devwinsoft/Devarc/_GeneratedCode");
                }
                AssetDatabase.Refresh(ImportAssetOptions.Default);
                EditorUtility.DisplayDialog("Compile Tables", "Build Completed.", "Success");
            }
        }

        GUILayout.Space(20f);

        {
            GUILayout.Label("Data Tables");
            int newTableCount = EditorGUILayout.IntField("Table Count", build_info.data_tables.Length);
            if (newTableCount != build_info.data_tables.Length)
            {
                TextAsset[] newTableList = new TextAsset[newTableCount];
                if (newTableCount > 0)
                {
                    System.Array.Copy(build_info.data_tables, newTableList, Mathf.Min(newTableCount, build_info.data_tables.Length));
                }
                build_info.data_tables = newTableList;
            }
            for (int i = 0; i < build_info.data_tables.Length; i++)
            {
                build_info.data_tables[i] = (TextAsset)EditorGUILayout.ObjectField(string.Format("Table-{0}", i), build_info.data_tables[i], typeof(TextAsset));
            }

            if (GUILayout.Button("Make Json Files !!"))
            {
                TableData.UnLoad_TestSchema();
                foreach (TextAsset asset in build_info.data_tables)
                {
                    TableData.Load_TestSchema_XmlData(asset.text);
                }
                TableData.Save_TestSchema_JsonFile(Application.dataPath + @"/Devwinsoft/Devarc/_test/TestSchema.json");
                AssetDatabase.Refresh(ImportAssetOptions.Default);
                EditorUtility.DisplayDialog("Make Json Files", "Build Completed.", "Success");
            }
        }

        GUILayout.Space(20f);

        if (GUILayout.Button("Compile Protocols !!"))
        {
            Builder_Net builder1 = new Builder_Net();

            // search dll
            Assembly[] _assems = System.AppDomain.CurrentDomain.GetAssemblies();
            Assembly _assem = null;
            for (int i = 0; i < _assems.Length && _assem == null; i++)
            {
                foreach (System.Type tp in _assems[i].GetTypes())
                {
                    if (_assems[i].FullName.Contains("Assembly")
                        && _assems[i].FullName.Contains("Editor"))
                    {
                        _assem = _assems[i];
                        break;
                    }
                }
            }

            if (_assem == null)
            {
                EditorUtility.DisplayDialog("Compile Protocols", "Build Failed.", "Failed");
            }
            else
            {
                builder1.BuildFromAssem(_assem, Application.dataPath + @"/Devwinsoft/Devarc/_GeneratedCode");
                AssetDatabase.Refresh(ImportAssetOptions.Default);
                EditorUtility.DisplayDialog("Compile Protocols", "Build Completed.", "Success");
            }
        }
    }
}
