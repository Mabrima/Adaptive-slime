using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public UnitBase unitBase;

    public Button buttonBase;
    public GameObject inactiveEquipablesButtons, activeEquipablesButtons, activateablesButtons;

    public List<PassiveSkillBase> activeEquipables;
    public List<PassiveSkillBase> inactiveEquipables;

    private PassiveSkillBase unequipedSkillToSwap;
    private PassiveSkillBase equipedSkillToSwap;

    [Header("EmptySkill")]
    public PassiveSkillBase emptySkill;

    public bool hasReadapted = false;

    private float maxHealthBank = 0;
    private float abilityPowerBank = 0;
    private float defenceBank = 0;

    private void Start()
    {
        unitBase = Instantiate(unitBase);
        unitBase.Initialize();

        SetInitialPassives();

        InstantiateInitialButtons();
    }

    private void InstantiateInitialButtons()
    {
        for (int i = 0; i < unitBase.GetActivatables().Count; i++)
        {
            Button tempButton = Instantiate(buttonBase, activateablesButtons.transform);
            tempButton.transform.localPosition = new Vector3(0, 180 - 35 * i, 0);
            tempButton.GetComponentInChildren<Text>().text = unitBase.GetActivatables()[i].skillName;
            tempButton.GetComponent<SkillButtonHandler>().activeHolder = unitBase.GetActivatables()[i];
        }
        ReMakeEquipableButtons();
    }

    private void ReMakeEquipableButtons()
    {
        int size = inactiveEquipablesButtons.transform.childCount;
        for (int i = 0; i < size; i++)
        {
            Destroy(inactiveEquipablesButtons.transform.GetChild(i).gameObject);
        }

        size = activeEquipablesButtons.transform.childCount;
        for (int i = 0; i < size; i++)
        {
            Destroy(activeEquipablesButtons.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < inactiveEquipables.Count; i++)
        {
            Button tempButton = Instantiate(buttonBase, inactiveEquipablesButtons.transform);
            tempButton.transform.localPosition = new Vector3(0, 80 - 35 * i, 0);
            tempButton.GetComponentInChildren<Text>().text = inactiveEquipables[i].skillName;
            tempButton.GetComponent<SkillButtonHandler>().passiveHolder = inactiveEquipables[i];
        }

        for (int i = 0; i < activeEquipables.Count; i++)
        {
            Button tempButton = Instantiate(buttonBase, activeEquipablesButtons.transform);
            tempButton.transform.localPosition = new Vector3(0, 80 - 35 * i, 0);
            tempButton.GetComponentInChildren<Text>().text = activeEquipables[i].skillName;
            tempButton.GetComponent<SkillButtonHandler>().passiveHolder = activeEquipables[i];
        }
    }

    public void AddActivatableButton(ActivatableSkillBase skill)
    {
        Button tempButton = Instantiate(buttonBase, activateablesButtons.transform);
        tempButton.transform.localPosition = new Vector3(0, 180 - 35 * (activateablesButtons.transform.childCount-1), 0);
        tempButton.GetComponentInChildren<Text>().text = skill.skillName;
        tempButton.GetComponent<SkillButtonHandler>().activeHolder = skill;
    }

    public void AddActiveEquipButton(PassiveSkillBase skill)
    {
        Button tempButton = Instantiate(buttonBase, activeEquipablesButtons.transform);
        tempButton.transform.localPosition = new Vector3(0, 80 - 35 * (activeEquipablesButtons.transform.childCount-1), 0);
        tempButton.GetComponentInChildren<Text>().text = skill.skillName;
        tempButton.GetComponent<SkillButtonHandler>().passiveHolder = skill;
    }

    public void AddEmptyActiveEquipButton()
    {
        activeEquipables.Add(emptySkill);
        AddActiveEquipButton(emptySkill);
    }

    public void AddInactiveEquipButton(PassiveSkillBase skill)
    {
        Button tempButton = Instantiate(buttonBase, inactiveEquipablesButtons.transform);
        tempButton.transform.localPosition = new Vector3(0, 80 - 35 * (inactiveEquipablesButtons.transform.childCount-1), 0);
        tempButton.GetComponentInChildren<Text>().text = skill.skillName;
        tempButton.GetComponent<SkillButtonHandler>().passiveHolder = skill;
    }

    void SetInitialPassives()
    {
        foreach (PassiveSkillBase activePassive in activeEquipables)
        {
            unitBase.GetPassives().Add(activePassive);
            activePassive.GiveBonus(unitBase);
        }

        foreach (PassiveSkillBase inactivePassive in inactiveEquipables)
        {
            unitBase.GetPassives().Add(inactivePassive);
        }
    }

    public ActivatableSkillBase GetActivatableSkillByName(string name)
    {
        foreach (ActivatableSkillBase skill in unitBase.GetActivatables())
        {
            if (name.ToLower() == skill.skillName.ToLower())
            {
                return skill;
            }
        }
        return null;
    }

    public PassiveSkillBase GetPassiveSkillByName(string name)
    {
        foreach (PassiveSkillBase skill in unitBase.GetPassives())
        {
            if (name.ToLower() == skill.skillName.ToLower())
            {
                return skill;
            }
        }
        return null;
    }

    public PassiveSkillBase GetActivePassiveSkillByName(string name)
    {
        foreach (PassiveSkillBase skill in activeEquipables)
        {
            if (name.ToLower() == skill.skillName.ToLower())
            {
                return skill;
            }
        }
        return null;
    }

    public PassiveSkillBase GetInactivePassiveSkillByName(string name)
    {
        foreach (PassiveSkillBase skill in inactiveEquipables)
        {
            if (name.ToLower() == skill.skillName.ToLower())
            {
                return skill;
            }
        }
        return null;
    }

    public void BankStats(UnitBase unit)
    {
        maxHealthBank += unit.maxHealth * 0.1f;
        abilityPowerBank += unit.abilityPower * 0.1f;
        defenceBank += unit.defence * 0.1f;

        unitBase.Heal(unit.maxHealth * 0.2f, "end of battle heal");
    }

    public void ApplyBank()
    {
        float tempFullmaxHealth = unitBase.maxHealth + maxHealthBank;
        float tempFullAbilityPower = unitBase.abilityPower + abilityPowerBank;
        float tempFullDefence = unitBase.defence + defenceBank;
        GameManager.instance.worldText.text += '\n' + "While resting you have had time to fully digest your foes, your stats have grown from \n <color=red>" + unitBase.abilityPower
            + "</color> to <color=red>" + (int)tempFullAbilityPower + " power</color>, <color=green>" + unitBase.maxHealth + "</color> to <color=green>" 
            + (int)tempFullmaxHealth + " health</color> and <color=cyan>" + unitBase.defence + "</color> to <color=cyan>" + (int)tempFullDefence + " defence</color>";

        unitBase.maxHealth = tempFullmaxHealth;
        unitBase.abilityPower = tempFullAbilityPower;
        unitBase.defence = tempFullDefence;

        abilityPowerBank = 0;
        maxHealthBank = 0;
        defenceBank = 0;
    }

    public void Readapting(PassiveSkillBase skillToSwap)
    {
        if (hasReadapted)
        {
            GameManager.instance.worldText.text += '\n' + "You have already readapted this turn";
            return;
        }

        if (inactiveEquipables.Contains(skillToSwap))
        {
            unequipedSkillToSwap = skillToSwap;
            GameManager.instance.worldText.text += '\n' + "Currently unequiped " + skillToSwap.skillName + " selected to swap";
        }
        else if (activeEquipables.Contains(skillToSwap))
        {
            equipedSkillToSwap = skillToSwap;
            GameManager.instance.worldText.text += '\n' + "Currently equiped " + skillToSwap.skillName + " selected to swap";
        }

        if (unequipedSkillToSwap != null && equipedSkillToSwap != null)
        {
            if (equipedSkillToSwap.skillName != "Empty Equip Slot")
            {
                inactiveEquipables.Add(equipedSkillToSwap);
                equipedSkillToSwap.RemoveBonus(unitBase);
            }
            activeEquipables.Remove(equipedSkillToSwap);
            activeEquipables.Add(unequipedSkillToSwap);
            inactiveEquipables.Remove(unequipedSkillToSwap);

            unequipedSkillToSwap.GiveBonus(unitBase);

            GameManager.instance.worldText.text += '\n' + "Swapped " + equipedSkillToSwap.skillName + " with " + unequipedSkillToSwap.skillName;

            ReMakeEquipableButtons();

            equipedSkillToSwap = null;
            unequipedSkillToSwap = null;
            hasReadapted = true;
        }
    }


    public void TurnOffButtons()
    {
        for (int i = 0; i < activateablesButtons.transform.childCount; i++)
        {
            activateablesButtons.transform.GetChild(i).GetComponent<Button>().interactable = false;
        }

        for (int i = 0; i < inactiveEquipablesButtons.transform.childCount; i++)
        {
            inactiveEquipablesButtons.transform.GetChild(i).GetComponent<Button>().interactable = false;
        }

        for (int i = 0; i < activeEquipablesButtons.transform.childCount; i++)
        {
            activeEquipablesButtons.transform.GetChild(i).GetComponent<Button>().interactable = false;
        }
    }

    public void UpdateButtonCooldownText()
    {
        List<ActivatableSkillBase> tempSkills = unitBase.GetActivatables();
        for (int i = 0; i < unitBase.GetActivatables().Count; i++)
        {
            if (tempSkills[i].cooldownTimer > 0)
                activateablesButtons.transform.GetChild(i).GetComponentInChildren<Text>().text = tempSkills[i].skillName + " (" + tempSkills[i].cooldownTimer + ")";
            else
                activateablesButtons.transform.GetChild(i).GetComponentInChildren<Text>().text = tempSkills[i].skillName;
        }
    }
}
