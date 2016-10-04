﻿using UnityEngine;
using System.Collections;

public class BlockSkill : Skill
{
    public override bool InputDetermine()
    {
        return input.BlockHold;
    }

    public override void SkillStart(bool isServer)
    {
        base.SkillStart(isServer);
        phase = SkillPhase.Recovery;

        var animator = player.GetComponent<PlayerGeneric>().model.GetComponent<Animator>();
        animator.SetBool("blocking", true);
    }

    public override void SkillBreak(bool isServer)
    {
        base.SkillBreak(isServer);

        var animator = player.GetComponent<PlayerGeneric>().model.GetComponent<Animator>();
        animator.SetBool("blocking", false);
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
        }
        if (!input.BlockHold)
        {
            manager.SkillCancel();
        }
    }

    public override SkillType OnHit(bool isServer, GameObject hitBox)
    {
        //Block是长期处于后摇中的技能，所以要求全程防御
        return SkillType.Block;
    }

    public override bool BlockSucceed(bool isServer, GameObject opponent)
    {
        //按防御计算伤害
        return true;
    }
}
