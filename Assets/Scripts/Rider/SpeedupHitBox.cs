using UnityEngine;
using System.Collections;

public class SpeedupHitBox : HitBox
{
    [Tooltip("命中伤害")]
    public int hurtDamage;

    [Tooltip("命中硬直")]
    public float hurtStun;

    [Tooltip("命中吹飞")]
    public float hurtBlow;

    [Tooltip("命中吹飞时间")]
    public float hurtBlowTime = 0.1f;

    [Tooltip("飞行初速度")]
    public float initMoveSpeed;

    [Tooltip("飞行加速度")]
    public float accelerate;

    [Tooltip("命中SP增益")]
    public float hurtSpGain;

    [Tooltip("格挡伤害")]
    public int blockDamage;

    [Tooltip("格挡硬直")]
    public float blockStun;

    [Tooltip("格挡吹飞")]
    public float blockBlow;

    [Tooltip("格挡吹飞时间")]
    public float blockBlowTime = 0.1f;

    [Tooltip("格挡SP增益")]
    public float blockSpGain;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        var rigid = GetComponent<Rigidbody>();
        rigid.AddForce(transform.forward * initMoveSpeed, ForceMode.VelocityChange);   
    }

    protected void FixedUpdate()
    {
        int fpm = 30;

        var rigid = GetComponent<Rigidbody>();
        rigid.AddForce(transform.forward * accelerate / fpm, ForceMode.VelocityChange);
    }

    public override void OnTakeEffect(bool isServer, GameObject target, bool isBlocked)
    {
        base.OnTakeEffect(isServer, target, isBlocked);

        var opponent = target.GetComponent<BattlerGeneric>();
        var dist = GetHitDir(target);
        var sp = this.player.GetComponent<SPGeneric>();
        var mg = target.GetComponent<MovingGeneric>();

        if (isBlocked)
        {
            opponent.DealDamage(isBlocked, blockDamage, blockStun, hurtStyle, player.GetComponent<PlayerGeneric>());
            mg.AddDisplace(dist * blockBlow, blockBlowTime);
            if (sp)
            {
                sp.SP += blockSpGain;
            }
        }
        else
        {
            opponent.DealDamage(isBlocked, hurtDamage, hurtStun, hurtStyle, player.GetComponent<PlayerGeneric>());
            mg.AddDisplace(dist * hurtBlow, hurtBlowTime);
            if (sp)
            {
                sp.SP += hurtSpGain;
            }
        }

        Destroy(gameObject);        
    }
}
