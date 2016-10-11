using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MOMHitBox : HitBox
{
    [SyncVar]
    public float waitTime;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(MoveOn());
    }

    IEnumerator MoveOn()
    {
        var rigid = GetComponent<Rigidbody>();
        yield return new WaitForSeconds(waitTime);
        for (var i = 0; i < 60; i++)
        {
            rigid.AddForce(transform.up * 0.3f, ForceMode.VelocityChange);
            yield return new WaitForFixedUpdate();
        }
    }

    public override void OnTakeEffect(bool isServer, GameObject target, bool isBlocked)
    {
        base.OnTakeEffect(isServer, target, isBlocked);

        var opponent = target.GetComponent<BattlerGeneric>();
        var dist = GetHitDir(target);

        if (isBlocked)
        {
            opponent.DealDamage(isBlocked, 400, 1.3f, hurtStyle);
            target.GetComponent<Rigidbody>().AddForce(dist * 140.0f);
        }
        else
        {
            opponent.DealDamage(isBlocked, 1500, 0.5f, hurtStyle);
            target.GetComponent<Rigidbody>().AddForce(dist * 150.0f);
        }
    }
}
