using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CreatureGroup;

public class CreatureStatusIcon : MonoBehaviour
{
    [SerializeField] private RectTransform _Canvas;
    [SerializeField] private GameObject _Hot;
    [SerializeField] private GameObject _Cold;
    [SerializeField] private GameObject _Dry;
    [SerializeField] private GameObject _Hungry;

    public void Start()
    {
        _Canvas.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public void UpdateStatusIcon(Status status)
    {
        int count = 0;

        //Hot
        if ((status & Status.HOT) == Status.HOT)
        {
            _Hot.SetActive(true);
            ++count;
        }
        else
        {
            _Hot.SetActive(false);
        }

        //Cold
        if ((status & Status.COLD) == Status.COLD)
        {
            _Cold.SetActive(true);
            ++count;
        }
        else
        {
            _Cold.SetActive(false);
        }

        //Dry
        if ((status & Status.DRY) == Status.DRY)
        {
            _Dry.SetActive(true);
            ++count;
        }
        else
        {
            _Dry.SetActive(false);
        }

        //Hungry
        if ((status & Status.HUNGRY) == Status.HUNGRY)
        {
            _Hungry.SetActive(true);
            ++count;
        }
        else
        {
            _Hungry.SetActive(false);
        }


        //adjusted the canvas size according to the number of status
        _Canvas.sizeDelta = new Vector2(32.0f * count + (10.0f * (count - 1)), 32.0f);
    }
}
