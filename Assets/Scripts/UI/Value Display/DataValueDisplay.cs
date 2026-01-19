using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class DataValueDisplay : MonoBehaviour
{
    [SerializeField] protected Image _Icon;
    [SerializeField] protected TextMeshProUGUI _Label;

    public void UpdateIcon(Sprite sprite)
    {
        _Icon.sprite = sprite;
    }

    public void UpdateLabel(string label)
    {
        _Label.text = label;
    }

    public abstract void UpdateValue(object value);
}
