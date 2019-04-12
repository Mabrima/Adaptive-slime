using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    Player player;
    public static GameManager instance;

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
