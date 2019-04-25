using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Player player;
    public Text descriptionText;

    [HideInInspector]
    public PassiveSkillBase passiveHolder;
    [HideInInspector]
    public ActivatableSkillBase activeHolder;

    private readonly string defaultDescription = "Hover over a skill to see its description.";

    /// <summary>
    /// Script to handle buttons, OnEnter, OnClick and OnExit properties.
    /// </summary>
    /// <param name="eventData"></param>

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.instance.currentState != GameManager.State.PLAYER_TURN)
        {
            return;
        }

        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        if (activeHolder != null)
        {
            GameManager.instance.PlayerUseSkill(activeHolder);
        }
        else if (passiveHolder != null)
        {
            player.Readapting(passiveHolder);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
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

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionText.text = defaultDescription;
    }
}
