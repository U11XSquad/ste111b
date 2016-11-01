using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MovingGeneric : NetworkBehaviour
{
    [Tooltip("最大主动加速度，单位m/s^2")]
    public float MaxAccelerate = 60.0f;

    [Tooltip("最大移动速度，单位m/s")]
    public float MaxSpeed = 40.0f;

    /// <summary>
    /// 表示一次要求的位移
    /// </summary>
    struct Displace
    {
        //------输入参数------
        /// <summary>
        /// 期待的位移距离
        /// </summary>
        public Vector3 expectedDis;

        /// <summary>
        /// 位移间隔
        /// </summary>
        public float interval;

        /// <summary>
        /// 来源签名
        /// </summary>
        public Object signature;

        //------实际使用------
        /// <summary>
        /// 当前速度
        /// </summary>
        public Vector3 speed;

        /// <summary>
        /// 剩余时间
        /// </summary>
        public float remain;

        public Displace decay(float delta)
        {
            var ret = this;
            ret.remain -= delta;
            return ret; //TODO
        }
    }
    /// <summary>
    /// 当前所有的位移
    /// </summary>
    List<Displace> displaces;

    void Awake()
    {
        displaces = new List<Displace>();
    }

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

    void FixedUpdate()
    {
        CalcMove();
    }

    public void AddDisplace(Vector3 expectedDis, float interval, Object source = null)
    {
        Displace disp = new Displace();
        disp.expectedDis = expectedDis;
        disp.interval = interval;
        disp.remain = interval;
        disp.speed = expectedDis / interval;
        disp.signature = source;
        displaces.Add(disp);
    }

    public void AddDisplaceBySpeed(Vector3 expectedSpd, float interval, Object source = null)
    {
        Displace disp = new Displace();
        disp.expectedDis = new Vector3();
        disp.interval = interval;
        disp.remain = interval;
        disp.speed = expectedSpd;
        disp.signature = source;
        displaces.Add(disp);
    }

    public void RemoveDisplace(Object source = null)
    {
        displaces.RemoveAll(disp => disp.signature == source);
    }

    void CalcMove()
    {
        var rigid = GetComponent<Rigidbody>();
        var expSpd = displaces.Aggregate(new Vector3(), (tot, cur) => tot + cur.speed);
        var expSpdLength = Mathf.Min(expSpd.magnitude, MaxSpeed);
        expSpd.Normalize();
        expSpd *= expSpdLength;

        var curSpd = rigid.velocity;
        var velDiff = expSpd - curSpd;

        var accLength = Mathf.Min(velDiff.magnitude, MaxAccelerate * Time.fixedDeltaTime);
        velDiff.Normalize();
        velDiff *= accLength;

        rigid.AddForce(velDiff, ForceMode.VelocityChange);
        displaces = (from disp in displaces 
                    where disp.remain > Time.fixedDeltaTime 
                    select disp.decay(Time.fixedDeltaTime)).ToList();
    }

    [Tooltip("地面阻力，单位m/s^2；已废除")]
    public float groundFric = 50.0f;

    /// <summary>
    /// 地面阻力造成的人物减速
    /// <remarks>
    /// 也可以考虑直接用Drag
    /// </remarks>
    /// </summary>
    [System.Obsolete("该方法已经废弃", true)] 
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
