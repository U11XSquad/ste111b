using UnityEngine;
using System.Collections;

public class Skill : MonoBehaviour
{
    /// <summary>
    /// 技能编号
    /// </summary>
    [Tooltip("动态分配不需填写")]
    public int SkillNo;

    /// <summary>
    /// 拥有该技能的当前玩家
    /// </summary>
    protected GameObject player;

    /// <summary>
    /// player的输入组件
    /// </summary>
    protected InputCtrl input;

    /// <summary>
    /// 取消等级，A > B时A可以打断B
    /// </summary>
    public int InterruptPriority = 0;

    /// <summary>
    /// 是否允许通常打断，即高等级取消低等级
    /// </summary>
    /// <remarks>
    /// 1.注意出招取消无法被禁止
    /// 2.通常打断遵循后摇取消规则，即动作中激发的最后一个技能延迟到后摇阶段开始时打断
    /// </remarks>
    public bool GenericInterruptable = true;

    /// <summary>
    /// 技能的施放状态
    /// </summary>
    public enum SkillStatus
    {
        Disable = 0,
        Inactive = 1,
        Active = 2
    }
    public SkillStatus status = SkillStatus.Inactive;

    /// <summary>
    /// 这里是技能的施放阶段
    /// </summary>
    public enum SkillPhase
    {
        /// <summary>
        /// 技能未释放，阶段无意义
        /// </summary>
        None = 0,

        /// <summary>
        /// 发生阶段
        /// </summary>
        Startup = 1,

        /// <summary>
        /// 判定阶段
        /// </summary>
        Active = 2,

        /// <summary>
        /// 收招阶段
        /// </summary>
        Recovery = 3
    }
    [Tooltip("动态分配不需填写")]
    public SkillPhase phase = SkillPhase.None;

    [Tooltip("动态分配不需填写")]
    public SkillManager manager = null;

    /// <summary>
    /// 技能作为防御方的种类
    /// </summary>
    public enum SkillType
    {
        /// <summary>
        /// 无防御
        /// </summary>
        None = 0,

        /// <summary>
        /// 打击技
        /// </summary>
        Hit = 1,

        /// <summary>
        /// 防御破坏技
        /// </summary>
        Break = 2,

        /// <summary>
        /// 当身技
        /// </summary>
        Block = 3,

        /// <summary>
        /// 远程技（弹幕）
        /// </summary>
        Project = 4,

        /// <summary>
        /// 免疫
        /// </summary>
        Immune = 5,
    }
    public SkillType skillType = SkillType.None;

    virtual protected void Awake()
    {
        //Player -> Skills -> Skill
        player = transform.parent.parent.gameObject;
        if (!player.GetComponent<PlayerGeneric>())
        {
            throw new MissingComponentException("Skill的对象结构和预期不符");
        }

        input = player.GetComponent<InputCtrl>();
    }

    virtual protected void Start()
    {
    }

    /// <summary>
    /// 技能输入判定，在技能可能被输入时根据按键判定其是否成立
    /// </summary>
    /// <returns>是否成立</returns>
    virtual public bool InputDetermine()
    {
        return false;
    }

    /// <summary>
    /// 技能的进程中处理
    /// </summary>
    /// <param name="isServer">是否是服务器调用</param>
    virtual public void Process(bool isServer)
    {
    }

    /// <summary>
    /// 技能开始施放
    /// </summary>
    /// <param name="isServer">是否是服务器调用</param>
    virtual public void SkillStart(bool isServer)
    {
        status = SkillStatus.Active;
        phase = SkillPhase.Startup;
    }

    /// <summary>
    /// 技能被打断时调用
    /// 如果是靠手动修改status终止技能则不调用
    /// </summary>
    /// <param name="isServer">是否是服务器调用</param>
    virtual public void SkillBreak(bool isServer)
    {
        status = SkillStatus.Inactive;
        phase = SkillPhase.None;
    }

    /// <summary>
    /// 技能发动中的受击事件
    /// 判断应当以什么方式计算伤害和硬直
    /// </summary>
    /// <param name="isServer">是否是服务器调用</param>
    /// <param name="hitBox">伤害的判定体</param>
    /// <returns>伤害计算方式</returns>
    /// <remarks>大部分情况可以靠调整skillType解决，并不需要改重载这个函数</remarks>
    virtual public SkillType OnHit(bool isServer, GameObject hitBox)
    {
        if (skillType == SkillType.Block && phase != SkillPhase.Active)
        {
            //一般当身技的前后摇中不计入防御
            return SkillType.None;
        }
        else
        {
            //否则按照技能标准的伤害计算来处理
            return skillType;
        }
    }

    /// <summary>
    /// 当身技防御成功时的处理
    /// </summary>
    /// <param name="isServer">是否是服务器调用</param>
    /// <param name="opponent">对方角色</param>
    /// <returns>
    /// 是否仍然计算伤害
    /// 1 -> 按防御状态计算伤害
    /// 0 -> 抵抗此次伤害
    /// </returns>
    virtual public bool BlockSucceed(bool isServer, GameObject opponent)
    {
        return true;
    }
}
