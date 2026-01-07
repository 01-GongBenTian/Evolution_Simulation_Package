using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    public static SystemManager INSTANCE;
    public static int COUNT = 0;

    [Range(0.02f, 10)]
    public float TimerCount;
    public float Timer;

    private void Start()
    {
        if(!INSTANCE)
        {
            INSTANCE = this;
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
            //Debug.Log("New Turn");
            //COUNT = 0;
            CreatureManager.INSTANCE.OnNewTurn();
            //Debug.Log(COUNT);

            Timer = 0;
        }
    }
}
