using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BuilderSaveData : ScriptableObject
{
    public TextAsset[] inObjTables = new TextAsset[0];
    public string[] outObjTables = new string[0];

    public TextAsset[] inDataTables = new TextAsset[0];
    public string[] outDataTables = new string[0];

    public string workingPath = "";
    public TextAsset[] localizeTables = new TextAsset[0];
}
