using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public UnitBase unitBase;

    public List<PassiveSkillBase> activePassives;
    public List<PassiveSkillBase> inactivePassives;

    private void Start()
    {
        unitBase = Instantiate(unitBase);
        SetPassives();
    }

    void SetPassives()
    {
        foreach (PassiveSkillBase activePassive in activePassives)
        {
            unitBase.GetPassives().Add(activePassive);
            activePassive.GiveBonus(unitBase);
        }

        foreach (PassiveSkillBase inactivePassive in inactivePassives)
        {
            unitBase.GetPassives().Add(inactivePassive);
        }
    }

    public ActivatableSkillBase GetActivatableSkillByName(string name)
    {
        foreach (ActivatableSkillBase skill in unitBase.GetActivatables())
        {
            if (name.ToLower() == skill.skillName.ToLower())
            {
                return skill;
            }
        }
        return null;
    }

    public PassiveSkillBase GetPassiveSkillByName(string name)
    {
        foreach (PassiveSkillBase skill in unitBase.GetPassives())
        {
            if (name.ToLower() == skill.skillName.ToLower())
            {
                return skill;
            }
        }
        return null;
    }

    public PassiveSkillBase GetActivePassiveSkillByName(string name)
    {
        foreach (PassiveSkillBase skill in activePassives)
        {
            if (name.ToLower() == skill.skillName.ToLower())
            {
                return skill;
            }
        }
        return null;
    }

    public PassiveSkillBase GetInactivePassiveSkillByName(string name)
    {
        foreach (PassiveSkillBase skill in inactivePassives)
        {
            if (name.ToLower() == skill.skillName.ToLower())
            {
                return skill;
            }
        }
        return null;
    }

    public void StealStats(UnitBase unit)
    {
        unitBase.maxHealth += unit.maxHealth * 0.1f;
        unitBase.attackPower += unit.attackPower * 0.1f;
        unitBase.agility += unit.agility * 0.1f;
        unitBase.defence += unit.defence * 0.1f;
        unitBase.Heal(unit.maxHealth * 0.2f);
    }

}
