using UnityEngine;
using System.Collections;

public class HurtSkill : Skill
{
    public override void SkillStart(bool isServer)
    {
        base.SkillStart(isServer);
        phase = SkillPhase.Recovery;

        var animator = player.GetComponent<PlayerGeneric>().model.GetComponent<Animator>();
        animator.SetBool("hurting", true);
    }

    public override void SkillBreak(bool isServer)
    {
        base.SkillBreak(isServer);

        var animator = player.GetComponent<PlayerGeneric>().model.GetComponent<Animator>();
        animator.SetBool("hurting", false);
    }

    public override void Process(bool isServer)
    {
        base.Process(isServer);
        if (!manager.IsInStun)
        {
            manager.SkillCancel();
        }
    }
}
