using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerGeneric : NetworkBehaviour
{
    public GameObject model;

    public void FaceTo(Vector3 dir)
    {
        var ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        ang = 450 - ang; //调整摄像机时，此数值需变动
        transform.rotation = Quaternion.AngleAxis(ang, Vector3.up);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
