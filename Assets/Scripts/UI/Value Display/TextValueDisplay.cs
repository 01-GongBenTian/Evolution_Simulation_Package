using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextValueDisplay : DataValueDisplay
{
    [SerializeField] private TextMeshProUGUI _Value;

    public override void UpdateValue(object value)
    {
        _Value.text = value.ToString();
    }
}
