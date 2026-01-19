using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPoint : MonoBehaviour
{
    public RectTransform RectTransform;
    [SerializeField] private GameObject _InRange;
    [SerializeField] private GameObject _OutRange;

    public void InRange()
    {
        if(!_InRange.activeSelf)
        {
            _InRange.SetActive(true);
            _OutRange.SetActive(false);
        }
    }

    public void OutRange()
    {
        if (!_OutRange.activeSelf)
        {
            _OutRange.SetActive(true);
            _InRange.SetActive(false);
        }
    }
}
