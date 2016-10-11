using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MOMSpellSkill : Skill
{
    public GameObject[] bullets;

    public override bool InputDetermine()
    {
        return input.STrigger;
    }

    public override void SkillStart(bool isServer)
    {
        base.SkillStart(isServer);
        if (isServer)
        {
            StartCoroutine(MountainOfMahjong());
        }
        Invoke("DoEnd", 6.0f);
        player.GetComponent<SPGeneric>().SpellEnd();
    }

    public override void SkillBreak(bool isServer)
    {
        base.SkillBreak(isServer);
        StopAllCoroutines();
    }

    void DoEnd()
    {
        phase = SkillPhase.None;
        status = SkillStatus.Disable;
    }

    IEnumerator MountainOfMahjong()
    {
        IEnumerator[] r = new IEnumerator[5];
        Vector3 center = transform.position;
        float rot = transform.rotation.eulerAngles.y;
        bool flag;

        //第一圈1环
        for (int i = 0; i < 5; i++)
        {
            var dis = 3.0f;
            var ang = rot + i * 72.0f;
            var cen = center + PolToVec(dis, rot + i * 72.0f);
            r[i] = GenerateRing(bullets[0], cen, dis, ang - 144f, 4.8f, ang + 144f, 30.0f);
        }
        flag = true;
        while (flag)
        {
            for (int i = 0; i < 5; i++)
            {
                if (!r[i].MoveNext())
                    flag = false;
            }
            yield return new WaitForFixedUpdate();
        }

        //第一圈2环
        rot += 36.0f;
        for (int i = 0; i < 5; i++)
        {
            var ang = rot + i * 72.0f;
            var cen = center + PolToVec(6.0f, rot + i * 72.0f);
            r[i] = GenerateRing(bullets[1], cen, 4.0f, ang - 90f, 5.0f, ang + 90f, 30.0f);
        }
        flag = true;
        while (flag)
        {
            for (int i = 0; i < 5; i++)
            {
                if (!r[i].MoveNext())
                    flag = false;
            }
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(1.0f);

        //第二圈1环
        rot += 36.0f;
        for (int i = 0; i < 5; i++)
        {
            var dis = 3.0f;
            var ang = rot + i * 72.0f;
            var cen = center + PolToVec(dis, rot + i * 72.0f);
            r[i] = GenerateRing(bullets[2], cen, dis, ang - 72f, 4.8f, ang + 180f, 30.0f);
        }
        flag = true;
        while (flag)
        {
            for (int i = 0; i < 5; i++)
            {
                if (!r[i].MoveNext())
                    flag = false;
            }
            yield return new WaitForFixedUpdate();
        }

        //第二圈2环
        rot += 36.0f;
        for (int i = 0; i < 5; i++)
        {
            var ang = rot + i * 72.0f;
            var cen = center + PolToVec(6.0f, rot + i * 72.0f);
            r[i] = GenerateRing(bullets[2], cen, 4.0f, ang - 90f, 5.0f, ang + 90f, 30.0f);
        }
        flag = true;
        while (flag)
        {
            for (int i = 0; i < 5; i++)
            {
                if (!r[i].MoveNext())
                    flag = false;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    static Vector3 PolToVec(float radius, float ang)
    {
        Vector3 ret = new Vector3(0, 0, 0);
        var rad = ang * Mathf.Deg2Rad;
        ret.z = radius * Mathf.Cos(rad);
        ret.x = radius * Mathf.Sin(rad);
        return ret;
    }

    IEnumerator GenerateRing(Object prefab, Vector3 center, float radius, float startAng, float stepAng, float endAng, float rotStep)
    {
        int cnt = 0;
        float rot = startAng - 90.0f + rotStep;
        float waitTime = 1.0f;

        for (var ang = startAng; ang < endAng || cnt % 4 != 0; ang += stepAng)
        {
            var pos = center + PolToVec(radius, ang);
            var quat = Quaternion.Euler(new Vector3(90.0f, rot + (cnt % 4) * 3.0f, 90.0f));

            var bullet = HitBox.Create(prefab, pos, quat, this, 30.0f);
            bullet.GetComponent<MOMHitBox>().waitTime = waitTime;
            NetworkServer.Spawn(bullet);

            cnt++;
            if (cnt % 4 == 0)
            {
                waitTime -= Time.fixedDeltaTime;
                rot = ang - 90.0f + rotStep;
                yield return cnt;
            }
        }
    }
}
