using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BuilderSaveData : ScriptableObject
{
    public string[] inObjFiles = new string[0];
    public string[] outObjTables = new string[0];

    public string[] inStrTables = new string[0];
    public string outStrDir = "";

    public string[] inDataFiles = new string[0];
    public string[] outDataTables = new string[0];

    public string[] inSQLiteFiles = new string[0];
    public string outSQLitePath = "";

}
