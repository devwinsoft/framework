using UnityEngine;
using System.Collections;

public class UserController : BaseController
{
	void Update()
    {
        Move(base.dir);
        base.updatePosition();
    }
}
