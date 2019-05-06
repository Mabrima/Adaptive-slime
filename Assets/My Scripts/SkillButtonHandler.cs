using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillButtonHandler : ButtonHoverOver, IPointerClickHandler
{

    [HideInInspector]
    public PassiveSkillBase passiveHolder;
    [HideInInspector]
    public ActivatableSkillBase activeHolder;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.instance.currentState != State.PLAYER_TURN)
        {
            return;
        }

        if (activeHolder != null)
        {
            GameManager.instance.PlayerUseSkill(activeHolder);
        }
        else if (passiveHolder != null)
        {
            GameManager.instance.player.Readapting(passiveHolder);
        }

    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (passiveHolder != null)
        {
            descriptionText.text = passiveHolder.description;
        }
        else if (activeHolder != null)
        {
            descriptionText.text = activeHolder.description;
        }
    }
}
