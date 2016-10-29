using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Basic3Phase : Skill
{
    [Tooltip("发生时间")]
    public float startupTime = 0.1f;

    [Tooltip("有效时间")]
    public float activeTime = 0.1f;

    [Tooltip("动作回复时间")]
    public float recoveryTime = 0.1f;

    [Tooltip("判定盒在人物身前的距离")]
    public float hitBoxDist = 1.0f;

    [Tooltip("输入按键")]
    public string keyName;

    [Tooltip("复合按键的输入时间")]
    public float inputInterval = 0.1f;

    [Tooltip("SP消耗")]
    public float spCost = 0.0f;

    [Tooltip("动画字符串")]
    public string animationString;

    [Tooltip("判定盒的预制件")]
    public Object hitBox;

    bool server;

    public override void OnRegisterPrefab()
    {
        ClientScene.RegisterPrefab(hitBox as GameObject);
    }

    public override bool InputDetermine()
    {
        //检测SP消耗
        if (spCost > 0.5f)
        {
            var spgeneric = player.GetComponent<SPGeneric>();
            if (!spgeneric || spgeneric.InSpell || spgeneric.SP < spCost)
            {
                return false;
            }
        }

        return NameToInput(keyName, inputInterval);
    }

    public override void SkillStart(bool isServer)
    {
        base.SkillStart(isServer);
        server = isServer;
        Invoke("DoActive", startupTime);

        var animator = Model.GetComponent<Animator>();
        animator.SetBool(animationString, true);
    }

    public override void SkillBreak(bool isServer)
    {
        base.SkillBreak(isServer);
        CancelInvoke();
        var animator = Model.GetComponent<Animator>();
        animator.SetBool(animationString, false);
    }

    protected virtual void DoActive()
    {
        phase = SkillPhase.Active;
        if (server)
        {
            var pos = player.transform.position + player.transform.forward * hitBoxDist;
            var bullet = HitBox.Create(hitBox, pos, player.transform.rotation, this, activeTime);
            NetworkServer.Spawn(bullet);
        }
        Invoke("DoRecover", activeTime);
    }

    protected virtual void DoRecover()
    {
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
}
