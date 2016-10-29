using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerGeneric : NetworkBehaviour
{
    [Tooltip("人物识别用名")]
    public string avatarName;

    [Tooltip("人物模型对象")]
    public GameObject model;

    [SyncVar]
    protected int playerIndex;
    /// <summary>
    /// 角色在本局游戏中的编号
    /// </summary>
    public int PlayerIndex
    {
        get
        {
            return playerIndex;
        }
        set
        {
            playerIndex = value;
        }
    }

    public enum Occupation
    {
        /// <summary>
        /// 本机玩家
        /// </summary>
        Player,

        /// <summary>
        /// 本机锁定的对手
        /// </summary>
        Opponent,

        /// <summary>
        /// 其他身份
        /// </summary>
        Other
    }
    /// <summary>
    /// 角色对本地的身份
    /// </summary>
    public Occupation Occup { get; set; }

    /// <summary>
    /// 本局击杀数
    /// </summary>
    public int KillCount { get; set; }

    /// <summary>
    /// 本局死亡数
    /// </summary>
    public int DeathCount { get; set; }

    void Start()
    {
        //TODO:此限定为二人对局所用
        if (isLocalPlayer)
        {
            Occup = Occupation.Player;
            TeamManager.Register(this, TeamManager.RegisterStatus.Player);
            TeamManager.LocalPlayerId = playerIndex;
        }
        else
        {
            Occup = Occupation.Opponent;
            TeamManager.Register(this, TeamManager.RegisterStatus.Opponent);
        }
        KillCount = 0;
        DeathCount = 0;
    }

    public void OnDeath(PlayerGeneric source)
    {
        //增加计数
        DeathCount++;
        if (source)
        {
            source.KillCount++;
        }
        //调用队伍管理器
        TeamManager.OnPlayerDeath(this, source);
    }

    ////////////////////////////////////

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
