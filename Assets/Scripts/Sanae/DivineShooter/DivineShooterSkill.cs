using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DivineShooterSkill : Skill
{
    bool server = false;
    public GameObject bulletPrefab;

    public override void OnRegisterPrefab()
    {
        ClientScene.RegisterPrefab(bulletPrefab);
    }

    public override bool InputDetermine()
    {
        return input.LTrigger;
    }

    public override void SkillStart(bool isServer)
    {
        base.SkillStart(isServer);
        server = isServer;
        Invoke("DoActive", 0.1f);
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
            var bullet = HitBox.Create(bulletPrefab, pos, player.transform.rotation, this, 100.0f);
            //注：此处不可设置Rigidbody受力，应在HitBox的Start中设定
            //同样此处只有SyncVar会得到传播
            NetworkServer.Spawn(bullet);
        }
        Invoke("DoRecover", 0.1f);
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
    }
}
