using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum DmgTypes { NULL, SLASH, PIERCE, CRUSH, BLEED, POISON }
    public enum State { NULL, PLAYER_TURN, ENEMY_TURN, WORLD, DEALING_WITH_READAPT, GAME_OVER, END_OF_TURN }

    Player player;
    public Text descriptionText;
    public Text worldText;
    public Text playerHealthText;
    public Text enemyHealthText;

    public static GameManager instance;
    public State currentState = State.NULL;
    public EnemyBase[] enemies;
    public EnemyBase currentEnemy;
    private int turn = 1;

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
        GameStartup();
    }

    void GameStartup()
    {
        player = FindObjectOfType<Player>();
        enemies = FindObjectsOfType<EnemyBase>(); 
        playerHealthText = GameObject.Find("PlayerHealth").GetComponent<Text>();
        enemyHealthText = GameObject.Find("EnemyHealth").GetComponent<Text>();
        worldText = GameObject.Find("WorldText").GetComponent<Text>();
        descriptionText = GameObject.Find("DescriptionText").GetComponent<Text>();

        currentState = State.PLAYER_TURN;
        FindNewEnemy();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);

            Invoke("GameStartup", .1f);
        }
        //WIN/LOSE
        if (currentState != State.GAME_OVER && currentEnemy.unitBase.currentHealth <= 0)
        {
            currentEnemy.ResetEnemy();
            player.StealStats(currentEnemy.unitBase);
            AttemptStealSkill();
            FindNewEnemy();
            turn = 1;
            currentState = State.END_OF_TURN;
        }
        if (currentState != State.GAME_OVER && player.unitBase.currentHealth <= 0)
        {
            player.TurnOffButtons();
            worldText.text += '\n' + "Game over, you have perished";
            currentState = State.GAME_OVER;
        }
        

        ////PLAYER COMBAT
        //wait for player input to trigger PlayerUseSkill()

        ////ENEMY COMBAT
        else if (currentState == State.ENEMY_TURN)
        {
            ActivatableSkillBase enemyUseSkill = currentEnemy.GetRandomSkill();
            UseSkill(enemyUseSkill, currentEnemy.unitBase, player.unitBase);
            currentState = State.END_OF_TURN;
        }

        //END OF TURN
        if (currentState == State.END_OF_TURN)
        {
            worldText.text += '\n' + "End of turn " + turn; 
            currentEnemy.unitBase.EndOfTurnEffects();
            player.unitBase.EndOfTurnEffects();
            playerHealthText.text = "PlayerHealth: " + Mathf.CeilToInt(player.unitBase.currentHealth);
            enemyHealthText.text = "EnemyHealth: " + Mathf.CeilToInt(currentEnemy.unitBase.currentHealth);
            player.hasReadapted = false;

            player.unitBase.DecrementCooldowns();
            currentEnemy.unitBase.DecrementCooldowns();
            player.UpdateButtonCooldownText();
            turn++;

            currentState = State.PLAYER_TURN;
        }
    }

    private void AttemptStealSkill()
    {
        if (Random.Range(0, 2) > 1)
        {
            if (!AttemptStealActive())
                AttemptStealEquip();
        }
        else
        {
            if (!AttemptStealEquip())
                AttemptStealActive();
        }
    }

    private bool AttemptStealActive()
    {
        bool notKnownSkill = true;
        foreach (ActivatableSkillBase enemySkill in currentEnemy.unitBase.GetActivatables())
        {
            foreach (ActivatableSkillBase playerSkill in player.unitBase.GetActivatables())
            {
                if (enemySkill.skillName == playerSkill.skillName)
                {
                    notKnownSkill = false;
                    break;
                }
            }
            if (notKnownSkill && Random.Range(0, 2) > 0)
            {
                ActivatableSkillBase tempSkill = Instantiate(enemySkill);
                player.unitBase.AddSkillToActivatableSkills(tempSkill);
                
                player.AddActivatableButton(tempSkill);
                return true;
            }
        }
        return false;
    }

    private bool AttemptStealEquip()
    {
        bool notKnownSkill = true;
        foreach (PassiveSkillBase enemySkill in currentEnemy.unitBase.GetPassives())
        {
            Debug.Log("Attempting to steal passive " + enemySkill.skillName);
            foreach (PassiveSkillBase playerSkill in player.unitBase.GetPassives())
            {
                if (playerSkill.skillName == enemySkill.skillName)
                {
                    notKnownSkill = false;
                    break;
                }
            }
            if (notKnownSkill && Random.Range(0, 2) > 0)
            {
                Debug.Log("Attempting to add skill");
                player.unitBase.GetPassives().Add(enemySkill);
                player.inactiveEquipables.Add(enemySkill);
                player.AddInactiveEquipButton(enemySkill);
                return true;
            }
        }
        return false;
    }

    public void PlayerUseSkill(ActivatableSkillBase skill)
    {
        if (skill.cooldownTimer > 0)
        {
            worldText.text += '\n' + "That skill is still on cooldown";
            return;
        }
        worldText.text = "";
        UseSkill(skill, player.unitBase, currentEnemy.unitBase);
        currentState = State.ENEMY_TURN;
    }

    private void UseSkill(ActivatableSkillBase skill, UnitBase usingUnit, UnitBase opposingUnit)
    {
        if (skill.target == ActivatableSkillBase.Target.ENEMY)
        {
            if (skill.poisonDuration > 0)
            {
                opposingUnit.poisonedDuration = skill.poisonDuration;
                opposingUnit.poisonedDmg = skill.basePoison + skill.scalingPoison * usingUnit.abilityPower;
            }
            if (skill.bleedDuration > 0)
            {
                opposingUnit.bleedingDuration = skill.bleedDuration;
                opposingUnit.bleedingDmg = skill.baseBleed + skill.scalingBleed * usingUnit.abilityPower;
            }
            if (skill.baseDmg > 0)
            {
                opposingUnit.Defend(skill.baseDmg + skill.scalingDmg * usingUnit.abilityPower, skill.dmgType, skill.skillName, usingUnit.unitName);
            }
}
        if (skill.target == ActivatableSkillBase.Target.SELF)
        {
            skill.UseSelfEffect(usingUnit);
        }

        usingUnit.SetSkillOnCooldown(skill);
    }

    //MOSTLY FINE TO KEEP
    public void FindNewEnemy()
    {
        currentEnemy = enemies[Random.Range(0, enemies.Length)];
        worldText.text += '\n' + "You face a " + currentEnemy.unitBase.unitName;
        enemyHealthText.text = "EnemyHealth: " + currentEnemy.unitBase.currentHealth.ToString();
    }

    public void CombatPrint(float dmg, bool hit, string name, string skillName)
    {
        if (!hit)
        {
            worldText.text += '\n' + name + " Missed";
        }
        else
        {
            worldText.text += '\n' + name + " used " + skillName + " it <color=red>dealt</color> " + (int)dmg + " dmg";
        }
    }

    public void HealPrint(float healAmount, string name, string skillName)
    {
         worldText.text += '\n' + name + " <color=green>Healed</color> " + (int)healAmount + " from " + skillName;
    }

    public void CorruptedBloodPrint(float dmg, string name)
    {
        worldText.text += '\n' + name + " Poison mixes with bleeding causing a <color=purple>corrupted blood</color> hemorrhage dealing " + (int)dmg + " dmg";
    }

    public void BleedPrint(float dmg, string name)
    {
        worldText.text += '\n' + name + " <color=red>bleed for</color> " + (int)dmg + " dmg";
    }

    public void PoisonPrint(float dmg, string name)
    {
        worldText.text += '\n' + name + " suffers " + (int)dmg + " dmg from the <color=#56ff23>poison</color>";
    }



}
