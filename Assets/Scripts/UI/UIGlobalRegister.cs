using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class UIGlobalRegister : MonoBehaviour
{
    public GameObject Register(Object prefab)
    {
        var ret = (GameObject)Instantiate(prefab, transform);

        var rectTrans = ret.GetComponent<RectTransform>();
        var pos = rectTrans.position;
        pos.x += Screen.width * rectTrans.anchorMin.x;
        pos.y += Screen.height * rectTrans.anchorMin.y;
        rectTrans.position = pos;

        return ret;
    }
}
