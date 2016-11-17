using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShootBullet : Skill 
{
    [Tooltip("发生时间")]
    public float startupTime = 0.1f;

    [Tooltip("有效时间")]
    public float activeTime = 0.1f;

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

    [Tooltip("投射物预制件")]
    public GameObject bulletPrefab;

    [Tooltip("投射物生命周期")]
    public float bulletLifeTime = 5.0f;
    
    bool server = false;

    public override void OnRegisterPrefab()
    {
        ClientScene.RegisterPrefab(bulletPrefab);
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

    public override void Process(bool isServer)
    {

    }

    void DoActive()
    {
        phase = SkillPhase.Active;
        if (server)
        {
            var pos = player.transform.position + player.transform.forward * 0.5f;
            var bullet = HitBox.Create(bulletPrefab, pos, player.transform.rotation, this, bulletLifeTime);
            //注：此处不可设置Rigidbody受力，应在HitBox的Start中设定
            //同样此处只有SyncVar会得到传播
            NetworkServer.Spawn(bullet);
        }
        Invoke("DoRecover", activeTime);
    }

    void DoRecover()
    {
        phase = SkillPhase.Recovery;
        Invoke("DoEnd", 0.1f);
    }

    void DoEnd()
    {
        phase = SkillPhase.None;
        status = SkillStatus.Inactive;

        var animator = Model.GetComponent<Animator>();
        animator.SetBool(animationString, false);
    }
}