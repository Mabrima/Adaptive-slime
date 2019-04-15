using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/UnitBase")]
public class UnitBase : ScriptableObject
{
    [Header("Skills")]
    [SerializeField]
    protected List<PassiveSkillBase> passives;
    [SerializeField]
    protected List<ActivatableSkillBase> activatables;
    [Header("Stats")]
    public float maxHealth = 100;
    public float currentHealth = 100;
    public float attackPower = 10;
    public float agility = 10;
    public float defence = 10;

    public float pierceResistance = 1;
    public float slashResistance = 1;
    public float crushResistance = 1;
    public float bleedResistance = 1;
    public float poisonResistance = 1;

    [Header("StatusEffects")]
    public float poisonedDmg = 0;
    public float poisonedDuration = 0;
    public float bleedingDmg = 0;
    public float bleedingDuration = 0;

    [Header("Special Bonuses")]
    public int poisionBonusDuration = 0;
    public int bleedBonusDuration = 0;

    void Start()
    {
        ActivatePassives();
    }

    public virtual void DoCombat()
    {
        activatables[Random.Range(0, activatables.Count)].UseSkill();
    }

    public virtual void ActivatePassives()
    {
        foreach (PassiveSkillBase passive in passives)
        {
            passive.GiveBonus(this);
        }
    }

    public virtual List<PassiveSkillBase> GetPassives()
    {
        return passives;
    }

    public virtual List<ActivatableSkillBase> GetActivatables()
    {
        return activatables;
    }

    public void SetPoisoned(float dmg, int duration)
    {
        poisonedDmg = dmg;
        poisonedDuration = duration;
    }

    public void SetBleeding(float dmg, int duration)
    {
        bleedingDmg = dmg;
        bleedingDuration = duration;
    }

    public void Defend(float otherDmg, GameManager.DmgTypes dmgType, float attackAccuracy, float otherAgility, bool trueHit = false)
    {
        if (trueHit)
        {
            TakeDmg(otherDmg, dmgType);
        }
        if (Random.Range(0, 1f) < ((float)agility/otherAgility) * attackAccuracy)
        {

        }
    }

    public void TakeDmg(float otherDmg, GameManager.DmgTypes dmgType)
    {
        switch (dmgType)
        {
            case GameManager.DmgTypes.CRUSH:
                currentHealth = currentHealth - Mathf.Abs(otherDmg) * crushResistance;
                break;
            case GameManager.DmgTypes.PIERCE:
                currentHealth = currentHealth - Mathf.Abs(otherDmg) * pierceResistance;
                break;
            case GameManager.DmgTypes.SLASH:
                currentHealth = currentHealth - Mathf.Abs(otherDmg) * slashResistance;
                break;
            default:
                currentHealth = currentHealth - Mathf.Abs(otherDmg);
                break;
        }

    }
}
