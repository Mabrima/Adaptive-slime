using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    Player player;
    public Text rollingText;
    public static GameManager instance;
    public State currentState = State.NULL;

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

    public enum DmgTypes { NULL, SLASH, PIERCE, CRUSH }
    public enum State { NULL, COMBAT, WORLD }

    void Start()
    {
        player = FindObjectOfType<Player>();
        rollingText = GameObject.Find("World Text").GetComponent<Text>();
     }

    void FindEnemy()
    {

    }

    private void CombatUpdate()
    {
        //wait for player input
        //enemy do combat
        //end of round
        //repeat
    }
}
