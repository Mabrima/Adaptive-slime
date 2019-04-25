using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public UnitBase unitBase;
    /// <summary>
    /// For letting the AI control unitbases as enemies.
    /// </summary>

    private void Start()
    {
        unitBase = Instantiate(unitBase);
        unitBase.Initialize();
    }

    public ActivatableSkillBase GetRandomSkill()
    {
        return unitBase.GetSkillsOffCooldown()[Random.Range(0, unitBase.GetSkillsOffCooldown().Count)];
    }

    public void ResetEnemy()
    {
        unitBase.currentHealth = unitBase.maxHealth;
        unitBase.ResetCooldowns();
        
    }
}
