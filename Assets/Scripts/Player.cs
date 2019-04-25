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

    public bool hasReadapted = false;

    private void Start()
    {
        unitBase = Instantiate(unitBase);

        SetPassives();

        InstantiateInitialButtons();
    }

    private void InstantiateInitialButtons()
    {
        for (int i = 0; i < unitBase.GetActivatables().Count; i++)
        {
            Button tempButton = Instantiate(buttonBase, new Vector3(0, 90 - 35*i, 0), Quaternion.identity, activateablesButtons.transform);
            tempButton.transform.localPosition = new Vector3(0, 90 - 35 * i, 0);
            tempButton.GetComponentInChildren<Text>().text = unitBase.GetActivatables()[i].skillName;
            tempButton.GetComponent<ButtonHoverOver>().activeHolder = unitBase.GetActivatables()[i];
        }
        ReMakeEquipableButtons();
    }

    private void ReMakeEquipableButtons()
    {
        for (int i = 0; i < inactiveEquipablesButtons.transform.childCount; i++)
        {
            Destroy(inactiveEquipablesButtons.transform.GetChild(0).gameObject);
        }

        for (int i = 0; i < activeEquipablesButtons.transform.childCount; i++)
        {
            Destroy(activeEquipablesButtons.transform.GetChild(0).gameObject);
        }

        for (int i = 0; i < inactiveEquipables.Count; i++)
        {
            Button tempButton = Instantiate(buttonBase, inactiveEquipablesButtons.transform);
            tempButton.transform.localPosition = new Vector3(0, 90 - 35 * i, 0);
            tempButton.GetComponentInChildren<Text>().text = inactiveEquipables[i].skillName;
            tempButton.GetComponent<ButtonHoverOver>().passiveHolder = inactiveEquipables[i];
        }

        for (int i = 0; i < activeEquipables.Count; i++)
        {
            Button tempButton = Instantiate(buttonBase, new Vector3(0, 90 - 35 * i, 0), Quaternion.identity, activeEquipablesButtons.transform);
            tempButton.transform.localPosition = new Vector3(0, 90 - 35 * i, 0);
            tempButton.GetComponentInChildren<Text>().text = activeEquipables[i].skillName;
            tempButton.GetComponent<ButtonHoverOver>().passiveHolder = activeEquipables[i];
        }
    }

    void SetPassives()
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

    public void StealStats(UnitBase unit)
    {
        unitBase.maxHealth += unit.maxHealth * 0.1f;
        unitBase.attackPower += unit.attackPower * 0.1f;
        unitBase.agility += unit.agility * 0.1f;
        unitBase.defence += unit.defence * 0.1f;
        unitBase.Heal(unit.maxHealth * 0.2f, "end of battle heal");
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
            activeEquipables.Remove(equipedSkillToSwap);
            activeEquipables.Add(unequipedSkillToSwap);
            inactiveEquipables.Remove(unequipedSkillToSwap);
            inactiveEquipables.Add(equipedSkillToSwap);

            equipedSkillToSwap.RemoveBonus(unitBase);
            unequipedSkillToSwap.GiveBonus(unitBase);

            GameManager.instance.worldText.text += '\n' + "Swapped " + equipedSkillToSwap + " with " + unequipedSkillToSwap;

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

}
