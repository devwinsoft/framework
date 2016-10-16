using UnityEngine;
using System.Collections;
using Devarc;

public class PlayerController : BaseController
{
    int input_fwd = 0;
    int input_right = 0;

    void Update()
    {
        DIRECTION curDir = _getInputDirection();
        if (base.dir != curDir)
        {
            Move(curDir);
        }
        base.updatePosition();
    }

    DIRECTION _getInputDirection()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            this.input_fwd = 2;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            this.input_fwd = -2;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            this.input_right = -2;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            this.input_right = 2;

        if ((Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow)) && this.input_fwd > 0)
            this.input_fwd = 0;
        if ((Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow)) && this.input_fwd < 0)
            this.input_fwd = 0;
        if ((Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow)) && this.input_right < 0)
            this.input_right = 0;
        if ((Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow)) && this.input_right > 0)
            this.input_right = 0;

        int fwd = this.input_fwd;
        int side = this.input_right;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            fwd++;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            fwd--;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            side--;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            side++;
        if (fwd > 0)
        {
            if (side > 0)
                return DIRECTION.FR;
            else if (side < 0)
                return DIRECTION.FL;
            else
                return DIRECTION.FORWARD;
        }
        else if (fwd < 0)
        {
            if (side > 0)
                return DIRECTION.BR;
            else if (side < 0)
                return DIRECTION.BL;
            else
                return DIRECTION.BACK;
        }
        else // (fwd==0)
        {
            if (side > 0)
                return DIRECTION.R;
            else if (side < 0)
                return DIRECTION.L;
            else
                return DIRECTION.STOP;
        }
    }
}