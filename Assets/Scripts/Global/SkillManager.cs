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
    public Skill nextSkill { get; set; }

    /// <summary>
    /// 硬直时间
    /// </summary>
    public float stunTime { get; set; }

    /// <summary>
    /// 是否处于硬直中
    /// </summary>
    public bool isInStun
    {
        get
        {
            return stunTime >= Mathf.Epsilon;
        }
    }

    /// <summary>
    /// 技能释放完或任何有必要的时候回站立状态，不需要服务器统一控制
    /// </summary>
    void ReturnToStand()
    {
        current = standSkill;
        current.SkillStart(isServer);
        nextSkill = null;
    }

    void Awake()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].SkillNo = i;
            skills[i].manager = this;
        }
        current = null;
        nextSkill = null;
        stunTime = 0.0f;
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
        if (current.status != Skill.SkillStatus.Active)
        {
            ReturnToStand();
        }

        //非本地玩家不进行检测、打断和取消
        if (!isLocalPlayer)
        {
            return;
        }

        //硬直中的处理
        if (isInStun)
        {
            stunTime -= Time.fixedDeltaTime;
            stunTime = Mathf.Max(stunTime, 0.0f);
        }

        //技能输入检测
        foreach (Skill skill in skills)
        {
            //如果技能无条件参与检查，那么依然检查技能
            if (skill.AlwaysCheckInput)
            {
                if (skill.InputDetermine())
                {
                    nextSkill = skill;
                }
                continue;
            }
            //否则硬直中不检测技能
            if (isInStun)
            {
                continue;
            }
            //技能可用，且优先级足够，那么进行检测
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
            (current.phase == Skill.SkillPhase.Recovery ||
            current.status != Skill.SkillStatus.Active))
        {
            CmdCast(nextSkill.SkillNo);
            return;
        }

        //技能无条件打断
        if (nextSkill &&
            nextSkill.AlwaysCheckInput)
        {
            //无条件打断会破坏硬直
            stunTime = 0.0f;
            CmdCast(nextSkill.SkillNo);
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
        nextSkill = null;
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
}
