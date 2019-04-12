using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    protected PassiveSkillBase[] passives;
    protected ActivatableSkillBase[] activatables;
    protected int maxHealth = 100;
    protected int currentHealth = 100;
    protected int attackPower = 10;
    protected int agility = 10;
    protected int defence = 10;

    protected float pierceResistance = 1;
    protected float slashResistance = 1;
    protected float crusheResistance = 1;

    protected int poisonedDmg = 0;
    protected int poisonedDuration = 0;
    protected int bleedingDmg = 0;
    protected int bleedingDuration = 0;

    public abstract void DoCombat();

    public virtual PassiveSkillBase[] GetPassives()
    {
        return passives;
    }

    public virtual ActivatableSkillBase[] GetActivatables()
    {
        return activatables;
    }

    protected abstract void SetBaseSkills();

    public void SetPoisoned(int dmg, int duration)
    {
        poisonedDmg = dmg;
        poisonedDuration = duration;
    }

    public void SetBleeding(int dmg, int duration)
    {
        bleedingDmg = dmg;
        bleedingDuration = duration;
    }

    public void Defend(int otherDmg, GameManager.DmgTypes dmgType, float attackAccuracy, int otherAgility, bool trueHit = false)
    {
        if (trueHit)
        {
            TakeDmg(otherDmg, dmgType);
        }
        if (Random.Range(0, 1f) < ((float)agility/otherAgility) * attackAccuracy)
        {

        }
    }

    public void TakeDmg(int otherDmg, GameManager.DmgTypes dmgType)
    {
        switch (dmgType)
        {
            case GameManager.DmgTypes.CRUSH:
                currentHealth = (int) (currentHealth - Mathf.Abs(otherDmg) * crusheResistance);
                break;
            case GameManager.DmgTypes.PIERCE:
                currentHealth = (int) (currentHealth - Mathf.Abs(otherDmg) * pierceResistance);
                break;
            case GameManager.DmgTypes.SLASH:
                currentHealth = (int) (currentHealth - Mathf.Abs(otherDmg) * slashResistance);
                break;
            default:
                currentHealth = (int)(currentHealth - Mathf.Abs(otherDmg));
                break;
        }

    }
}
