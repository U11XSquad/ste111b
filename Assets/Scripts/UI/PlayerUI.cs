using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class PlayerUI : MonoBehaviour
{
    protected NetworkIdentity player;
    public NetworkIdentity Player
    {
        get
        {
            return player;
        }
        set
        {
            player = value;
        }
    }

    virtual protected void OnGUI()
    {
        if (!player)
        {
            Destroy(gameObject);
        }
        else
        {
            OnDraw();
        }
    }

    virtual protected void OnDraw()
    {
    }

    virtual public void OnRegister(UIRegister panel, bool isLeft)
    {
    }
}
