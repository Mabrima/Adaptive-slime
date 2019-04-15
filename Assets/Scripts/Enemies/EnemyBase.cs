using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public UnitBase unitBase;

    private void Start()
    {
        unitBase = Instantiate(unitBase);
    }

    public ActivatableSkillBase GetRandomSkill()
    {
        return unitBase.GetActivatables()[Random.Range(0, unitBase.GetActivatables().Count)];
    }
}
