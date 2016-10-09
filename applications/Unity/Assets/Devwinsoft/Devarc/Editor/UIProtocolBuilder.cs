using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Devarc;

public class UIProtocolBuilder : EditorWindow
{
    [@MenuItem("Devarc/Build Protocols")]
    static void BakingCharacterTexture()
    {
        EditorWindow.GetWindowWithRect(typeof(UIProtocolBuilder), new Rect(0, 0, 510f, 340f), true);
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

        int newTableCount = EditorGUILayout.IntField("Protocol Count", build_info.protocols.Length);
        if (newTableCount != build_info.protocols.Length)
        {
            Object[] newTableList = new Object[newTableCount];
            if (newTableCount > 0)
            {
                System.Array.Copy(build_info.protocols, newTableList, Mathf.Min(newTableCount, build_info.protocols.Length));
            }
            build_info.protocols = newTableList;
        }

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
