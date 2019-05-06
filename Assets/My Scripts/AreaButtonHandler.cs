using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Area
{
    MOUNTAIN, DESERT, FORREST, LAKE
}

/// <summary>
/// This script has no real reason to be one script. Should probably have been split into 3 scripts. One for continue, one for camp and one for areas. 
/// </summary>
public class AreaButtonHandler : ButtonHoverOver, IPointerClickHandler
{
    public bool isCamp;
    public bool isContinue;

    public Area area;

    void OnEnable()
    {
        if (!isCamp && !isContinue)
        {
            area = (Area)Random.Range(0, 4);
            gameObject.GetComponentInChildren<Text>().text = area.ToString();
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (isCamp)
        {
            GameManager.instance.destinations.SetActive(false);
            GameManager.instance.GoToCamp();
            return;
        }
        if (!isContinue)
        {
            GameManager.instance.currentArea = area;
        }
        

        GameManager.instance.destinations.SetActive(false);
        GameManager.instance.NextCombat();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (isCamp)
        {
            CampDescription();
            return;
        }

        if (isContinue)
        {
            descriptionText.text = "Stay in the " + GameManager.instance.currentArea;
            //ContinueDescription();
            return;
        }

        switch (area)
        {
            case Area.MOUNTAIN:
                descriptionText.text = "Mountain, home of some 'intelligent' races";
                break;
            case Area.DESERT:
                descriptionText.text = "Desert, home of nasty venomous critters";
                break;
            case Area.FORREST:
                descriptionText.text = "Forrest, mostly housing weak creatures. Though occasionally others pass by.";
                break;
            case Area.LAKE:
                descriptionText.text = "Lake, creatures living here have developed increadible defences.";
                break;
            default:
                break;
        }
    }

    private void CampDescription()
    {
        descriptionText.text = "Go back to camp to rest up. (This will give you all the stats you banked from devouring enemies) You can camp " 
            + GameManager.instance.campUsesLeft + " more times.";
    }

    private void MountainDescription()
    {
        descriptionText.text = "An inhospitable place where only the best survive, all creatures here .";
    }
}