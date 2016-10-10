using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class UIGeneric : MonoBehaviour
{
    public delegate void RegisterEvent(bool isLocal, UIRegister panel);
    public event RegisterEvent OnRegister;
    bool startRun = false;

    void Start()
    {
        startRun = false;
    }

    protected virtual void AfterStart()
    {
        //注册UI
        var player = GetComponent<PlayerGeneric>();
        if (player.Occup == PlayerGeneric.Occupation.Player)
        {
            Register(true);
        }
        else if (player.Occup == PlayerGeneric.Occupation.Opponent)
        {
            Register(false);
        }

        startRun = true;
    }

    void Update()
    {
        if (!startRun)
        {
            AfterStart();
        }
    }

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
        OnRegister(isLocal, panel);
    }

    protected virtual void Awake()
    {
    }
}
