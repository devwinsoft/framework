using UnityEngine;
using System.Collections;
using Devarc;

public class PlayerObject : MonoBehaviour
{
    public BaseController Controller;
    public DataPlayer Data;

    public void Init(DataPlayer _data)
    {
        Data.Initialize(_data);
    }

	void Start () {
	
	}

    void Update () {
	
	}
}
