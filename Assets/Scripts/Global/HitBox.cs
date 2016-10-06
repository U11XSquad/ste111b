using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HitBox : NetworkBehaviour
{
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

    [Tooltip("动态分配不需填写")]
    public GameObject player;

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
        return true; //TODO:列表
    }

    /// <summary>
    /// 命中判定已经成功，产生特定效果时调用
    /// </summary>
    /// <param name="isServer">是否是服务器</param>
    /// <param name="target">命中目标角色</param>
    /// <param name="isBlocked">是否被防御（是确定计算的防御）</param>
    public virtual void OnTakeEffect(bool isServer, GameObject target, bool isBlocked)
    {
    }
}
