using UnityEngine;
using System.Collections;

public class MoveSkill : Skill
{
    [Tooltip("移动速度，单位m/s")]
    public float MoveSpeed = 5.0f;

    Vector3 preMovSpd;

    public override bool InputDetermine()
    {
        return input.IsMoving;
    }

    public override void SkillStart(bool isServer)
    {
        base.SkillStart(isServer);
        phase = SkillPhase.Recovery;

        var animator = Model.GetComponent<Animator>();
        animator.SetBool("walking", true);

        //创建初始的移动
        var movSpd = input.Move;
        preMovSpd = input.Move;
        movSpd.Normalize();
        movSpd *= MoveSpeed;
        player.GetComponent<MovingGeneric>().AddDisplaceBySpeed(movSpd, Mathf.Infinity, this);
    }

    public override void SkillBreak(bool isServer)
    {
        base.SkillBreak(isServer);

        var animator = Model.GetComponent<Animator>();
        animator.SetBool("walking", false);

        //删除移动
        player.GetComponent<MovingGeneric>().RemoveDisplace(this);

    }

    public override void Process(bool isServer)
    {
        base.Process(isServer);

        var playerGeneric = player.GetComponent<PlayerGeneric>();
        if (!playerGeneric.isLocalPlayer)
        {
            return; //非本地玩家不检测输入
        }

        if (input.IsMoving)
        {
            //转向
            playerGeneric.GetComponent<MovingGeneric>().FaceTo(input.Move);
            //移动
            var movSpd = input.Move;
            if (movSpd != preMovSpd)
            {
                //换向则更改
                player.GetComponent<MovingGeneric>().RemoveDisplace(this);
                movSpd.Normalize();
                movSpd *= MoveSpeed;
                player.GetComponent<MovingGeneric>().AddDisplaceBySpeed(movSpd, Mathf.Infinity, this);
                preMovSpd = movSpd;
            }
        }
        else
        {
            manager.SkillCancel();
        }
    }
}
