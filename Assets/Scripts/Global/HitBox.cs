using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class HitBox : NetworkBehaviour
{
    /// <summary>
    /// 技能的施放者
    /// </summary>
    [Tooltip("动态分配不需填写"), SyncVar]
    public NetworkIdentity player;

    /// <summary>
    /// 子弹的旋转
    /// </summary>
    [SyncVar]
    protected Quaternion rotate;

    /// <summary>
    /// 技能编号
    /// </summary>
    [SyncVar]
    protected int skillNo;

    /// <summary>
    /// 技能编号
    /// </summary>
    public int SkillNo
    {
        get
        {
            return skillNo;
        }
    }

    /// <summary>
    /// 技能作为攻击方的种类
    /// 一般和防御方种类是一致的
    /// </summary>
    public Skill.SkillType skillType;

    /// <summary>
    /// 受伤方式，如果有可能的话影响受伤后的动画
    /// 不影响游戏机制
    /// </summary>
    public enum HurtStyle
    {
        NoHurt,
        LightHurt,
        HeavyHurt,
        BlowAway,

        /// <summary>
        /// 目前游戏机制里就没倒地设定QAQ……
        /// </summary>
        KnockDown,
    }
    public HurtStyle hurtStyle = HurtStyle.LightHurt;

    /// <summary>
    /// 屏蔽列表，在此列表中的角色不会再次参与判定
    /// </summary>
    protected List<NetworkIdentity> maskList;

    /// <summary>
    /// 存续时间
    /// </summary>
    [SyncVar]
    protected float lifeTime;

    void Awake()
    {
        maskList = new List<NetworkIdentity>();
    }

    protected virtual void Start()
    {
        if (gameObject.layer != LayerMask.NameToLayer("HitBox"))
        {
            throw new UnityException("判定盒必须位于HitBox层");
        }

        transform.rotation = this.rotate;
        Invoke("KillSelf", lifeTime);
    }

    void KillSelf()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 在防御方OnHit后调用
    /// 只在服务器调用
    /// </summary>
    /// <param name="target">命中的目标角色</param>
    /// <param name="holdType">防御方技能种类</param>
    /// <returns>改变后的技能种类</returns>
    /// <remarks>大部分情况并不需要重载这个函数</remarks>
    public virtual Skill.SkillType OnHitType(GameObject target, Skill.SkillType holdType)
    {
        return skillType;
    }

    /// <summary>
    /// 刚刚命中某个目标时的判定
    /// </summary>
    /// <param name="isServer">是否是服务器</param>
    /// <param name="target">命中目标角色</param>
    /// <returns>命中是否有效</returns>
    /// <remarks>大部分情况并不需要重载这个函数</remarks>
    public virtual bool OnHitStart(bool isServer, GameObject target)
    {
        if (target.GetComponent<NetworkIdentity>() == player)
        {
            return false;
        }
        if (maskList.Exists(obj => obj == target.GetComponent<NetworkIdentity>()))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 命中判定已经成功，产生特定效果时调用
    /// </summary>
    /// <param name="isServer">是否是服务器</param>
    /// <param name="target">命中目标角色</param>
    /// <param name="isBlocked">是否被防御（是确定计算的防御）</param>
    public virtual void OnTakeEffect(bool isServer, GameObject target, bool isBlocked)
    {
        maskList.Add(target.GetComponent<NetworkIdentity>());
    }

    /// <summary>
    /// <para>创建HitBox对象并设置基本属性</para>
    /// 之后需要手动Spawn
    /// </summary>
    /// <param name="prefab">HitBox的预制件</param>
    /// <param name="pos">创建位置</param>
    /// <param name="rotate">创建方位</param>
    /// <param name="skill">创建的技能</param>
    /// <param name="lifeTime">存续时间</param>
    /// <returns>新创建的HitBox</returns>
    public static GameObject Create(Object prefab, Vector3 pos, Quaternion rotate, Skill skill, float lifeTime)
    {
        pos += Vector3.up * 1.6f; //身高补正
        var ret = (GameObject)Instantiate(prefab, pos, rotate);
        var hitBox = ret.GetComponent<HitBox>();
        hitBox.rotate = rotate;
        hitBox.player = skill.Player.GetComponent<NetworkIdentity>();
        hitBox.skillNo = skill.SkillNo;
        hitBox.lifeTime = lifeTime;
        return ret;
    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //此时仍然不能判断击中的是否是玩家，因为有可能是创造物
            //所以交给对方处理
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("HitBox"))
        {
            //相杀，这里选择不处理
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            //默认层的是障碍物，直接删除对象
            Destroy(gameObject);
        }
    }
}
