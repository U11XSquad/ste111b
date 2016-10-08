using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerGeneric : NetworkBehaviour
{
    public GameObject model;

    [Tooltip("地面阻力，单位m/s^2")]
    public float groundFric = 50.0f;

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

    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer)
        {
            TeamManager.Register(this, TeamManager.RegisterStatus.Player);
            GetComponent<UIGeneric>().Register(true);
        }
        else
        {
            TeamManager.Register(this, TeamManager.RegisterStatus.Opponent);
            GetComponent<UIGeneric>().Register(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        GroundFriction();
    }

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
