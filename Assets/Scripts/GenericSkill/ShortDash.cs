using UnityEngine;
using System.Collections;

public class ShortDash : Skill
{
    [Tooltip("两次按键的输入间隔")]
    public float inputInterval = 0.1f;

    [Tooltip("移动距离，单位m")]
    public float MoveDistance = 5.0f;

    [Tooltip("移动时间，单位s")]
    public float MoveTime = 0.3f;

    [Tooltip("动画字符串")]
    public string animationString = "walking";

    protected override void Start()
    {
        base.Start();
        player.GetComponent<MovingGeneric>().ColliEvents += this.OnCollide;
    }

    public override bool InputDetermine()
    {
        //HINT:这个条件可能会有一段时间被多次判定成立，不过并不认为这会造成bug
        return input.TestDash(inputInterval);
    }

    public override void SkillStart(bool isServer)
    {
        base.SkillStart(isServer);
        phase = SkillPhase.Recovery;

        var animator = Model.GetComponent<Animator>();
        animator.SetBool(animationString, true);

        //转向
        player.GetComponent<MovingGeneric>().FaceTo(input.Move);

        //创建移动
        var movSpd = input.Move;
        movSpd.Normalize();
        movSpd *= MoveDistance;
        player.GetComponent<MovingGeneric>().AddDisplace(movSpd, MoveTime, this);

        Invoke("DoEnd", MoveTime);
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

    void DoEnd()
    {
        manager.SkillCancel();
    }

    void OnCollide(Collision collisionInfo)
    {
        //撞东西后停止
        CancelInvoke();
        manager.SkillCancel();
    }
}