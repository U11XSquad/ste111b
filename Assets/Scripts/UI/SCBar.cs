using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class SCBar : PlayerUI
{

    override protected void OnDraw()
    {
        var text = GetComponent<Text>();
        var spgeneric = this.player.GetComponent<SPGeneric>();
        text.text = string.Format("{0}", spgeneric.SCName);
    }

    public override void OnRegister(UIRegister panel, bool isLeft)
    {
        if (isLeft)
        {
            GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
        }
        else
        {
            GetComponent<Text>().alignment = TextAnchor.MiddleRight;
        }
    }
}
