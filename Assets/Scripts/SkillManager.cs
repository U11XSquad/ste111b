using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class SkillManager : NetworkBehaviour
{

    public Skill[] skills;

    public Skill current = null;

    // Use this for initialization
    void Start()
    {
        current = skills[0];
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].SkillNo = i;
        }
        current.SkillStart(isServer);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (current)
        {
            current.Process(isServer);
        }
    }

    [Command]
    public void CmdCast(int skillNo)
    {
        RpcSkillStart(skillNo);
    }

    [ClientRpc]
    void RpcSkillStart(int skillNo)
    {
        ///current.SkillBreak;
        current = skills[skillNo];
        current.SkillStart(isServer);
    }
}
