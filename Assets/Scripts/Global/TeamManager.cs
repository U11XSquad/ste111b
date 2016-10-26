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
    static public NetworkIdentity Player
    {
        get
        {
            return Instance.player;
        }
    }

    NetworkIdentity opponent;
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

    void Start()
    {
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
        if (isServer)
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

    PlayerResult[] GetResults()
    {
        var ret = new PlayerResult[allPlayers.Count];
        //TODO
        ret[0].result = ResultCarrier.Result.Win;
        ret[1].result = ResultCarrier.Result.Lose;
        return ret;
    }

    void RegisterUI()
    {
        UISystem.FullPanel.Register(TimeBarPrefab);
    }
}
