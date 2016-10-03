using UnityEngine;
using System.Collections;

public class StandSkill : Skill
{
    public override bool InputDetermine()
    {
        return true;
    }

    public override void SkillStart(bool isServer)
    {
        var animator = player.GetComponent<PlayerGeneric>().model.GetComponent<Animator>();
        animator.SetBool("walking", false);
        Debug.Log(isServer);
    }
}
