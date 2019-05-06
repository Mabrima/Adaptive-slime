using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public UnitBase unitBase;
    protected UnitBase originalBase;
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

    private float metaHealthBoost = 150;
    private float metaPowerBoost = 20;
    private float metaDefenceBoost = 10;
    private int metamorphEquipAmount = 5;

    public void GenerateMetaMorph()
    {
        //Copy and equip the player unitBase
        unitBase = Instantiate(GameManager.instance.player.unitBase);
        unitBase.InitializeWithoutPassives();
        unitBase.ResetCooldowns();
        //remove all active equips
        foreach (PassiveSkillBase skill in GameManager.instance.player.activeEquipables)
        {
            skill.RemoveBonus(unitBase);
        }

        //make a new more potent unitBase
        unitBase.maxHealth += metaHealthBoost;
        unitBase.currentHealth = Mathf.FloorToInt(unitBase.maxHealth);
        unitBase.abilityPower += metaPowerBoost;
        unitBase.defence += metaDefenceBoost;

        //Give some random equips the player found. Can be duplicate.
        for (int i = 0; i < metamorphEquipAmount; i++)
            unitBase.GetPassives()[Random.Range(0, unitBase.GetPassives().Count)].GiveBonus(unitBase);

        //fix name
        unitBase.unitName = "<color=red>***</color>";
    }
}
