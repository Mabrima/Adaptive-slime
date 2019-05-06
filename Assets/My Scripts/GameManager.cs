using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum State { NULL, PLAYER_TURN, ENEMY_TURN, WORLD, GAME_OVER, END_OF_TURN, HANDLE_PLAYER_CHOICE, GAME_WIN }
public enum DmgTypes { NULL, SLASH, PIERCE, CRUSH, BLEED, POISON }

public class GameManager : MonoBehaviour
{

    public Player player;
    public Text worldText;
    public Text playerHealthText;
    public Text enemyHealthText;
    public GameObject destinations;

    public static GameManager instance;
    public State currentState = State.NULL;
    public EnemyBase[] enemies;
    public EnemyBase currentEnemy;
    private int turn;
    private int enemiesSinceLastCamp = 0;
    private int enemiesBeforeForcedCamp = 10;

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
        NextCombat();
    }

    private void Update()
    {
        if (enemiesSinceLastCamp > enemiesBeforeForcedCamp)
        {
            worldText.text += "\n <color=magenta>Night falls and you are forced to make camp</color>";
            GoToCamp();
        }
        //WIN/LOSE
        if (currentState == State.ENEMY_TURN && currentEnemy.unitBase.currentHealth <= 0)
        {
            enemyHealthText.text = "Enemy health: 0";
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
            playerHealthText.text = "Player health: " + Mathf.CeilToInt(player.unitBase.currentHealth);
            enemyHealthText.text = "Enemy health: " + Mathf.CeilToInt(currentEnemy.unitBase.currentHealth);
            player.hasReadapted = false;

            player.unitBase.DecrementCooldowns();
            currentEnemy.unitBase.DecrementCooldowns();
            player.UpdateButtonCooldownText();
            turn++;

            currentState = State.PLAYER_TURN;
        }

        if (currentState == State.GAME_WIN)
        {
            worldText.text = "The Metamorph has been <color=red>vanquished!</color> \n <color=yellow>You Win!</color>";
            player.TurnOffButtons();
            currentState = State.GAME_OVER;
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

    public void Restart()
    {
        SceneManager.LoadScene(0);
        Invoke("GameStartup", .1f);
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
                worldText.text += "\n You stole <color=yellow>" + enemySkill.skillName + "</color> from " + currentEnemy.unitBase.unitName;
                ActivatableSkillBase tempSkill = Instantiate(enemySkill);
                tempSkill.cooldownTimer = 0;
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
                worldText.text += "\n You stole <color=yellow>" + enemySkill.skillName + "</color> from " + currentEnemy.unitBase.unitName;
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
        enemyHealthText.text = "Enemy health: " + currentEnemy.unitBase.currentHealth.ToString();
        enemiesSinceLastCamp++;
    }



    public void CombatPrint(float dmg, bool hit, string usingName, string defendingName, string skillName)
    {
        if (!hit)
        {
            worldText.text += '\n' + usingName + " Missed";
        }
        else
        {
            worldText.text += '\n' + "The " + usingName + " used " + skillName + " it <color=red>dealt</color> " + (int)dmg + " dmg to " + defendingName;
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
        player.unitBase.ResetCooldowns();
        player.UpdateButtonCooldownText();
        player.AddEmptyActiveEquipButton();
        player.unitBase.currentHealth = player.unitBase.maxHealth;
        enemiesSinceLastCamp = 0;
        campUsesLeft--;
        if (campUsesLeft != 0)
        {
            worldText.text += '\n' + "You now only have <color=yellow>" + campUsesLeft + "</color> times left to camp before you face your ultimate <color=#f97522>foe</color>";
            NextCombat();
        }
        else
        {
            currentEnemy.GenerateMetaMorph();
            worldText.text += '\n' + "You face a " + currentEnemy.unitBase.unitName + "\n <color=red>better start praying</color>";
            enemyHealthText.text = "Enemy health: " + currentEnemy.unitBase.currentHealth.ToString();
            currentState = State.PLAYER_TURN;
        }

    }

    public void NextCombat()
    {
        FindNewEnemy(currentArea);
        currentState = State.PLAYER_TURN;
    }


}
