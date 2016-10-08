﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class SkillManager : NetworkBehaviour
{
    /// <summary>
    /// 所有的技能
    /// </summary>
    protected Skill[] skills;

    /// <summary>
    /// 通常站立技
    /// </summary>
    public Skill standSkill;
    /// <summary>
    /// 通常防御技
    /// </summary>
    public Skill blockSkill;
    /// <summary>
    /// 通常受伤
    /// </summary>
    public Skill hurtSkill;

    /// <summary>
    /// 当前施放的技能
    /// </summary>
    public Skill current { get; set; }

    /// <summary>
    /// 后摇取消后接的技能，参见<seealso cref="Skill.GenericInterruptable"/>
    /// </summary>
    public Skill NextSkill { get; set; }

    /// <summary>
    /// 硬直时间
    /// </summary>
    public float StunTime { get; set; }

    /// <summary>
    /// 是否处于硬直中
    /// </summary>
    public bool IsInStun
    {
        get
        {
            return StunTime >= Mathf.Epsilon;
        }
    }

    /// <summary>
    /// 技能释放完或任何有必要的时候回站立状态，不需要服务器统一控制
    /// </summary>
    void ReturnToStand()
    {
        current = standSkill;
        current.SkillStart(isServer);
        NextSkill = null;
    }

    void Awake()
    {
        skills = GetComponentsInChildren<Skill>() as Skill[];

        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].SkillNo = i;
            skills[i].manager = this;
        }
        current = null;
        NextSkill = null;
        StunTime = 0.0f;
    }

    // Use this for initialization
    void Start()
    {
        ReturnToStand();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!current)
        {
            throw new UnityException("当前技能为空");
        }

        //当前技能的处理（硬直中依然处理）
        current.Process(isServer);

        //非本地玩家不进行检测、打断和取消
        if (!isLocalPlayer)
        {
            return;
        }

        //当前技能的结束
        if (current.status != Skill.SkillStatus.Active)
        {
            ReturnToStand();
        }

        //硬直中的处理
        if (IsInStun)
        {
            StunTime -= Time.fixedDeltaTime;
            StunTime = Mathf.Max(StunTime, 0.0f);
        }

        //技能输入检测
        foreach (Skill skill in skills)
        {
            //如果技能无条件参与检查，那么依然检查技能
            if (skill.AlwaysCheckInput)
            {
                if (skill.InputDetermine())
                {
                    NextSkill = skill;
                }
                continue;
            }
            //否则硬直中不检测技能
            if (IsInStun)
            {
                continue;
            }
            //技能可用，且优先级足够，那么进行检测
            if (skill.status != Skill.SkillStatus.Disable &&
                skill.InterruptPriority > current.InterruptPriority &&
                skill.InputDetermine())
            {
                NextSkill = skill;
            }
        }

        //技能通常打断
        if (NextSkill &&
            current.GenericInterruptable &&
            (current.phase == Skill.SkillPhase.Recovery ||
            current.status != Skill.SkillStatus.Active))
        {
            CmdCast(NextSkill.SkillNo);
            return;
        }

        //技能无条件打断
        if (NextSkill &&
            NextSkill.AlwaysCheckInput)
        {
            //无条件打断会破坏硬直
            StunTime = 0.0f;
            CmdCast(NextSkill.SkillNo);
            return;
        }

        //技能自发结束
        if (current.status != Skill.SkillStatus.Active)
        {
            ReturnToStand();
            return;
        }

        //出招取消
        //TODO
    }

    /// <summary>
    /// 打断当前技能，施放新技能
    /// </summary>
    /// <param name="skillNo">新技能编号</param>
    [Command]
    public void CmdCast(int skillNo)
    {
        RpcSkillStart(skillNo);
    }
    [ClientRpc]
    void RpcSkillStart(int skillNo)
    {
        //中断当前技能
        current.SkillBreak(isServer);
        current = skills[skillNo];
        //开始新技能
        current.SkillStart(isServer);
        //撤销下一个技能
        NextSkill = null;
    }

    /// <summary>
    /// 取消当前技能，同时通知其他的客户端取消
    /// </summary>
    /// <remarks>
    /// 限定于有按键检测的技能因特殊按键而停止
    /// 如果是技能自发终止建议直接改status
    /// </remarks>
    public void SkillCancel()
    {
        //一般取消直接用Stand取消
        CmdCast(standSkill.SkillNo);
    }

    /// <summary>
    /// 进入特定的受伤状态
    /// </summary>
    /// <param name="style">受伤种类(未使用)</param>
    void EnterHurt(HitBox.HurtStyle style)
    {
        //Command需要在权限方调用
        if (!isLocalPlayer)
        {
            return;
        }
        //忽略style
        CmdCast(hurtSkill.SkillNo);
    }

    /// <summary>
    /// 伤害反应
    /// </summary>
    /// <param name="isBlocked">是否防御</param>
    /// <param name="stunTime">硬直时间</param>
    /// <param name="style">受伤种类（未使用）</param>
    public void DamageReact(bool isBlocked, float stunTime, HitBox.HurtStyle style)
    {
        this.StunTime = stunTime;
        if (!isBlocked)
        {
            EnterHurt(style);
        }
    }
}
