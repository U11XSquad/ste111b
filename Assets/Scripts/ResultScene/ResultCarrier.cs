using UnityEngine;
using System.Collections;

public class ResultCarrier : MonoBehaviour
{
    public enum Result
    {
        Win,
        Lose,
        Tie
    }
    public Result result;

    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
