using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Devarc;

public class UITableBuilder : EditorWindow
{
    [@MenuItem("Devarc/Build Tables")]
    static void BakingCharacterTexture()
    {
        EditorWindow.GetWindowWithRect(typeof(UITableBuilder), new Rect(0, 0, 510f, 340f), true);
    }

    const string build_settings = "Assets/Devwinsoft/Devarc/BuildSettings.asset";
    BuilderSaveData build_info = null;

    void OnEnable()
    {
        this.titleContent.text = "Compile Tables";
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
}
