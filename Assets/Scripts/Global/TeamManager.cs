using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[NetworkSettings(channel=1)]
public class TeamManager : NetworkBehaviour
{
    static TeamManager instance;
    static public TeamManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<TeamManager>();
                if (!instance)
                {
                    throw new UnityException("缺乏TeamManager");
                }
            }
            return instance;
        }
    }

    NetworkIdentity player;
    /// <summary>
    /// 本地的玩家
    /// </summary>
    static public NetworkIdentity Player
    {
        get
        {
            return Instance.player;
        }
    }

    NetworkIdentity opponent;
    /// <summary>
    /// 本地当前锁定的对手
    /// </summary>
    static public NetworkIdentity Opponent
    {
        get
        {
            return Instance.opponent;
        }
    }

    List<NetworkIdentity> allPlayers;
    static public List<NetworkIdentity> AllPlayers
    {
        get
        {
            return Instance.allPlayers;
        }
    }

    public float totalTime = 5.0f;
    [SyncVar]
    float remainTime;
    /// <summary>
    /// 剩余的比赛时间
    /// </summary>
    static public float RemainTime
    {
        get
        {
            return Instance.remainTime;
        }
    }

    bool uiRegistered = false;
    public Object TimeBarPrefab;

    public ResultCarrier resultCarrierPrefab;

    int localPlayerId;
    static public int LocalPlayerId
    {
        get
        {
            return Instance.localPlayerId;
        }
        set
        {
            Instance.localPlayerId = value;
        }
    }

    [SyncVar]
    Prototype.NetworkLobby.GameFormat gameFormat;
    static public Prototype.NetworkLobby.GameFormat GameFormat
    {
        get
        {
            return Instance.gameFormat;
        }
        set
        {
            Instance.gameFormat = value;
        }
    }

    void Start()
    {
        //获取赛制
        if (isServer)
        {
            gameFormat = FindObjectOfType<Prototype.NetworkLobby.LobbyManager>().CurFormat;
        }
        //创建玩家列表
        allPlayers = new List<NetworkIdentity>();
        //通知各个凭依创建玩家对象替换
        var yorisiros = FindObjectsOfType<Yorisiro>();
        foreach (var yorisiro in yorisiros)
        {
            yorisiro.OnSceneGetReady();
        }
        //UI注册标志
        uiRegistered = false;
        //开启计时器
        remainTime = totalTime;
    }

    void Update()
    {
        if (!uiRegistered)
        {
            RegisterUI();
            uiRegistered = true;
        }
    }

    void FixedUpdate()
    {
        if (isServer && gameFormat != Prototype.NetworkLobby.GameFormat.Training)
        {
            remainTime -= Time.fixedDeltaTime;
        }
        if (isServer && remainTime <= 0.0f)
        {
            OnRoundEnd();
        }
    }

    public enum RegisterStatus
    {
        Player,
        Opponent
    }

    static public void Register(PlayerGeneric player, RegisterStatus status)
    {
        Instance.allPlayers.Add(player.GetComponent<NetworkIdentity>());
        if (status == RegisterStatus.Player)
        {
            Instance.player = player.GetComponent<NetworkIdentity>();
        }
        else if (status == RegisterStatus.Opponent)
        {
            Instance.opponent = player.GetComponent<NetworkIdentity>();
        }
    }

    struct PlayerResult
    {
        public ResultCarrier.Result result;
    }

    void OnRoundEnd()
    {
        PlayerResult[] results = GetResults();
        RpcEnterResultScene(results);

        //var manager = FindObjectOfType<NetworkManager>();
        //manager.ServerChangeScene("ResultScene");
    }

    [ClientRpc]
    void RpcEnterResultScene(PlayerResult[] results)
    {
        var res = Object.Instantiate(resultCarrierPrefab.gameObject).GetComponent<ResultCarrier>();
        res.result = results[localPlayerId].result;
        SceneManager.LoadScene("ResultScene");
    }

    /// <summary>
    /// 计算每个人的结算结果
    /// </summary>
    /// <returns>结算结果</returns>
    PlayerResult[] GetResults()
    {
        var ret = new PlayerResult[allPlayers.Count];
        //TODO:强制双人局
        var p0 = allPlayers[0].GetComponent<PlayerGeneric>();
        var p1 = allPlayers[1].GetComponent<PlayerGeneric>();

        if (p0.DeathCount != p1.DeathCount)
        {
            if (p0.DeathCount < p1.DeathCount)
            {
                ret[p0.PlayerIndex].result = ResultCarrier.Result.Win;
                ret[p1.PlayerIndex].result = ResultCarrier.Result.Lose;
            }
            else
            {
                ret[p0.PlayerIndex].result = ResultCarrier.Result.Lose;
                ret[p1.PlayerIndex].result = ResultCarrier.Result.Win;
            }
        }
        else
        {
            var b0 = p0.GetComponent<BattlerGeneric>();
            var b1 = p1.GetComponent<BattlerGeneric>();
            var hpRatio0 = b0.HP * 1.0f / b0.HPMax;
            var hpRatio1 = b1.HP * 1.0f / b1.HPMax;
            if (Mathf.Approximately(hpRatio0, hpRatio1))
            {
                ret[p0.PlayerIndex].result = ResultCarrier.Result.Tie;
                ret[p1.PlayerIndex].result = ResultCarrier.Result.Tie;
            }
            else
            {
                if (hpRatio0 > hpRatio1)
                {
                    ret[p0.PlayerIndex].result = ResultCarrier.Result.Win;
                    ret[p1.PlayerIndex].result = ResultCarrier.Result.Lose;
                }
                else
                {
                    ret[p0.PlayerIndex].result = ResultCarrier.Result.Lose;
                    ret[p1.PlayerIndex].result = ResultCarrier.Result.Win;
                }
            }
        }
        return ret;
    }

    void RegisterUI()
    {
        UISystem.FullPanel.Register(TimeBarPrefab);
    }

    static public void OnPlayerDeath(PlayerGeneric deadPlayer, PlayerGeneric source)
    {
        if (GameFormat == Prototype.NetworkLobby.GameFormat.FirstDown)
        {
            Instance.remainTime = 0.0f;
        }
    }
}
