using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class BuildData : System.Object
{
    public string[] inFiles = new string[0];
    public string outFile = "";

    [NonSerialized]
    public List<string> backupFiles = new List<string>();
}


public class BuilderSaveData : ScriptableObject
{
    public string[] inObjFiles = new string[0];
    public string[] outObjTables = new string[0];

    public string[] inStrTables = new string[0];
    public string outStrDir = "";

    public string[] inDataFiles = new string[0];
    public string[] outDataTables = new string[0];

    public BuildData[] dataSQLite = new BuildData[0];
}
