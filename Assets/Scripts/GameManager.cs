using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Range(0, 10)]
    public float TimerCount;
    public float Timer;

    private void Start()
    {
        if(!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Timer = 0;
    }

    private void FixedUpdate()
    {
        Timer += Time.fixedDeltaTime;
        if(Timer >= TimerCount)
        {
            Debug.Log("New Turn");
            CreatureManager.INSTANCE.OnNewTurn();

            Timer = 0;
        }
    }
}
