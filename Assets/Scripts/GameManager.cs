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

    private PassiveSkillBase activeSkillToSwap;
    private PassiveSkillBase inactiveSkillToSwap;

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

        activatableAbilitiesText = GameObject.Find("ActivatableAbilitiesText").GetComponent<Text>();
        activatableAbilitiesText.text = "Activatable Abilites \n \n" + "Readapt \n";
        foreach (ActivatableSkillBase skill in player.unitBase.GetActivatables())
        {
            activatableAbilitiesText.text += skill.skillName + '\n';
        }

        activePassivesText = GameObject.Find("ActivePassivesText").GetComponent<Text>();
        activePassivesText.text = "Active Passives \n \n";
        foreach (PassiveSkillBase skill in player.activePassives)
        {
            activePassivesText.text += skill.skillName + '\n';
        }

        inactivePassivesText = GameObject.Find("InactivePassivesText").GetComponent<Text>();
        inactivePassivesText.text = "Inactive Passives \n \n";
        foreach (PassiveSkillBase skill in player.inactivePassives)
        {
            inactivePassivesText.text += skill.skillName + '\n';
        }

        FindNewEnemy();
    }

    private void Update()
    {
        //WIN/LOSE
        if (currentState == State.PLAYER_TURN && player.unitBase.currentHealth < 0)
        {
            worldText.text += '\n' + "You lose";
            currentState = State.GAME_OVER;
        }
        if (currentState == State.ENEMY_TURN && currentEnemy.unitBase.currentHealth < 0)
        {
            worldText.text += '\n' + "You have defeated a " + currentEnemy.name;
            foreach (ActivatableSkillBase skill in currentEnemy.unitBase.GetActivatables())
            {
                if (!player.unitBase.GetActivatables().Contains(skill))
                {
                    player.unitBase.GetActivatables().Add(skill);

                    activatableAbilitiesText.text = "Activatable Abilites \n \n" + "Readapt \n";
                    foreach (ActivatableSkillBase skills in player.unitBase.GetActivatables())
                    {
                        activatableAbilitiesText.text += skills.skillName + '\n';
                    }
                    break;
                }
            }
            player.StealStats(currentEnemy.unitBase);
            FindNewEnemy();
            currentState = State.PLAYER_TURN;
        }

        //READAPT
        if (Input.GetKeyDown(KeyCode.Return) && currentState == State.DEALING_WITH_READAPT)
        {
            if (inactiveSkillToSwap == null)
            {
                if (inputText.text.ToLower() == "cancel")
                { 
                    currentState = State.PLAYER_TURN;
                }
                inactiveSkillToSwap = player.GetInactivePassiveSkillByName(inputText.text);
                if (inactiveSkillToSwap != null)
                {
                    worldText.text += '\n' + "Please type the name of an active skill to swap, or type cancel to cancel readapt";
                }
            }
            else if (activeSkillToSwap == null)
            {
                if (inputText.text.ToLower() == "cancel")
                {
                    currentState = State.PLAYER_TURN;
                    inactiveSkillToSwap = null;
                }
                activeSkillToSwap = player.GetActivePassiveSkillByName(inputText.text);
                if (activeSkillToSwap != null)
                {
                    player.activePassives.Remove(activeSkillToSwap);
                    player.activePassives.Add(inactiveSkillToSwap);
                    player.inactivePassives.Remove(inactiveSkillToSwap);
                    player.inactivePassives.Add(activeSkillToSwap);

                    activeSkillToSwap.RemoveBonus(player.unitBase);
                    inactiveSkillToSwap.GiveBonus(player.unitBase);

                    worldText.text += '\n' + "Swapped " + activeSkillToSwap + " with " + inactiveSkillToSwap;

                    activePassivesText.text = "Active Passives \n \n";
                    foreach (PassiveSkillBase skill in player.activePassives)
                    {
                        activePassivesText.text += skill.skillName + '\n';
                    }

                    inactivePassivesText.text = "Inactive Passives \n \n";
                    foreach (PassiveSkillBase skill in player.inactivePassives)
                    {
                        inactivePassivesText.text += skill.skillName + '\n';
                    }

                    activeSkillToSwap = null;
                    inactiveSkillToSwap = null;
                    currentState = State.PLAYER_TURN;
                }
            }
        }

        //PLAYER COMBAT
        if (Input.GetKeyDown(KeyCode.Return) && currentState == State.PLAYER_TURN)
        {
            string[] tempString = inputText.text.Split(' ');
            if (inputText.text.Split(' ').Length > 1)
            {
                if (tempString[0].ToLower() == "use")
                {
                    ActivatableSkillBase activatableSkill = player.GetActivatableSkillByName(tempString[1]);
                    if (activatableSkill != null)
                    {
                        UseSkill(activatableSkill, player.unitBase, currentEnemy.unitBase);
                        currentState = State.ENEMY_TURN;
                    }
                    else if (tempString[1].ToLower() == "readapt" && player.unitBase.healingDuration == 0)
                    {
                        worldText.text += '\n' + "Please type the name of an inactive skill to swap, or type cancel to cancel readapt";
                        currentState = State.DEALING_WITH_READAPT;
                    }
                }
            }

            else 
            {
                if (tempString[0].ToLower() == "readapt")
                {
                    descriptionText.text = "Choose one active passive to swap with an inactive passive";
                }
                ActivatableSkillBase activatableSkill = player.GetActivatableSkillByName(tempString[0]);
                if (activatableSkill != null)
                {
                    descriptionText.text = activatableSkill.description;
                    return;
                }
                PassiveSkillBase passiveSkill = player.GetPassiveSkillByName(tempString[0]);
                if (passiveSkill != null)
                {
                    descriptionText.text = passiveSkill.description;
                    return;
                }
            }
            
        }

        //ENEMY COMBAT
        else if (currentState == State.ENEMY_TURN)
        {
            ActivatableSkillBase enemyUseSkill = currentEnemy.GetRandomSkill();
            UseSkill(enemyUseSkill, currentEnemy.unitBase, player.unitBase);
            currentState = State.END_OF_TURN;
        }

        if (currentState == State.END_OF_TURN)
        {
            currentEnemy.unitBase.EndOfTurnEffects();
            player.unitBase.EndOfTurnEffects();
            currentState = State.PLAYER_TURN;
            playerHealthText.text = "PlayerHealth: " + player.unitBase.currentHealth.ToString();
            enemyHealthText.text = "EnemyHealth: " + currentEnemy.unitBase.currentHealth.ToString();
        }
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
                opposingUnit.Defend(skill.baseDmg + skill.scalingDmg * usingUnit.attackPower, skill.dmgType, opposingUnit.agility, usingUnit.name);
            }
}
        if (skill.target == ActivatableSkillBase.Target.SELF)
        {
            skill.UseSelfEffect(usingUnit);
        }
    }

    public void FindNewEnemy()
    {
        currentEnemy = enemies[Random.Range(0, enemies.Length)];
        currentEnemy.unitBase.currentHealth = currentEnemy.unitBase.maxHealth;
        worldText.text += '\n' + "You face a " + currentEnemy.name;
        enemyHealthText.text = "EnemyHealth: " + currentEnemy.unitBase.currentHealth.ToString();
    }

    public void CombatPrint(float dmg, bool hit, string name)
    {
        if (!hit)
        {
            worldText.text += '\n' + name + " Missed";
        }
        else
        {
            worldText.text += '\n' + name + " Dealt " + dmg + " dmg";
        }
    }

    public void HealPrint(float healAmount, string name)
    {
         worldText.text += '\n' + name + " Healed " + healAmount + " dmg";
    }

}
