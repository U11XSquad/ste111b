using UnityEngine;
using System.Collections;

public class SimpleHitBox : HitBox
{
    [Tooltip("前进速度")]
    public float speed = 0.0f;

    [Tooltip("命中伤害")]
    public int hurtDamage;

    [Tooltip("命中硬直")]
    public float hurtStun;

    [Tooltip("命中吹飞")]
    public float hurtBlow;

    [Tooltip("命中SP增益")]
    public float hurtSpGain;

    [Tooltip("格挡伤害")]
    public int blockDamage;

    [Tooltip("格挡硬直")]
    public float blockStun;

    [Tooltip("格挡吹飞")]
    public float blockBlow;

    [Tooltip("格挡SP增益")]
    public float blockSpGain;

    protected override void Start()
    {
        base.Start();
        var rigid = GetComponent<Rigidbody>();
        rigid.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }

    public override void OnTakeEffect(bool isServer, GameObject target, bool isBlocked)
    {
        base.OnTakeEffect(isServer, target, isBlocked);

        var opponent = target.GetComponent<BattlerGeneric>();
        var dist = GetHitDir(target);
        var sp = this.player.GetComponent<SPGeneric>();

        if (isBlocked)
        {
            opponent.DealDamage(isBlocked, blockDamage, blockStun, hurtStyle, player.GetComponent<PlayerGeneric>());
            target.GetComponent<Rigidbody>().AddForce(dist * blockBlow);
            if (sp)
            {
                sp.SP += blockSpGain;
            }
        }
        else
        {
            opponent.DealDamage(isBlocked, hurtDamage, hurtStun, hurtStyle, player.GetComponent<PlayerGeneric>());
            target.GetComponent<Rigidbody>().AddForce(dist * hurtBlow);
            if (sp)
            {
                sp.SP += hurtSpGain;
            }
        }
    }
}
