using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class SkillManager : NetworkBehaviour
{
    /// <summary>
    /// 所有的技能
    /// </summary>
    /// <remarks>
    /// 固定技能如下：
    /// <list type="bullet">
    /// <item><term>0</term><description>通常站立</description></item>
    /// <item><term>1</term><description>通常移动</description></item>
    /// <item><term>2</term><description>通常防御</description></item>
    /// </list>
    /// </remarks>
    public Skill[] skills;

    /// <summary>
    /// 当前施放的技能
    /// </summary>
    public Skill current = null;

    /// <summary>
    /// 后摇取消后接的技能，参见<seealso cref="Skill.GenericInterruptable"/>
    /// </summary>
    public Skill nextSkill = null;

    void ReturnToStand()
    {
        current = skills[0];
        current.SkillStart(isServer);
        nextSkill = null;
    }

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].SkillNo = i;
        }
        ReturnToStand();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!current)
        {
            return;
        }

        //当前技能的处理
        current.Process(isServer);
        if (current.status != Skill.SkillStatus.Active)
        {
            ReturnToStand();
        }

        //技能输入检测
        foreach (Skill skill in skills)
        {
            if (skill.status != Skill.SkillStatus.Disable &&
                skill.InterruptPriority > current.InterruptPriority &&
                skill.InputDetermine())
            {
                nextSkill = skill;
            }
        }

        //技能通常打断
        if (nextSkill &&
            current.GenericInterruptable &&
            current.phase == Skill.SkillPhase.Recovery)
        {
            CmdCast(nextSkill.SkillNo);
        }

        //出招取消
        //TODO
    }

    [Command]
    public void CmdCast(int skillNo)
    {
        RpcSkillStart(skillNo);
    }

    [ClientRpc]
    void RpcSkillStart(int skillNo)
    {
        current.SkillBreak(isServer);
        current = skills[skillNo];
        current.SkillStart(isServer);
    }
}
