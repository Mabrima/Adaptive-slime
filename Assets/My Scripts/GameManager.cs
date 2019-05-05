using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum State { NULL, PLAYER_TURN, ENEMY_TURN, WORLD, GAME_OVER, END_OF_TURN, HANDLE_PLAYER_CHOICE }
public enum DmgTypes { NULL, SLASH, PIERCE, CRUSH, BLEED, POISON }

public class GameManager : MonoBehaviour
{

    Player player;
    public Text worldText;
    public Text playerHealthText;
    public Text enemyHealthText;
    public GameObject destinations;

    public static GameManager instance;
    public State currentState = State.NULL;
    public EnemyBase[] enemies;
    public EnemyBase currentEnemy;
    private int turn;
    private Area starterArea = Area.FORREST;
    public Area currentArea;


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
        destinations = GameObject.Find("Destinations");
        destinations.SetActive(false);

        campUsesLeft = 3;
        turn = 1;

        currentArea = starterArea;
        currentState = State.PLAYER_TURN;
        FindNewEnemy(starterArea);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);

            Invoke("GameStartup", .1f);
        }
        //WIN/LOSE
        if (currentState == State.ENEMY_TURN && currentEnemy.unitBase.currentHealth <= 0)
        {
            enemyHealthText.text = "EnemyHealth: 0";
            worldText.text += '\n' + "<color=yellow>Well done!</color> You have slain " + currentEnemy.unitBase.unitName;
            player.BankStats(currentEnemy.GetOriginalBase());
            AttemptStealSkill();
            turn = 1;
            destinations.SetActive(true);
            currentState = State.HANDLE_PLAYER_CHOICE;
        }
        if (currentState != State.GAME_OVER && player.unitBase.currentHealth <= 0)
        {
            player.TurnOffButtons();
            worldText.text += '\n' + "<color=red>Game over!</color> you have perished";
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

    int enemyRerollCap = 100;
    public void FindNewEnemy(Area area)
    {
        for (int i = 0; i < enemyRerollCap; i++)
        {
            currentEnemy = enemies[Random.Range(0, enemies.Length)];
            if (currentEnemy.unitBase.unitArea == area)
                break;
        }

        currentEnemy.ResetEnemy();
        worldText.text += '\n' + "You face a " + currentEnemy.unitBase.unitName;
        enemyHealthText.text = "EnemyHealth: " + currentEnemy.unitBase.currentHealth.ToString();
    }



    public void CombatPrint(float dmg, bool hit, string usingName, string defendingName, string skillName)
    {
        if (!hit)
        {
            worldText.text += '\n' + usingName + " Missed";
        }
        else
        {
            worldText.text += '\n' + usingName + " used " + skillName + " it <color=red>dealt</color> " + (int)dmg + " dmg to " + defendingName;
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

    public int campUsesLeft;
    public void GoToCamp()
    {
        player.ApplyBank();
        campUsesLeft--;
        worldText.text += '\n' + "You now only have <color=yellow>" + campUsesLeft + "</color> times left to camp before you face your ultimate <color=#f97522>foe</color>";
    }

    public void NextCombat()
    {
        FindNewEnemy(currentArea);
        currentState = State.PLAYER_TURN;

        
    }

}
