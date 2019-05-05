using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/UnitBase")]
public class UnitBase : ScriptableObject
{
    [Header("Name")]
    public string unitName = "unitBase";

    [Header("Area")]
    public Area unitArea = Area.FORREST;

    [Header("Skills")]
    [SerializeField]
    protected List<PassiveSkillBase> passives;
    [SerializeField]
    protected List<ActivatableSkillBase> activatables;

    protected List<ActivatableSkillBase> activatablesOffCooldown;
    protected List<ActivatableSkillBase> activatablesOnCooldown;

    [Header("Stats")]
    public float maxHealth = 100;
    public float currentHealth = 100;
    public float abilityPower = 10;
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

    public void Initialize()
    {
        ActivatePassives();
        MakeActivatablesIntoCopies();
        activatablesOffCooldown = new List<ActivatableSkillBase>();
        activatablesOnCooldown = new List<ActivatableSkillBase>();
        AddInitialSkillsToOffCooldown();
    }

    public void ActivatePassives()
    {
        foreach (PassiveSkillBase passive in passives)
        {
            passive.GiveBonus(this);
        }
    }

    private void MakeActivatablesIntoCopies()
    {
        for (int i = 0; i < activatables.Count; i++)
        {
            activatables.Add(Instantiate(activatables[0]));
            activatables.RemoveAt(0);
        }
    }

    private void AddInitialSkillsToOffCooldown()
    {
        foreach (ActivatableSkillBase skill in activatables)
        {
            activatablesOffCooldown.Add(skill);
        }
    }

    public List<PassiveSkillBase> GetPassives()
    {
        return passives;
    }

    public List<ActivatableSkillBase> GetActivatables()
    {
        return activatables;
    }

    public List<ActivatableSkillBase> GetSkillsOffCooldown()
    {
        return activatablesOffCooldown;
    }

    public void AddSkillToActivatableSkills(ActivatableSkillBase skill)
    {
        activatablesOffCooldown.Add(skill);
        activatables.Add(skill);
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

    /// <summary>
    /// Handles all the units end of turn effects.
    /// </summary>
    public void EndOfTurnEffects()
    {
        if (healingDuration > 0)
        {
            Heal(healingOverTimeAmount, "heal over time");
            healingDuration--;
        }
        if (bleedingDuration > 0 && poisonedDuration > 0)
        {
            TakeDmg(bleedingDmg + poisonedDmg, DmgTypes.NULL, unitName);
            bleedingDuration--;
            poisonedDuration--;
        }
        else if (bleedingDuration > 0)
        {
            TakeDmg(bleedingDmg, DmgTypes.BLEED, unitName);
            bleedingDuration--;
        }
        else if (poisonedDuration > 0)
        {
            TakeDmg(poisonedDmg, DmgTypes.POISON, unitName);
            poisonedDuration--;
        }
    }

    /// <summary>
    /// Handles healing the unit recieves.
    /// </summary>
    /// <param name="healAmount"></param>
    /// <param name="skillname"></param>
    public void Heal(float healAmount, string skillname)
    {
        currentHealth += healAmount;
        currentHealth = currentHealth > maxHealth ? maxHealth : currentHealth;
        GameManager.instance.HealPrint(healAmount, unitName, skillname);
    }

    /// <summary>
    /// Future purpose, handling hitchance of abilities. For now just calls take dmg
    /// </summary>
    /// <param name="otherDmg"></param>
    /// <param name="dmgType"></param>
    /// <param name="skillName"></param>
    /// <param name="otherName"></param>
    public void Defend(float otherDmg, DmgTypes dmgType, string skillName, string otherName)
    {
        TakeDmg(otherDmg, dmgType, otherName, skillName);
    }

    /// <summary>
    /// Handles taking dmg from other units skills.
    /// </summary>
    /// <param name="otherDmg"></param>
    /// <param name="dmgType"></param>
    /// <param name="otherName"></param>
    /// <param name="skillName"></param>
    private void TakeDmg(float otherDmg, DmgTypes dmgType, string otherName, string skillName = "")
    {
        float tempDmg;
        switch (dmgType)
        {
            case DmgTypes.CRUSH:
                tempDmg = Mathf.Clamp((otherDmg - defence) * crushResistance * Random.Range(0.8f, 1.2f), 1, 9999);
                currentHealth = currentHealth - tempDmg;
                GameManager.instance.CombatPrint(tempDmg, true, otherName, unitName, skillName);
                break;
            case DmgTypes.PIERCE:
                tempDmg = Mathf.Clamp((otherDmg - defence) * pierceResistance * Random.Range(0.8f, 1.2f), 1, 9999);
                currentHealth = currentHealth - tempDmg;
                GameManager.instance.CombatPrint(tempDmg, true, otherName, unitName,skillName);
                break;
            case DmgTypes.SLASH:
                tempDmg = Mathf.Clamp((otherDmg - defence) * slashResistance * Random.Range(0.8f, 1.2f), 1, 9999);
                currentHealth = currentHealth - tempDmg;
                GameManager.instance.CombatPrint(tempDmg, true, otherName, unitName, skillName);
                break;
            case DmgTypes.POISON:
                tempDmg = otherDmg * poisonResistance;
                currentHealth = currentHealth - tempDmg;
                GameManager.instance.PoisonPrint(tempDmg, otherName);
                break;
            case DmgTypes.BLEED:
                tempDmg = otherDmg * bleedResistance;
                currentHealth = currentHealth - tempDmg;
                GameManager.instance.BleedPrint(tempDmg, otherName);
                break;
            default:
                tempDmg = otherDmg;
                currentHealth = currentHealth - tempDmg;
                GameManager.instance.CorruptedBloodPrint(tempDmg, otherName);
                break;
        }
    }

    /// <summary>
    /// Decrements the cooldownTimer of all skils that are on cooldown by 1
    /// </summary>
    public void DecrementCooldowns()
    {
        for (int i = 0; i < activatablesOnCooldown.Count; i++)
        {
            activatablesOnCooldown[i].DecrementCooldown();
            if (activatablesOnCooldown[i].cooldownTimer == 0)
            {
                activatablesOffCooldown.Add(activatablesOnCooldown[i]);
                activatablesOnCooldown.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>
    /// Sets a skill on cooldown if the skill has a nonzero cooldown time
    /// </summary>
    /// <param name="skill"></param>
    public void SetSkillOnCooldown(ActivatableSkillBase skill)
    {
        if (skill.cooldown > 0)
        {
            skill.SetCooldownTimer();
            activatablesOffCooldown.Remove(skill);
            activatablesOnCooldown.Add(skill);
        }
    }

    /// <summary>
    /// Resets all cooldowns to 0 and puts them in the offColdown list
    /// </summary>
    public void ResetCooldowns()
    {
        for (int i = 0; i < activatablesOnCooldown.Count; i++)
        {
            activatablesOnCooldown[0].cooldownTimer = 0;
            activatablesOffCooldown.Add(activatablesOnCooldown[0]);
            activatablesOnCooldown.RemoveAt(0);
        }
    }
}
