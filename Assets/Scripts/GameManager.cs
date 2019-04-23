using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum DmgTypes { NULL, SLASH, PIERCE, CRUSH }
    public enum State { NULL, PLAYER_TURN, ENEMY_TURN, WORLD, DEALING_WITH_READAPT, GAME_OVER, END_OF_TURN }

    Player player;
    public Text inputText;
    public Text descriptionText;
    public Text worldText;
    public Text activePassivesText;
    public Text inactivePassivesText;
    public Text activatableAbilitiesText;
    public Text playerHealthText;
    public Text enemyHealthText;

    public static GameManager instance;
    public State currentState = State.NULL;
    public EnemyBase[] enemies;
    public EnemyBase currentEnemy;

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }

        else
        {
            instance = this;
            DontDestroyOnLoad(instance);

            instance.enabled = true;
        }
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
        enemies = FindObjectsOfType<EnemyBase>();
        playerHealthText = GameObject.Find("PlayerHealth").GetComponent<Text>();
        enemyHealthText = GameObject.Find("EnemyHealth").GetComponent<Text>();


        FindNewEnemy();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //reload
        }
        ////WIN/LOSE
        //if (currentState == State.PLAYER_TURN && player.unitBase.currentHealth <= 0)
        //{
        //    currentState = State.GAME_OVER;
        //}
        //if (currentState == State.ENEMY_TURN && currentEnemy.unitBase.currentHealth <= 0)
        //{
        //    foreach (ActivatableSkillBase skill in currentEnemy.unitBase.GetActivatables())
        //    {
        //        if (!player.unitBase.GetActivatables().Contains(skill))
        //        {
        //            player.unitBase.GetActivatables().Add(skill);

        //            foreach (ActivatableSkillBase skills in player.unitBase.GetActivatables())
        //            {
        //            }
        //            break;
        //        }
        //    }
        //    player.StealStats(currentEnemy.unitBase);
        //    FindNewEnemy();
        //    currentState = State.PLAYER_TURN;
        //}

        ////PLAYER COMBAT
        //if (Input.GetKeyDown(KeyCode.Return) && currentState == State.PLAYER_TURN)
        //{

        //}

        ////ENEMY COMBAT
        //else if (currentState == State.ENEMY_TURN)
        //{
        //    ActivatableSkillBase enemyUseSkill = currentEnemy.GetRandomSkill();
        //    UseSkill(enemyUseSkill, currentEnemy.unitBase, player.unitBase);
        //    currentState = State.END_OF_TURN;
        //}

        //END OF TURN
        if (currentState == State.END_OF_TURN)
        {
            currentEnemy.unitBase.EndOfTurnEffects();
            player.unitBase.EndOfTurnEffects();
            currentState = State.PLAYER_TURN;
            playerHealthText.text = "PlayerHealth: " + player.unitBase.currentHealth.ToString();
            enemyHealthText.text = "EnemyHealth: " + currentEnemy.unitBase.currentHealth.ToString();

            player.hasReadapted = false;
        }
    }

    public void PlayerUseSkill(ActivatableSkillBase skill)
    {
        UseSkill(skill, player.unitBase, currentEnemy.unitBase);
    }

    private void UseSkill(ActivatableSkillBase skill, UnitBase usingUnit, UnitBase opposingUnit)
    {
        if (skill.target == ActivatableSkillBase.Target.ENEMY)
        {
            if (skill.poisonDuration > 0)
            {
                opposingUnit.poisionBonusDuration = skill.poisonDuration;
                opposingUnit.poisonedDmg = skill.basePoison + skill.scalingPoison * usingUnit.attackPower;
            }
            if (skill.bleedDuration > 0)
            {
                opposingUnit.bleedingDuration = skill.bleedDuration;
                opposingUnit.bleedingDmg = skill.baseBleed + skill.scalingBleed * usingUnit.attackPower;
            }
            if (skill.baseDmg > 0)
            {
                opposingUnit.Defend(skill.baseDmg + skill.scalingDmg * usingUnit.attackPower, skill.dmgType, opposingUnit.agility, skill.skillName, usingUnit.name);
            }
}
        if (skill.target == ActivatableSkillBase.Target.SELF)
        {
            skill.UseSelfEffect(usingUnit);
        }
    }

    //MOSTLY FINE TO KEEP
    public void FindNewEnemy()
    {
        currentEnemy = enemies[Random.Range(0, enemies.Length)];
        currentEnemy.unitBase.currentHealth = currentEnemy.unitBase.maxHealth;
        worldText.text += '\n' + "You face a " + currentEnemy.name;
        enemyHealthText.text = "EnemyHealth: " + currentEnemy.unitBase.currentHealth.ToString();
    }

    public void CombatPrint(float dmg, bool hit, string name, string skillName = "")
    {
        if (!hit)
        {
            worldText.text += '\n' + name + " Missed";
        }
        else
        {
            worldText.text += '\n' + name + " " + skillName + " Dealt " + dmg + " dmg";
        }
    }

    public void HealPrint(float healAmount, string name)
    {
         worldText.text += '\n' + name + " Healed " + healAmount + " dmg";
    }

}
