using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MovingGeneric : NetworkBehaviour
{
    /// <summary>
    /// 指定角色朝向
    /// </summary>
    /// <param name="dir">面朝方向</param>
    public void FaceTo(Vector3 dir)
    {
        var ang = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        ang = 450 - ang; //调整摄像机时，此数值需变动
        transform.rotation = Quaternion.AngleAxis(ang, Vector3.up);
    }

    /// <summary>
    /// 面朝向当前锁定的目标
    /// </summary>
    public void Lock()
    {
        var dir = TeamManager.Opponent.transform.position - transform.position;
        FaceTo(dir);
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        GroundFriction();
    }

    [Tooltip("地面阻力，单位m/s^2")]
    public float groundFric = 50.0f;

    /// <summary>
    /// 地面阻力造成的人物减速
    /// 也可以考虑直接用Drag
    /// </summary>
    void GroundFriction()
    {
        var rigid = GetComponent<Rigidbody>();
        var velocity = rigid.velocity;
        var speed = velocity.magnitude;
        if (Mathf.Approximately(speed, 0.0f))
        {
            return;
        }

        velocity.Normalize();
        velocity = velocity * -Mathf.Min(speed, groundFric * Time.fixedDeltaTime);
        rigid.AddForce(velocity, ForceMode.VelocityChange);
    }
}
