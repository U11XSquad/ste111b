﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class UIRegister : MonoBehaviour
{
    public bool isLeft;
    public float border;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject Register(Object prefab, NetworkIdentity player)
    {
        return Register(prefab, player, border);
    }

    public GameObject Register(Object prefab, NetworkIdentity player, float border)
    {
        var ret = (GameObject)Instantiate(prefab, transform);
        ret.GetComponent<PlayerUI>().Player = player;

        var rectTrans = ret.GetComponent<RectTransform>();

        //左右对齐调整
        if (isLeft)
        {
            rectTrans.anchorMin = new Vector2(0, rectTrans.anchorMin.y);
            rectTrans.anchorMax = new Vector2(0, rectTrans.anchorMax.y);
            rectTrans.pivot = new Vector2(0, rectTrans.pivot.y);
            var pos = rectTrans.position;
            pos.x = border;
            pos.y += 480.0f;
            rectTrans.position = pos;
        }
        else
        {
            rectTrans.anchorMin = new Vector2(1, rectTrans.anchorMin.y);
            rectTrans.anchorMax = new Vector2(1, rectTrans.anchorMax.y);
            rectTrans.pivot = new Vector2(1, rectTrans.pivot.y);
            var pos = rectTrans.position;
            pos.x = -border;
            pos.x += 640.0f;
            pos.y += 480.0f;
            rectTrans.position = pos;
        }

        return ret;
    }
}
