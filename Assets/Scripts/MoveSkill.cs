using UnityEngine;
using System.Collections;

public class MoveSkill : Skill
{
    public float MoveSpeed = 5.0f;

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
        if (input.IsMoving)
        {
            //转向
            playerGeneric.FaceTo(input.Move);
            //移动
            playerGeneric.transform.Translate(Vector3.forward * MoveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            SkillBreak(isServer);
        }
    }
}
