using UnityEngine;
using System.Collections;

public class DeathSkill : Skill
{
    public override void SkillStart(bool isServer)
    {
        base.SkillStart(isServer);
        phase = SkillPhase.Recovery;

        var animator = Model.GetComponent<Animator>();
        animator.SetBool("dead", true);
    }

    public override void SkillBreak(bool isServer)
    {
        base.SkillBreak(isServer);

        var animator = Model.GetComponent<Animator>();
        animator.SetBool("dead", false);
    }
}
