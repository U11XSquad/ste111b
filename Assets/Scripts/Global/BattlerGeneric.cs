using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BattlerGeneric : NetworkBehaviour
{
    /// <summary>
    /// 最大生命值
    /// </summary>
    public int HPMax = 10000;

    /// <summary>
    /// 生命值
    /// </summary>
    /// <remarks>因为服务器同步受伤事件，所以就不同步HP了</remarks>
    public int HP { get; set; }

    // Use this for initialization
    void Start()
    {
        Respawn();
    }

    void Respawn()
    {
        //TODO:为了保证C/S处于同一平面内，需要手动调整位置
        if (isLocalPlayer)
        {
            if (isServer)
            {
                transform.position = new Vector3(0f, 0f, 10.0f);
            }
            else
            {
                transform.position = new Vector3(0f, 0f, 0f);
            }
        }
        HP = HPMax;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        SkillManager manager = GetComponent<SkillManager>();
        //伤害对人/弹幕对人检查
        if (other.gameObject.layer == LayerMask.NameToLayer("HitBox"))
        {
            var hitBox = other.GetComponent<HitBox>();
            //首先进行有效命中判定
            if (!hitBox.OnHitStart(isServer, gameObject))
            {
                return;
            }
            //然后如果不是服务器就没啥事了，命中和伤害是服务器算的
            if (!isServer)
            {
                return;
            }
            //进行防御方类型判定
            var holdType = manager.current.OnHit(isServer, other.gameObject);
            //进行攻击方类型判定
            var hitType = hitBox.OnHitType(gameObject, holdType);
            //计算命中和防御
            bool isHit, isBlocked;
            CalcHitStatus(holdType, hitType, out isHit, out isBlocked);
            //如果命中，向全体客户端发送结果
            if (isHit)
            {
                RpcTriggerHit(hitBox.GetComponent<NetworkIdentity>(), isBlocked);
            }
        }
    }

    [ClientRpc]
    void RpcTriggerHit(NetworkIdentity hitBoxId, bool isBlocked)
    {
        HitBox hitBox = hitBoxId.GetComponent<HitBox>();
        var manager = GetComponent<SkillManager>();
        bool isHit = true;
        //然后如果防御成功，计算防御效果
        if (isBlocked)
        {
            isHit = manager.current.BlockSucceed(isServer, hitBox);
        }
        //最后如果防御方依然认为技能命中，计算技能的攻击效果
        if (isHit)
        {
            hitBox.OnTakeEffect(isServer, gameObject, isBlocked);
        }
    }

    /// <summary>
    /// <para>造成伤害并反映在技能上</para>
    /// 该函数会在客户机和服务器同时被调用，因此不必同步HP
    /// </summary>
    /// <param name="isBlocked"></param>
    /// <param name="damage"></param>
    /// <param name="stunTime"></param>
    /// <param name="style"></param>
    public void DealDamage(bool isBlocked, int damage, float stunTime, HitBox.HurtStyle style)
    {
        HP -= damage;
        //TODO:UI事件
        if (HP <= 0)
        {
            OnDeath();
        }
        GetComponent<SkillManager>().DamageReact(isBlocked, stunTime, style);
    }

    void OnDeath()
    {
        Respawn();
    }

    /// <summary>
    /// 计算命中与防御状态
    /// </summary>
    /// <param name="holdType">防御方类型</param>
    /// <param name="hitType">攻击类型</param>
    /// <param name="isHit">输出是否命中</param>
    /// <param name="isBlocked">输出是否防御成功</param>
    void CalcHitStatus(Skill.SkillType holdType, Skill.SkillType hitType, out bool isHit, out bool isBlocked)
    {
        if (hitType == Skill.SkillType.None)
        {
            throw new UnityException("非法的攻击类型");
        }

        if (holdType == Skill.SkillType.None)
        {
            //无防御状态下命中，强制算作直接击中
            isHit = true;
            isBlocked = false;
        }
        else if (holdType == Skill.SkillType.Immune)
        {
            //免疫状态下命中，强制视为不命中
            isHit = false;
            isBlocked = false;
        }
        else if (holdType == Skill.SkillType.Project)
        {
            //弹幕发射中命中，视为击中
            isHit = true;
            isBlocked = false;
        }
        else if (holdType == Skill.SkillType.Hit)
        {
            if (hitType == Skill.SkillType.Break)
            {
                //打击技对防御破坏技具有一般克制关系
                isHit = false;
                isBlocked = false;
            }
            else
            {
                //其他按照判定顺序算，先判定的击中并且终止现有的技能
                isHit = true;
                isBlocked = false;
            }
        }
        else if (holdType == Skill.SkillType.Break)
        {
            //这种情况应当是破防技能发动中
            if (hitType == Skill.SkillType.Block)
            {
                throw new UnityException("这是理论上不可能发生的攻防组合，请检查游戏设计");
            }
            isHit = true;
            isBlocked = false;
        }
        else if (holdType == Skill.SkillType.Block)
        {
            isHit = true;
            if (hitType == Skill.SkillType.Break || hitType == Skill.SkillType.Immune)
            {
                isBlocked = false;
            }
            else
            {
                isBlocked = true;
            }
        }
        else
        {
            throw new UnityException("非法的防御类型");
        }
    }
}
