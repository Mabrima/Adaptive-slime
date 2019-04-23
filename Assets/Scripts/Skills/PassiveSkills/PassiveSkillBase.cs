using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/PassiveSkills")]
public class PassiveSkillBase : ScriptableObject
{

    [Header("Dmg Stats")]
    public string skillName = "unNamed";
    public string description = "noDescription";


    [Header("Stats")]
    public float attackPower = 0;
    public float attackPowerPercent = 1;
    public float agility = 0;
    public float agilityPercent = 1;
    public float defence = 0;
    public float defencePercent = 1;
    public float health = 0;
    public float healthPercent = 1;

    [Header("Defence")]
    public float crushResistance = 1;
    public float slashResistance = 1;
    public float pierceResistance = 1;
    public float bleedResistance = 1;
    public float poisonResistance = 1;

    [Header("Special Effects")]
    public int bleedDurationBonus = 0;
    public int poisonDurationBonus = 0;


    public void GiveBonus(UnitBase unit)
    {
        unit.attackPower += attackPower;
        unit.attackPower *= attackPowerPercent;
        unit.agility += agility;
        unit.agility *= agilityPercent;
        unit.defence += defence;
        unit.defence *= defencePercent;
        unit.maxHealth += health;
        unit.currentHealth += health;
        unit.maxHealth *= healthPercent;
        unit.currentHealth *= healthPercent;

        unit.crushResistance *= crushResistance;
        unit.pierceResistance *= pierceResistance;
        unit.slashResistance *= slashResistance;
        unit.poisonResistance *= poisonResistance;
        unit.bleedResistance *= bleedResistance;

        unit.poisionBonusDuration += poisonDurationBonus;
        unit.bleedBonusDuration += bleedDurationBonus;
    }

    public void RemoveBonus(UnitBase unit)
    {
        unit.attackPower -= attackPower;
        unit.attackPower /= attackPowerPercent;
        unit.agility -= agility;
        unit.agility /= agilityPercent;
        unit.defence -= defence;
        unit.defence /= defencePercent;
        unit.maxHealth -= health;
        unit.currentHealth -= health;
        unit.maxHealth /= healthPercent;
        unit.currentHealth /= healthPercent;

        unit.crushResistance /= crushResistance;
        unit.pierceResistance /= pierceResistance;
        unit.slashResistance /= slashResistance;
        unit.poisonResistance /= poisonResistance;
        unit.bleedResistance /= bleedResistance;

        unit.poisionBonusDuration -= poisonDurationBonus;
        unit.bleedBonusDuration -= bleedDurationBonus;
    }



  
}
