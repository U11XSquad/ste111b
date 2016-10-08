using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class HPBar : PlayerUI
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void OnGUI()
    {
        var text = GetComponent<Text>();
        var battler = this.player.GetComponent<BattlerGeneric>();
        text.text = string.Format("{0:f0}/{1:f0}", battler.HP, battler.HPMax);
    }
}
