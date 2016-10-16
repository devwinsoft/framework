using UnityEngine;
using System.Collections;
using Devarc;


public class BaseController : MonoBehaviour
{
    static Vector3[] directions = new Vector3[]
    { Vector3.zero
    , new Vector3(0, 0, -1f)
    , new Vector3(0, 0, 1f)
    , new Vector3(-1, 0, 0)
    , new Vector3(1, 0, 0)
    , new Vector3(-Mathf.Sqrt(0.5f), 0, Mathf.Sqrt(0.5f))
    , new Vector3(Mathf.Sqrt(0.5f), 0, Mathf.Sqrt(0.5f))
    , new Vector3(-Mathf.Sqrt(0.5f), 0, -Mathf.Sqrt(0.5f))
    , new Vector3(Mathf.Sqrt(0.5f), 0, -Mathf.Sqrt(0.5f))
    };

    public Transform character;
    protected DIRECTION dir;

    public void Move(DIRECTION _curDir)
    {
        this.dir = _curDir;
        switch (_curDir)
        {
            case DIRECTION.FORWARD:
                character.localRotation = Quaternion.identity;
                break;
            case DIRECTION.BACK:
                character.localRotation = Quaternion.Euler(0f, -180f, 0f);
                break;
            case DIRECTION.L:
                character.localRotation = Quaternion.Euler(0f, -90f, 0f);
                break;
            case DIRECTION.R:
                character.localRotation = Quaternion.Euler(0f, 90f, 0f);
                break;
            case DIRECTION.FL:
                character.localRotation = Quaternion.Euler(0f, -45f, 0f);
                break;
            case DIRECTION.FR:
                character.localRotation = Quaternion.Euler(0f, 45f, 0f);
                break;
            case DIRECTION.BL:
                character.localRotation = Quaternion.Euler(0f, -135f, 0f);
                break;
            case DIRECTION.BR:
                character.localRotation = Quaternion.Euler(0f, 135f, 0f);
                break;
            case DIRECTION.STOP:
            default:
                break;
        }
    }

    protected void updatePosition()
    {
        Vector3 delta = 0.1f * directions[(int)this.dir];
        transform.position += delta;
    }
}
