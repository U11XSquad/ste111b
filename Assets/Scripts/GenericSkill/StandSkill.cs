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
        base.SkillStart(isServer);
        phase = SkillPhase.Recovery;
        //var animator = Model.GetComponent<Animator>();
    }
}
