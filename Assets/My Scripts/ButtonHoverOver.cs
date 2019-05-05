using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Player player;
    public Text descriptionText;

    private readonly string defaultDescription = "Hover over a skill to see its description.";

    /// <summary>
    /// Script to handle buttons, OnEnter, and OnExit properties.
    /// </summary>
    /// <param name="eventData"></param>


    public virtual void OnPointerEnter(PointerEventData eventData)
    {


    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionText.text = defaultDescription;
    }
}
