using UnityEngine;
using System.Collections;

public class DivineShooterHitBox : HitBox
{

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        var rigid = GetComponent<Rigidbody>();
        rigid.AddForce(transform.forward, ForceMode.VelocityChange);
    }

    public override void OnTakeEffect(bool isServer, GameObject target, bool isBlocked)
    {
        base.OnTakeEffect(isServer, target, isBlocked);

        var opponent = target.GetComponent<BattlerGeneric>();
        opponent.DealDamage(isBlocked, 1000, 1.0f, HurtStyle.LightHurt, player.GetComponent<PlayerGeneric>());

        var dist = GetHitDir(target); ;
        //target.GetComponent<Rigidbody>().AddForce(dist * 150.0f);
        target.GetComponent<MovingGeneric>().AddDisplace(dist * 1.5f, 0.1f);

        var sp = this.player.GetComponent<SPGeneric>();
        if (sp)
        {
            sp.SP += 50.0f;
        }

        KillSelf();
    }
}
