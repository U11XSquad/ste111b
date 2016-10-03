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

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        //获取按键信息
        LeftHold = Input.GetButton("Left");
        RightHold = Input.GetButton("Right");
        UpHold = Input.GetButton("Up");
        DownHold = Input.GetButton("Down");

        LHold = Input.GetButton("L");
        LTrigger = Input.GetButtonDown("L");
        NHold = Input.GetButton("N");
        NTrigger = Input.GetButtonDown("N");
        HHold = Input.GetButton("H");
        HTrigger = Input.GetButtonDown("H");
        SHold = Input.GetButton("S");
        STrigger = Input.GetButtonDown("S");

        //修正移动值
        AdjustMove();
    }

    void AdjustMove()
    {
        if (LeftHold && !RightHold)
        {
            moveDir.x = -1.0f;
        }
        else if (RightHold && !LeftHold)
        {
            moveDir.x = 1.0f;
        }
        else
        {
            moveDir.x = 0.0f;
        }
        if (UpHold && !DownHold)
        {
            moveDir.y = 1.0f;
        }
        else if (DownHold && !UpHold)
        {
            moveDir.y = -1.0f;
        }
        else
        {
            moveDir.y = 0.0f;
        }
        moveDir.z = 0.0f;
    }
}
