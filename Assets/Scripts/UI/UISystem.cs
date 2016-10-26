using UnityEngine;
using System.Collections;

public class UISystem : MonoBehaviour
{
    static UISystem instance;
    static public UISystem Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<UISystem>();
                if (!instance)
                {
                    throw new UnityException("缺乏UISystem");
                }
            }
            return instance;
        }
    }

    public UIRegister leftPanel;
    static public UIRegister LeftPanel
    {
        get
        {
            return Instance.leftPanel;
        }
    }

    public UIRegister rightPanel;
    static public UIRegister RightPanel
    {
        get
        {
            return Instance.rightPanel;
        }
    }

    public UIGlobalRegister fullPanel;
    static public UIGlobalRegister FullPanel
    {
        get
        {
            return Instance.fullPanel;
        }
    }
}
