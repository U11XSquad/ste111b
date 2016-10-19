using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

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

    // Use this for initialization
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
    }

    // Update is called once per frame
    void Update()
    {

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
}
