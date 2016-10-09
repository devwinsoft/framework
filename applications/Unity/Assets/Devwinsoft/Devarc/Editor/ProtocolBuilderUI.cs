using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Devarc;

public class ProtocolBuilderUI : EditorWindow
{
    [@MenuItem("Devarc/Build Protocols")]
    static void BakingCharacterTexture()
    {
        EditorWindow.GetWindowWithRect(typeof(ProtocolBuilderUI), new Rect(0, 0, 510f, 340f), true);
    }

    const string build_settings = "Assets/Devwinsoft/Devarc/BuildSettings.asset";
    BuilderSaveData build_info = null;

    void OnEnable()
    {
        this.titleContent.text = "Compile Protocols";
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

        int newTableCount = EditorGUILayout.IntField("Table Count", build_info.protocols.Length);
        if (newTableCount != build_info.protocols.Length)
        {
            Object[] newTableList = new Object[newTableCount];
            if (newTableCount > 0)
            {
                System.Array.Copy(build_info.protocols, newTableList, Mathf.Min(newTableCount, build_info.protocols.Length));
            }
            build_info.protocols = newTableList;
        }

        for (int i = 0; i < build_info.protocols.Length; i++)
        {
            build_info.protocols[i] = (Object)EditorGUILayout.ObjectField(string.Format("Table-{0}", i), build_info.protocols[i], typeof(Object));
        }

        if (GUILayout.Button("Compile Protocols !!"))
        {
            for (int i = 0; i < build_info.protocols.Length; i++)
            {
                Object obj = build_info.protocols[i];
                if (obj == null)
                    continue;
                const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
                MethodInfo[] methods = obj.GetType().GetMethods(flags);
                foreach (MethodInfo mInfo in methods)
                {
                    Debug.Log("Obj: " + obj.name + ", Method: " + mInfo.Name);
                }

            }
            AssetDatabase.Refresh(ImportAssetOptions.Default);
            EditorUtility.DisplayDialog("Compile Protocols", "Build Completed.", "Success");
        }
    }
}
