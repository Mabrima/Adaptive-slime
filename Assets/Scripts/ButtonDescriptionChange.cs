using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDescriptionChange : MonoBehaviour
{
    Text text;
    public string textDescription;

    void Start()
    {
        text = GameObject.Find("DescriptionText").GetComponent<Text>();
    }

    public void ChangeText()
    {
        text.text = textDescription;
    }
}
