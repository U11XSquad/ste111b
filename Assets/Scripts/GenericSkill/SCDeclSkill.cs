using UnityEngine;
using System.Collections;

public class SCDeclSkill : Skill
{
    [Tooltip("发生时间")]
    public float startupTime = 0.1f;

    [Tooltip("动作回复时间")]
    public float recoveryTime = 0.1f;

    [Tooltip("输入按键")]
    public string keyName;

    [Tooltip("复合按键的输入时间")]
    public float inputInterval = 0.1f;

    [Tooltip("SP消耗")]
    public float spCost = 0.0f;

    [Tooltip("动画字符串")]
    public string animationString;

    [Tooltip("实际技能")]
    public Skill spellSkill;

    [Tooltip("宣言后的持续时间")]
    public float lastTime;

    [Tooltip("播放音效")]
    public AudioClip seClip;

    public override bool InputDetermine()
    {
        var spgeneric = player.GetComponent<SPGeneric>();
        if (!spgeneric || spgeneric.InSpell || spgeneric.SP < spCost)
        {
            return false;
        }

        return NameToInput(keyName, inputInterval);
    }

    public override void SkillStart(bool isServer)
    {
        base.SkillStart(isServer);
        Invoke("DoActive", startupTime);

        var animator = Model.GetComponent<Animator>();
        animator.SetBool(animationString, true);

        if (seClip)
        {
            SoundEffect.Play(seClip);
        }
    }

    protected virtual void DoActive()
    {
        player.GetComponent<SPGeneric>().SpellStart(spellSkill, lastTime);

        phase = SkillPhase.Recovery;
        Invoke("DoEnd", recoveryTime);
    }

    protected virtual void DoEnd()
    {
        phase = SkillPhase.None;
        status = SkillStatus.Inactive;
        var animator = Model.GetComponent<Animator>();
        animator.SetBool(animationString, false);
    }

    public override void SkillBreak(bool isServer)
    {
        base.SkillBreak(isServer);
        CancelInvoke();
        var animator = Model.GetComponent<Animator>();
        animator.SetBool(animationString, false);
    }
}
