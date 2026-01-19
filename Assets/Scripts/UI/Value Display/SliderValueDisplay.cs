using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderValueDisplay : DataValueDisplay
{
    [SerializeField] private RectTransform _LowerOuterRange;
    [SerializeField] private RectTransform _HigherOuterRange;
    [SerializeField] private CurrentPoint _CurrentPoint;

    [SerializeField] private float _RangeLowest;
    [SerializeField] private float _RangeHighest;

    [SerializeField] private float _Total;
    [SerializeField] private float _Lower;
    [SerializeField] private float _Upper;

    public void Setup(float lowest, float highest, float lower, float upper, float value)
    {
        _RangeLowest = lowest;
        _RangeHighest = highest;
        _Total = _RangeHighest - _RangeLowest;

        _Lower = lower;
        _Upper = upper;

        _LowerOuterRange.offsetMax = new Vector2(-Mathf.Clamp(360.0f - (360.0f * ((_Lower - _RangeLowest) / _Total)), 0, 360.0f), _LowerOuterRange.offsetMax.y);
        _HigherOuterRange.offsetMin = new Vector2(Mathf.Clamp((360.0f * ((_Upper - _RangeLowest) / _Total)), 0, 360.0f), _HigherOuterRange.offsetMin.y);
        UpdateValue(value);
    }

    public override void UpdateValue(object value)
    {
        float f_value = (float)value;

        _CurrentPoint.RectTransform.anchoredPosition = new Vector2(Mathf.Clamp((360.0f * ((f_value - _RangeLowest) / _Total)), 0, 360.0f), 0);
        
        if (f_value < _Lower || f_value > _Upper)
            _CurrentPoint.OutRange();
        else
            _CurrentPoint.InRange();
    }
}
