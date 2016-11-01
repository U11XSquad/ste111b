using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CharacterManager : NetworkBehaviour
{
    public PlayerGeneric[] avatars;
    static CharacterManager instance;
    static public CharacterManager Instance
    {
        get
        {
            return instance;
        }
    }

    static public PlayerGeneric[] Avatars
    {
        get
        {
            return Instance.avatars;
        }
    }

    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public override void OnStartClient()
    {
        System.Array.ForEach(avatars, avatar => ClientScene.RegisterPrefab(avatar.gameObject));
    }
}
