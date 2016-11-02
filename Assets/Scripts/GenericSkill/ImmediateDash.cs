using UnityEngine;
using System.Collections;

public class ImmediateDash : Skill
{
    [Tooltip("两次按键的输入间隔")]
    public float inputInterval = 0.1f;

    [Tooltip("移动距离，单位m")]
    public float MoveDistance = 5.0f;

    [Tooltip("移动前后摇时间（分别），单位s")]
    public float MoveTime = 0.08f;

    [Tooltip("前后摇中移动距离，单位m")]
    public float PrepareDistance = 0.8f;

    [Tooltip("动画字符串")]
    public string animationString = "walking";

    Vector3 movDir;

    public override bool InputDetermine()
    {
        return input.TestDash(inputInterval);
    }

    public override void SkillBreak(bool isServer)
    {
        base.SkillBreak(isServer);

        var animator = Model.GetComponent<Animator>();
        animator.SetBool(animationString, false);

        //删除移动（如果有的话）
        player.GetComponent<MovingGeneric>().RemoveDisplace(this);

        //取消Invoke
        CancelInvoke();
    }

    public override void SkillStart(bool isServer)
    {
        base.SkillStart(isServer);
        phase = SkillPhase.Startup;

        var animator = Model.GetComponent<Animator>();
        animator.SetBool(animationString, true);

        //转向
        player.GetComponent<MovingGeneric>().FaceTo(input.Move);

        //第一段位移
        movDir = input.Move;
        movDir.Normalize();
        var movDis = movDir * PrepareDistance;
        player.GetComponent<MovingGeneric>().AddDisplace(movDis, MoveTime, this);

        Invoke("DoActive", MoveTime);
    }

    void DoActive()
    {
        phase = SkillPhase.Recovery;

        //第二段位移
        var movDis = movDir * MoveDistance;
        player.GetComponent<MovingGeneric>().ImmediateDisplace(movDis);

        //第三段位移
        movDis = movDir * PrepareDistance;
        player.GetComponent<MovingGeneric>().AddDisplace(movDis, MoveTime, this);

        Invoke("DoEnd", MoveTime);
    }

    void DoEnd()
    {
        manager.SkillCancel();
    }
}