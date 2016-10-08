using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class UIGeneric : MonoBehaviour
{
    public Object HPBar;

    public void Register(bool isLocal)
    {
        if (isLocal)
        {
            DealRegister(isLocal, UISystem.LeftPanel);
        }
        else
        {
            DealRegister(isLocal, UISystem.RightPanel);
        }
    }

    protected virtual void DealRegister(bool isLocal, UIRegister panel)
    {
        panel.Register(HPBar, GetComponent<NetworkIdentity>());
    }

    protected virtual void Awake()
    {
    }
}
