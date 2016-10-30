using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BattlerGeneric : NetworkBehaviour
{
    /// <summary>
    /// 最大生命值
    /// </summary>
    public int HPMax = 10000;

    [Tooltip("复活准备时间")]
    public float respawnTime = 3.0f;

    /// <summary>
    /// 生命值
    /// </summary>
    /// <remarks>因为服务器同步受伤事件，所以就不同步HP了</remarks>
    public int HP { get; set; }

    /// <summary>
    /// HP显示条预制件
    /// </summary>
    public Object hpbar;

    public delegate void BlockEvent();
    /// <summary>
    /// <para>格挡成功后的事件调用</para>
    /// 供其他非技能的人物系统使用
    /// </summary>
    public event BlockEvent OnBlock;

    public delegate void DamageEvent(int damage);
    /// <summary>
    /// <para>受到伤害后的事件调用</para>
    /// 供其他非技能的人物系统使用
    /// </summary>
    public event DamageEvent OnDamage;

    protected bool isDead;
    public bool IsDead
    {
        get
        {
            return isDead;
        }
    }

    void Awake()
    {
        GetComponent<UIGeneric>().OnRegister += UIRegister;
    }

    void UIRegister(bool isLocal, UIRegister panel)
    {
        panel.Register(hpbar, GetComponent<NetworkIdentity>());
    }

    // Use this for initialization
    void Start()
    {
        //重生
        Respawn();
    }

    void Respawn()
    {
        //取消复活的准备
        CancelInvoke("Respawn");
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
        isDead = false;
        //如果是玩家，准备技能
        var sm = GetComponent<SkillManager>();
        if (sm && isLocalPlayer)
        {
            sm.InstantBrake();
        }
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
            OnBlock();
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
    /// <param name="isBlocked">结果上此次伤害后对方是否是防御</param>
    /// <param name="damage">伤害值</param>
    /// <param name="stunTime">硬直时间</param>
    /// <param name="style">受伤风格</param>
    /// <param name="source">伤害来源（玩家）</param>
    public void DealDamage(bool isBlocked, int damage, float stunTime, HitBox.HurtStyle style, PlayerGeneric source)
    {
        HP -= damage;
        OnDamage(damage);
        if (HP <= 0)
        {
            HP = 0;
            OnDeath(source);
        }
        GetComponent<SkillManager>().DamageReact(isBlocked, stunTime, style);
    }

    void OnDeath(PlayerGeneric source)
    {
        isDead = true;
        //如果是玩家，记录死亡结果
        var pg = GetComponent<PlayerGeneric>();
        if (pg)
        {
            pg.OnDeath(source);
        }
        //如果现在依然是死亡状态，准备技能，计时重生
        if (isDead)
        {
            GetComponent<SkillManager>().EnterDeath();
            Invoke("Respawn", respawnTime);
        }
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
