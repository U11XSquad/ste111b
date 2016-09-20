using UnityEngine;
using System.Collections;

public abstract class Skill : MonoBehaviour
{
    /// <summary>
    /// 拥有该技能的当前玩家
    /// </summary>
    protected GameObject player;

    // Use this for initialization
    virtual void Start()
    {
        //Player -> SkillManager -> Skill
        player = transform.parent.parent.gameObject;
        if (!player.GetComponent<PlayerGeneric>())
        {
            throw new MissingComponentException("Skill的对象结构和预期不符");
        }
    }

    /// <summary>
    /// 技能输入判定，在技能可能被输入时根据按键判定其是否成立
    /// </summary>
    /// <returns>是否成立</returns>
    abstract public bool InputDetermine();
}
