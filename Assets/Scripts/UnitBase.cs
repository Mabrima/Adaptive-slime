using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/UnitBase")]
public class UnitBase : ScriptableObject
{
    [Header("Name")]
    public string unitName;
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

    public enum Environment { FORREST, DESERT, JUNGLE, SEA, MOUNTAINS }

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
            Heal(healingOverTimeAmount, "heal over time");
            healingDuration--;
        }
        if (bleedingDuration > 0 && poisonedDuration > 0)
        {
            TakeDmg(bleedingDmg + poisonedDmg, GameManager.DmgTypes.NULL, unitName + "s corrupted blood");
            bleedingDuration--;
            poisonedDuration--;
        }
        else if (bleedingDuration > 0)
        {
            TakeDmg(bleedingDmg, GameManager.DmgTypes.BLEED, unitName + "s bleeding wound");
            bleedingDuration--;
        }
        else if (poisonedDuration > 0)
        {
            TakeDmg(poisonedDmg, GameManager.DmgTypes.POISON, unitName + "s poisioned body");
            poisonedDuration--;
        }
    }

    public void Heal(float healAmount, string skillname)
    {
        currentHealth += healAmount;
        currentHealth = currentHealth > maxHealth ? maxHealth : currentHealth;
        GameManager.instance.HealPrint(healAmount, unitName, skillname);
    }

    public void Defend(float otherDmg, GameManager.DmgTypes dmgType, float otherAgility, string skillName, string otherName, bool trueHit = false)
    {
        if (trueHit)
        {
            TakeDmg(otherDmg, dmgType, otherName);
            GameManager.instance.CombatPrint(otherDmg, true, otherName, skillName);
        }
        if (Random.Range(0, 1f) < (otherAgility / agility))
        {
            TakeDmg(otherDmg, dmgType, otherName, skillName);
        }
        else
        {
            GameManager.instance.CombatPrint(otherDmg, false, otherName, skillName);
        }
    }

    public void TakeDmg(float otherDmg, GameManager.DmgTypes dmgType, string otherName, string skillName = "")
    {
        float tempDmg;
        switch (dmgType)
        {
            case GameManager.DmgTypes.CRUSH:
                tempDmg = Mathf.Clamp(otherDmg - defence, 0, 9999) * crushResistance;
                currentHealth = currentHealth - tempDmg;
                GameManager.instance.CombatPrint(tempDmg, true, otherName, skillName);
                break;
            case GameManager.DmgTypes.PIERCE:
                tempDmg = Mathf.Clamp(otherDmg - defence, 0, 9999) * pierceResistance;
                currentHealth = currentHealth - tempDmg;
                GameManager.instance.CombatPrint(tempDmg, true, otherName, skillName);
                break;
            case GameManager.DmgTypes.SLASH:
                tempDmg = Mathf.Clamp(otherDmg - defence, 0, 9999) * slashResistance;
                currentHealth = currentHealth - tempDmg;
                GameManager.instance.CombatPrint(tempDmg, true, otherName, skillName);
                break;
            case GameManager.DmgTypes.POISON:
                tempDmg = otherDmg * poisonResistance;
                currentHealth = currentHealth - tempDmg;
                GameManager.instance.PoisonPrint(tempDmg, otherName);
                break;
            case GameManager.DmgTypes.BLEED:
                tempDmg = otherDmg * bleedResistance;
                currentHealth = currentHealth - tempDmg;
                GameManager.instance.BleedPrint(tempDmg, otherName);
                break;
            default:
                tempDmg = otherDmg;
                currentHealth = currentHealth - tempDmg;
                GameManager.instance.CombatPrint(tempDmg, true, otherName, skillName);
                break;
        }

    }
}
