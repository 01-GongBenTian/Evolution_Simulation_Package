using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField] private Image _Bar;

    [SerializeField] private RectTransform.Axis _BarAxis;
    [SerializeField] private float _Maximum;

    public void ChangeColor(Color color)
    {
        _Bar.color = color;
    }

    public void UpdateBar(float progress)
    {
        _Bar.rectTransform.SetSizeWithCurrentAnchors(_BarAxis, _Maximum * progress);
    }
}
