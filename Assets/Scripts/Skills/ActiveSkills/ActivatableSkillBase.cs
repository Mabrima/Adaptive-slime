﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/ActivatableSkills")]
public class ActivatableSkillBase : ScriptableObject
{
    public enum Target { NULL, ENEMY, SELF }

    [Header("Dmg Stats")]
    public string skillName = "unNamed";
    public string description = "noDescription";


    [Header("Dmg Stats")]
    public float baseDmg = 0;
    public float scalingDmg = 0;
    public float basePoison = 0;
    public float scalingPoison = 0;
    public float poisonDuration = 0;
    public float baseBleed = 0;
    public float scalingBleed = 0;
    public float bleedDuration = 0;
    public GameManager.DmgTypes dmgType = 0;

    [Header("Target")]
    public Target target = 0;

    [Header("Self Modification")]
    public float crushResistance = 1;
    public float slashResistance = 1;
    public float pierceResistance = 1;
    public float bleedResistance = 1;
    public float poisonResistance = 1;
    public float baseHeal = 0;
    public float scalingHeal = 0;
    public float baseHealOverTime = 0;
    public float scalingHealOverTime = 0;




    public virtual void UseSkill()
    {

    }

    public virtual void CombineSkill()
    {

    }
    
}
