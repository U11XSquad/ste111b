using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultTransfer : MonoBehaviour
{
    public Text winText;

    // Use this for initialization
    void Start()
    {
        var result = FindObjectOfType<ResultCarrier>();
        winText.text = result.result.ToString();
        Destroy(result.gameObject);
    }
}
