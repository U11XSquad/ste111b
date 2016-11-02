using UnityEngine;
using System.Collections;

public class LongDash : Skill
{
    [Tooltip("两次按键的输入间隔")]
    public float inputInterval = 0.1f;

    [Tooltip("移动速度，单位m/s")]
    public float MoveSpeed = 10.0f;

    [Tooltip("固定移动时间，单位s")]
    public float MoveTime = 0.1f;

    [Tooltip("收尾速度补正")]
    public float EndingSpeedCoe = 0.6f;

    [Tooltip("动画字符串")]
    public string animationString = "walking";

    Vector3 preMovSpd;
    bool isEnding;

    protected override void Start()
    {
        base.Start();
        player.GetComponent<MovingGeneric>().ColliEvents += this.OnCollide;
    }

    public override bool InputDetermine()
    {
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
        preMovSpd = movSpd;
        movSpd.Normalize();
        movSpd *= MoveSpeed;
        player.GetComponent<MovingGeneric>().AddDisplaceBySpeed(movSpd, Mathf.Infinity, this);
        isEnding = false;
    }

    public override void SkillBreak(bool isServer)
    {
        base.SkillBreak(isServer);

        var animator = Model.GetComponent<Animator>();
        animator.SetBool(animationString, false);

        //删除移动
        player.GetComponent<MovingGeneric>().RemoveDisplace(this);

        //取消Invoke
        CancelInvoke();
    }

    void DoEnd()
    {
        manager.SkillCancel();
    }

    public override void Process(bool isServer)
    {
        base.Process(isServer);

        if (!isEnding)
        {
            var playerGeneric = player.GetComponent<PlayerGeneric>();
            if (!playerGeneric.isLocalPlayer)
            {
                return; //非本地玩家不检测输入
            }

            if (!input.IsMoving || input.Move != preMovSpd)
            {
                preMovSpd.Normalize();
                preMovSpd *= MoveSpeed * EndingSpeedCoe;
                player.GetComponent<MovingGeneric>().RemoveDisplace(this);
                player.GetComponent<MovingGeneric>().AddDisplaceBySpeed(preMovSpd, MoveTime, this);
                isEnding = true;
                Invoke("DoEnd", MoveTime);
            }
        }   
    }

    void OnCollide(Collision collisionInfo)
    {
        //撞东西后停止
        CancelInvoke();
        manager.SkillCancel();
    }
}