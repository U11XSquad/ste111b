using UnityEngine;
using System.Collections;

public class MoveSkill : Skill
{
    [Tooltip("最大移速，单位m/s^2")]
    public float MoveSpeed = 5.0f;

    [Tooltip("最大主动加速度，单位m/s^2")]
    public float MaxAccelerate = 60.0f;

    public override bool InputDetermine()
    {
        return input.IsMoving;
    }

    public override void SkillStart(bool isServer)
    {
        base.SkillStart(isServer);
        phase = SkillPhase.Recovery;

        var animator = player.GetComponent<PlayerGeneric>().model.GetComponent<Animator>();
        animator.SetBool("walking", true);
    }

    public override void SkillBreak(bool isServer)
    {
        base.SkillBreak(isServer);

        var animator = player.GetComponent<PlayerGeneric>().model.GetComponent<Animator>();
        animator.SetBool("walking", false);
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
            playerGeneric.FaceTo(input.Move);
            //移动
            DealMove();
        }
        else
        {
            manager.SkillCancel();
        }
    }

    void DealMove()
    {
        var rigid = player.GetComponent<Rigidbody>();
        var velocity = rigid.velocity;
        var moveDir = input.Move;
        moveDir.Normalize();
        moveDir = moveDir * MoveSpeed;
        var velDiff = moveDir - velocity;
        var accLength = velDiff.magnitude;
        var maxAcc = MaxAccelerate * Time.fixedDeltaTime;
        accLength = Mathf.Clamp(accLength, -maxAcc, maxAcc);
        velDiff.Normalize();
        velDiff *= accLength;
        rigid.AddForce(velDiff, ForceMode.VelocityChange);
    }
}
