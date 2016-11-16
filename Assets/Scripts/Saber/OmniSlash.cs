using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class OmniSlash : Skill
{
    [Tooltip("发生时间")]
    public float startupTime = 0.1f;

    [Tooltip("第一段伤害时间")]
    public float firstAttackTime = 0.1f;

    [Tooltip("第二段伤害时间")]
    public float secondAttackTime = 0.1f;

    [Tooltip("第三段伤害时间")]
    public float thirdAttackTime = 0.1f;

    [Tooltip("动作回复时间")]
    public float recoveryTime = 0.1f;

    [Tooltip("判定盒在人物身前的距离")]
    public float hitBoxDist = 1.0f;

    [Tooltip("判定盒的预制件1")]
    public Object firstHitBox;

    [Tooltip("判定盒的预制件2")]
    public Object secondHitBox;

    [Tooltip("判定盒的预制件3")]
    public Object thirdHitBox;

    bool server;

    public override void OnRegisterPrefab()
    {
        ClientScene.RegisterPrefab(firstHitBox as GameObject);
        ClientScene.RegisterPrefab(secondHitBox as GameObject);
        ClientScene.RegisterPrefab(thirdHitBox as GameObject);
    }

    public override bool InputDetermine()
    {
        return input.STrigger;
    }

    public override void SkillStart(bool isServer)
    {
        base.SkillStart(isServer);
        server = isServer;
        Invoke("FirstAttack", startupTime);

        var animator = Model.GetComponent<Animator>();
        animator.SetBool("spell", true);
    }

    public override void SkillBreak(bool isServer)
    {
        base.SkillBreak(isServer);
        CancelInvoke();
        var animator = Model.GetComponent<Animator>();
        animator.SetBool("spell", false);
    }

    protected void FirstAttack()
    {
        phase = SkillPhase.Active;

        var animator = Model.GetComponent<Animator>();
        animator.SetBool("spell", false);
        animator.SetBool("slash", true);
        player.GetComponent<MovingGeneric>().AddDisplaceBySpeed(player.transform.forward * 16, firstAttackTime, this);
        Invoke("SecondAttack", firstAttackTime);  
    }

    protected void SecondAttack()
    {
        if (server)
        {
            var pos = player.transform.position + player.transform.forward * hitBoxDist;
            var bullet = HitBox.Create(firstHitBox, pos, player.transform.rotation, this, firstAttackTime);
            NetworkServer.Spawn(bullet);
        }

        var animator = Model.GetComponent<Animator>();
        animator.SetBool("slash", false);
        animator.SetBool("poke", true);
        player.GetComponent<MovingGeneric>().AddDisplaceBySpeed(player.transform.forward * 16, secondAttackTime, this);
        Invoke("ThirdAttack", secondAttackTime);  
    }

    protected void ThirdAttack()
    {
        if (server)
        {
            var pos = player.transform.position + player.transform.forward * hitBoxDist;
            var bullet = HitBox.Create(secondHitBox, pos, player.transform.rotation, this, secondAttackTime);
            NetworkServer.Spawn(bullet);
        }

        var animator = Model.GetComponent<Animator>();
        animator.SetBool("poke", false);
        animator.SetBool("hack", true);
        player.GetComponent<MovingGeneric>().AddDisplaceBySpeed(player.transform.forward * 16, thirdAttackTime, this);
        Invoke("DoRecover", thirdAttackTime); 
    }

    protected virtual void DoRecover()
    {
        if (server)
        {
            var pos = player.transform.position + player.transform.forward * hitBoxDist;
            var bullet = HitBox.Create(secondHitBox, pos, player.transform.rotation, this, thirdAttackTime);
            NetworkServer.Spawn(bullet);
        }

        phase = SkillPhase.Recovery;
        Invoke("DoEnd", recoveryTime);
    }

    protected virtual void DoEnd()
    {
        phase = SkillPhase.None;
        status = SkillStatus.Inactive;
        var animator = Model.GetComponent<Animator>();
        animator.SetBool("hack", false);
    }
}
