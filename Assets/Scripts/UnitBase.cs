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
    public int poisonedDuration = 0;
    public float bleedingDmg = 0;
    public int bleedingDuration = 0;
    public float healingOverTimeAmount = 0;
    public int healingDuration = 0;

    [Header("Special Bonuses")]
    public int poisionBonusDuration = 0;
    public int bleedBonusDuration = 0;
    public int healingBonusDuration = 0;

    void Start()
    {
        ActivatePassives();
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

    public void EndOfTurnEffects()
    {
        if (healingDuration > 0)
        {
            Heal(healingOverTimeAmount);
            healingDuration--;
        }
        if (bleedingDuration > 0)
        {
            TakeDmg(bleedingDmg, GameManager.DmgTypes.NULL, name + "s bleeding wound");
            bleedingDuration--;
        }
        if (poisionBonusDuration > 0)
        {
            TakeDmg(poisonedDmg, GameManager.DmgTypes.NULL, name + "s poisioned body");
            poisonedDuration--;
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = currentHealth > maxHealth ? maxHealth : currentHealth;
        GameManager.instance.HealPrint(healAmount, name);
    }

    public void Defend(float otherDmg, GameManager.DmgTypes dmgType, float otherAgility, string otherName, bool trueHit = false)
    {
        if (trueHit)
        {
            TakeDmg(otherDmg, dmgType, otherName);
            GameManager.instance.CombatPrint(otherDmg, true, otherName);
        }
        if (Random.Range(0, 1f) < (otherAgility / agility))
        {
            TakeDmg(otherDmg, dmgType, otherName);
        }
        else
        {
            GameManager.instance.CombatPrint(otherDmg, false, otherName);
        }
    }

    public void TakeDmg(float otherDmg, GameManager.DmgTypes dmgType, string otherName)
    {
        float tempDmg;
        switch (dmgType)
        {
            case GameManager.DmgTypes.CRUSH:
                tempDmg = Mathf.Clamp(otherDmg - defence, 0, 9999) * crushResistance;
                currentHealth = currentHealth - tempDmg;
                GameManager.instance.CombatPrint(tempDmg, true, otherName);
                break;
            case GameManager.DmgTypes.PIERCE:
                tempDmg = Mathf.Clamp(otherDmg - defence, 0, 9999) * pierceResistance;
                currentHealth = currentHealth - tempDmg;
                GameManager.instance.CombatPrint(tempDmg, true, otherName);
                break;
            case GameManager.DmgTypes.SLASH:
                tempDmg = Mathf.Clamp(otherDmg - defence, 0, 9999) * slashResistance;
                currentHealth = currentHealth - tempDmg;
                GameManager.instance.CombatPrint(tempDmg, true, otherName);
                break;
            default:
                tempDmg = otherDmg;
                currentHealth = currentHealth - tempDmg;
                GameManager.instance.CombatPrint(tempDmg, true, otherName);
                break;
        }

    }
}
