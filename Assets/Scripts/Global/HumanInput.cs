using UnityEngine;
using System.Collections;

public class HumanInput : InputCtrl
{
    private Vector3 moveDir;
    public override Vector3 Move
    {
        get
        {
            return moveDir;
        }
        protected set
        {
            moveDir = value;
        }
    }

    void Start()
    {

    }

    void GetKeyState(string keyName, ref bool hold, ref bool down)
    {
        //因为GetButtonDown是给Update用的，所以FixedUpdate不准
        var holdNow = Input.GetButton(keyName);
        down = !hold && holdNow;
        hold = holdNow;
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        //获取按键信息
        LeftHold = Input.GetButton("Left");
        RightHold = Input.GetButton("Right");
        UpHold = Input.GetButton("Up");
        DownHold = Input.GetButton("Down");

        GetKeyState("L", ref lhold, ref ltrigger);
        GetKeyState("N", ref nhold, ref ntrigger);
        GetKeyState("H", ref hhold, ref htrigger);
        GetKeyState("S", ref shold, ref strigger);
        GetKeyState("Block", ref bhold, ref btrigger);

        //修正移动值
        AdjustMove();

        //基类的按键记录
        RecordKey();
    }

    void AdjustMove()
    {
        IsMoving = false;
        if (LeftHold && !RightHold)
        {
            moveDir.x = -1.0f;
            IsMoving = true;
        }
        else if (RightHold && !LeftHold)
        {
            moveDir.x = 1.0f;
            IsMoving = true;
        }
        else
        {
            moveDir.x = 0.0f;
        }
        if (UpHold && !DownHold)
        {
            moveDir.z = 1.0f;
            IsMoving = true;
        }
        else if (DownHold && !UpHold)
        {
            moveDir.z = -1.0f;
            IsMoving = true;
        }
        else
        {
            moveDir.z = 0.0f;
        }
        moveDir.y = 0.0f;
    }
}
