using UnityEngine;
using System.Collections;

public class DivineShooterHitBox : HitBox
{

    // Use this for initialization
    void Start()
    {
        transform.rotation = base.rotate;
        var rigid = GetComponent<Rigidbody>();
        rigid.AddForce(transform.forward, ForceMode.VelocityChange);
    }

    public override void OnTakeEffect(bool isServer, GameObject target, bool isBlocked)
    {
        base.OnTakeEffect(isServer, target, isBlocked);

        var opponent = target.GetComponent<BattlerGeneric>();
        opponent.DealDamage(isBlocked, 1000, 1.0f, HurtStyle.LightHurt);

        var dist = target.transform.position - transform.position;
        dist.Normalize();
        target.GetComponent<Rigidbody>().AddForce(dist * 150.0f);

        Destroy(gameObject);
    }
}
