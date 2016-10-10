using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SPGeneric : MonoBehaviour
{
    string scName;
    public string SCName
    {
        get
        {
            return scName;
        }
    }

    bool inSpell;
    public bool InSpell
    {
        get
        {
            return inSpell;
        }
    }

    float spval;
    public float SP
    {
        get
        {
            return spval;
        }
        set
        {
            spval = Mathf.Clamp(value, 0.0f, 100.0f);
        }
    }

    [Tooltip("SP显示条预制件")]
    public Object spbar;

    [Tooltip("SC显示条预制件")]
    public Object scbar;

    /// <summary>
    /// SC中SP减少的速率
    /// </summary>
    float lostSpeed;

    /// <summary>
    /// SC发动后的技能
    /// </summary>
    Skill spellSkill;

    void UIRegister(bool isLocal, UIRegister panel)
    {
        panel.Register(spbar, GetComponent<NetworkIdentity>());
        panel.Register(scbar, GetComponent<NetworkIdentity>());
    }

    void Awake()
    {
        spval = 0;
        scName = "";
        inSpell = false;
        GetComponent<UIGeneric>().OnRegister += UIRegister;
    }

    void SPOnBlock()
    {
        SP += 1.0f;
    }

    void SPOnDamage(int damage)
    {
        SP += damage / 100.0f;
    }

    void Start()
    {
        var battler = GetComponent<BattlerGeneric>();
        battler.OnBlock += SPOnBlock;
        battler.OnDamage += SPOnDamage;
    }

    public void SpellEnd()
    {
        scName = "";
        inSpell = false;
        spval = 0.0f;
        lostSpeed = 0.0f;
        spellSkill.status = Skill.SkillStatus.Disable;
    }

    public void SpellStart(Skill spellSkill, float lastTime)
    {
        this.spellSkill = spellSkill;
        spellSkill.status = Skill.SkillStatus.Inactive;
        lostSpeed = 100.0f * Time.fixedDeltaTime / lastTime;
        inSpell = true;
        scName = spellSkill.skillName;
    }

    void FixedUpdate()
    {
        if (inSpell)
        {
            spval -= lostSpeed;
            if (spval <= 0.0f)
            {
                SpellEnd();
            }
        }
    }
}
