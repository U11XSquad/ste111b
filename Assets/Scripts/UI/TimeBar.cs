using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class TimeBar : MonoBehaviour
{
    void OnGUI()
    {
        var text = GetComponent<Text>();
        if (TeamManager.GameFormat == Prototype.NetworkLobby.GameFormat.Training)
        {
            text.text = "∞";
        }
        else
        {
            text.text = string.Format("{0}", Mathf.CeilToInt(TeamManager.RemainTime));
        }
    }
}
