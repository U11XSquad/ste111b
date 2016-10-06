using UnityEngine;
using System.Collections;

public class DivineShooterHitBox : HitBox
{

    // Use this for initialization
    void Start()
    {
        var rigid = GetComponent<Rigidbody>();
        rigid.AddForce(Vector3.forward, ForceMode.VelocityChange);
    }

}
