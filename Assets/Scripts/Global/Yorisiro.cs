using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;

public class Yorisiro : NetworkBehaviour
{
    [SyncVar]
    protected string characterName;
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
        NetworkServer.Spawn(newp);
        NetworkServer.ReplacePlayerForConnection(conn, newp, 0);
        Destroy(gameObject);
    }

    /// <summary>
    /// 当场景准备好时，用真实的角色替换凭依
    /// </summary>
    /// <remarks>注意Yorisiro创建的时候场景载入还没完成，因此无法在Update中执行</remarks>
    public void OnSceneGetReady()
    {
        if (!isLocalPlayer)
            return;
        CmdChangePlayer();
    }
}
