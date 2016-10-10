using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MOMSpellSkill : Skill
{
    public GameObject bulletPrefab;

    public override bool InputDetermine()
    {
        return input.STrigger;
    }

    public override void SkillStart(bool isServer)
    {
        base.SkillStart(isServer);

        //--begin--
        if (isServer)
        {
            float[] range06 = {50.0f, 75.0f, 60.0f};
            //内圈


            //var bullet = HitBox.Create(bulletPrefab, pos, player.transform.rotation, this, 100.0f);
            //NetworkServer.Spawn(bullet);
        }
        //--end--

        Invoke("DoEnd", 3.0f);
        player.GetComponent<SPGeneric>().SpellEnd();
    }

    void DoEnd()
    {
        phase = SkillPhase.None;
        status = SkillStatus.Inactive;
    }
}
