using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RestartButton : ButtonHoverOver
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        descriptionText.text = "Restart the game. Takes you right back to the beginning.";
    }

    public void Restart()
    {
        GameManager.instance.Restart();
    }
}
