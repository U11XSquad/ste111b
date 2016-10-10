using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SPGeneric : MonoBehaviour
{
    float spval;
    public float SP
    {
        get
        {
            return spval;
        }
        set
        {
            spval = Mathf.Clamp(value, 0.0f, 100.0f);
        }
    }

    /// <summary>
    /// SP显示条预制件
    /// </summary>
    public Object spbar;

    void UIRegister(bool isLocal, UIRegister panel)
    {
        panel.Register(spbar, GetComponent<NetworkIdentity>());
    }

    void Awake()
    {
        spval = 0;
        GetComponent<UIGeneric>().OnRegister += UIRegister;
    }

    void SPOnBlock()
    {
        SP += 1.0f;
    }

    void SPOnDamage(int damage)
    {
        SP += damage / 100.0f;
    }

    // Use this for initialization
    void Start()
    {
        var battler = GetComponent<BattlerGeneric>();
        battler.OnBlock += SPOnBlock;
        battler.OnDamage += SPOnDamage;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
