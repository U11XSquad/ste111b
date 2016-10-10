using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class SPBar : PlayerUI
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    override protected void OnDraw()
    {
        var text = GetComponent<Text>();
        var spgeneric = this.player.GetComponent<SPGeneric>();
        text.text = string.Format("{0:f0}%", spgeneric.SP);
    }
}
