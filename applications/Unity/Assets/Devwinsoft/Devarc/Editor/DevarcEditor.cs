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
        EditorWindow.GetWindowWithRect(typeof(DevarcEditor), new Rect(0, 0, 540f, 500f), false);
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
            GUI.backgroundColor = Color.cyan;
            GUILayout.TextField("Object Tables");
            GUI.backgroundColor = Color.white;
            int newDirCount = EditorGUILayout.IntField("Output Directory Count", buildConfigData.outObjTables.Length);
            if (newDirCount != buildConfigData.outObjTables.Length)
            {
                string[] newTableList = new string[newDirCount];
                if (newDirCount > 0)
                {
                    System.Array.Copy(buildConfigData.outObjTables, newTableList, Mathf.Min(newDirCount, buildConfigData.outObjTables.Length));
                }
                buildConfigData.outObjTables = newTableList;
            }
            for (int i = 0; i < buildConfigData.outObjTables.Length; i++)
            {
                buildConfigData.outObjTables[i] = EditorGUILayout.TextField(string.Format("Output Directory-{0}", i), buildConfigData.outObjTables[i]);
            }

            GUILayout.Space(10f);

            int newTableCount = EditorGUILayout.IntField("Table Count", buildConfigData.inObjTables.Length);
            if (newTableCount != buildConfigData.inObjTables.Length)
            {
                TextAsset[] newTables = new TextAsset[newTableCount];
                if (newTableCount > 0)
                {
                    System.Array.Copy(buildConfigData.inObjTables, newTables, Mathf.Min(newTableCount, buildConfigData.inObjTables.Length));
                }
                buildConfigData.inObjTables = newTables;
            }
            for (int i = 0; i < buildConfigData.inObjTables.Length; i++)
            {
                buildConfigData.inObjTables[i] = (TextAsset)EditorGUILayout.ObjectField(string.Format("Table-{0}", i), buildConfigData.inObjTables[i], typeof(TextAsset));
            }

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
                    EditorUtility.SetDirty(buildConfigData);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh(ImportAssetOptions.Default);
                    EditorUtility.DisplayDialog("Compile Tables", "Build Completed.", "Success");
                }
            }
        }

        GUILayout.Space(20f);

        {
            GUI.backgroundColor = Color.cyan;
            GUILayout.TextField("Data Tables");
            GUI.backgroundColor = Color.white;
            int newDirCount = EditorGUILayout.IntField("Output Directory Count", buildConfigData.outDataTables.Length);
            if (newDirCount != buildConfigData.outDataTables.Length)
            {
                string[] newTableList = new string[newDirCount];
                if (newDirCount > 0)
                {
                    System.Array.Copy(buildConfigData.outDataTables, newTableList, Mathf.Min(newDirCount, buildConfigData.outDataTables.Length));
                }
                buildConfigData.outDataTables = newTableList;
            }
            for (int i = 0; i < buildConfigData.outDataTables.Length; i++)
            {
                buildConfigData.outDataTables[i] = EditorGUILayout.TextField(string.Format("Output Directory-{0}", i), buildConfigData.outDataTables[i]);
            }

            GUILayout.Space(10f);
            int newTableCount = EditorGUILayout.IntField("Table Count", buildConfigData.inDataTables.Length);
            if (newTableCount != buildConfigData.inDataTables.Length)
            {
                TextAsset[] newTableList = new TextAsset[newTableCount];
                if (newTableCount > 0)
                {
                    System.Array.Copy(buildConfigData.inDataTables, newTableList, Mathf.Min(newTableCount, buildConfigData.inDataTables.Length));
                }
                buildConfigData.inDataTables = newTableList;
            }
            for (int i = 0; i < buildConfigData.inDataTables.Length; i++)
            {
                buildConfigData.inDataTables[i] = (TextAsset)EditorGUILayout.ObjectField(string.Format("Table-{0}", i), buildConfigData.inDataTables[i], typeof(TextAsset));
            }

            if (GUILayout.Button("Make Json Files !!"))
            {
                DataManager.UnLoad_TestSchema();
                foreach (TextAsset asset in buildConfigData.inDataTables)
                {
                    DataManager.Load_TestSchema_XmlData(asset.text);
                }
                DataManager.Save_TestSchema_JsonFile(Application.dataPath + @"/Devwinsoft/Devarc/_test/TestSchema.json");
                EditorUtility.SetDirty(buildConfigData);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.Default);
                EditorUtility.DisplayDialog("Make Json Files", "Build Completed.", "Success");
            }
        }

        GUILayout.Space(20f);

        if (protocolAssem != null)
        {
            GUI.backgroundColor = Color.cyan;
            GUILayout.TextField("Protocols");
            GUI.backgroundColor = Color.white;
            int newProtocols = EditorGUILayout.IntField("Output Directory Count", buildConfigData.outProtocols.Length);
            if (newProtocols != buildConfigData.outProtocols.Length)
            {
                string[] newTableList = new string[newProtocols];
                if (newProtocols > 0)
                {
                    System.Array.Copy(buildConfigData.outProtocols, newTableList, Mathf.Min(newProtocols, buildConfigData.outProtocols.Length));
                }
                buildConfigData.outProtocols = newTableList;
            }
            for (int i = 0; i < buildConfigData.outProtocols.Length; i++)
            {
                buildConfigData.outProtocols[i] = EditorGUILayout.TextField(string.Format("Output Directory-{0}", i), buildConfigData.outProtocols[i]);
            }
            GUILayout.TextArea(this.protocolNames);
            if (GUILayout.Button("Compile Protocols !!"))
            {
                for (int i = 0; i < buildConfigData.outProtocols.Length; i++)
                {
                    string tempDir = System.IO.Path.Combine(Application.dataPath, buildConfigData.outProtocols[i]);
                    if (System.IO.Directory.Exists(tempDir) == false)
                        continue;
                    builderNet.BuildFromAssem(protocolAssem, tempDir);
                }
                EditorUtility.SetDirty(buildConfigData);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.Default);
                EditorUtility.DisplayDialog("Compile Protocols", "Build Completed.", "Success");
            }
        }

    }
}
