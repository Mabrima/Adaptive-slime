using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public UnitBase unitBase;
    private UnitBase originalBase;
    /// <summary>
    /// For letting the AI control unitbases as enemies.
    /// </summary>
    private void Start()
    {
        originalBase = unitBase;
        ResetEnemy();
    }

    public ActivatableSkillBase GetRandomSkill()
    {
        return unitBase.GetSkillsOffCooldown()[Random.Range(0, unitBase.GetSkillsOffCooldown().Count)];
    }

    public void ResetEnemy()
    {
        unitBase = Instantiate(originalBase);
        unitBase.maxHealth *= Random.Range(0.9f, 1.1f);
        unitBase.currentHealth = Mathf.FloorToInt(unitBase.maxHealth);
        unitBase.abilityPower *= Random.Range(0.9f, 1.1f);
        unitBase.defence *= Random.Range(0.9f, 1.1f);
        unitBase.Initialize();
    }

    public UnitBase GetOriginalBase()
    {
        return originalBase;
    }
}
