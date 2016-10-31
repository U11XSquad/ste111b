using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class SpawnPoints : MonoBehaviour
{
    static SpawnPoints instance;
    static public SpawnPoints Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<SpawnPoints>();
                if (!instance)
                {
                    throw new UnityException("缺乏SpawnPoints");
                }
            }
            return instance;
        }
    }

    public Transform[] spawnPoints;
    public Transform this[int idx]
    {
        get
        {
            return spawnPoints[idx];
        }
        set
        {
            spawnPoints[idx] = value;
        }
    }

    static public Transform at(int idx)
    {
        return Instance[idx];
    }
}
