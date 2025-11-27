using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParamInfo : MonoBehaviour
{
    [SerializeField] private Text _Label;
    [SerializeField] private Text _Value;

    public void SetLabel(string label)
    {
        _Label.text = label + ':';
    }

    public void SetValue(float value)
    {
        _Value.text = value.ToString("N2");
    }
}
