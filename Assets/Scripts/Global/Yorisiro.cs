using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;

public class Yorisiro : NetworkBehaviour
{
    [SyncVar]
    protected int playerIndex;
    /// <summary>
    /// 角色的编号
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

    [SyncVar]
    protected string characterName;
    /// <summary>
    /// 角色的识别用名
    /// </summary>
    public string CharacterName
    {
        get
        {
            return characterName;
        }
        set
        {
            characterName = value;
        }
    }

    [SyncVar]
    protected bool isCpuPlayer = false;
    /// <summary>
    /// 是否是AI玩家
    /// </summary>
    public bool IsCpuPlayer
    {
        get
        {
            return isCpuPlayer;
        }
        set
        {
            isCpuPlayer = value;
        }
    }

    [Command]
    void CmdChangePlayer()
    {
        PlayerGeneric[] avatars = CharacterManager.Avatars;
        var nid = GetComponent<NetworkIdentity>();
        var conn = nid.connectionToClient;
        GameObject chara = (from data in avatars where data.avatarName == characterName select data.gameObject).ElementAtOrDefault(0);
        if (chara == null)
        {
            chara = avatars[0].gameObject;
        }

        var newp = (GameObject)Instantiate(chara, transform.position, transform.rotation);
        newp.GetComponent<PlayerGeneric>().PlayerIndex = playerIndex;
        newp.GetComponent<PlayerGeneric>().IsCpuPlayer = isCpuPlayer;
        newp.GetComponent<BattlerGeneric>().SpawnPoint = SpawnPoints.at(playerIndex).position;
        NetworkServer.Spawn(newp);
        if (!isCpuPlayer)
        {
            NetworkServer.ReplacePlayerForConnection(conn, newp, 0);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// 当场景准备好时，用真实的角色替换凭依
    /// </summary>
    /// <remarks>注意Yorisiro创建的时候场景载入还没完成，因此无法在Update中执行</remarks>
    public void OnSceneGetReady()
    {
        if (!isLocalPlayer && !(isCpuPlayer && isServer))
            return;
        CmdChangePlayer();
    }
}
