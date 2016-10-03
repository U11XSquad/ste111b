using UnityEngine;
using System.Collections;

public class Skill : MonoBehaviour
{
    /// <summary>
    /// 技能编号
    /// </summary>
    public int SkillNo;

    /// <summary>
    /// 拥有该技能的当前玩家
    /// </summary>
    protected GameObject player;

    protected InputCtrl input;

    /// <summary>
    /// 取消等级，A > B时A可以打断B
    /// </summary>
    public int InterruptLevel;

    //暂定
    public enum SkillStatus
    {
        Disable = 0,
        Inactive = 1,
        Active = 2
    }

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
    }
}
