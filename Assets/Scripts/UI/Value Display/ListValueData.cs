using UnityEngine;

public class ListValueDisplay : DataValueDisplay
{
    [SerializeField] private RectTransform _Value;
    [SerializeField] private ListDisplayData[] _Displays;

    public override void UpdateValue(object value)
    {
        
    }
}
