using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuitButton : ButtonHoverOver
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        descriptionText.text = "Quit game, game saves nothing so don't missclick this.";
    }

    public void Quit()
    {
        Application.Quit();
    }
}
