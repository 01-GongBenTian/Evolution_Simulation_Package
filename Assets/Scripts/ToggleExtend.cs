using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleExtend : MonoBehaviour
{
    [SerializeField] private UnityEvent OnToggleOn;

    [SerializeField] private Sprite _OnSprite;
    [SerializeField] private Sprite _OffSprite;
    private Toggle _Toggle;
    

    // Start is called before the first frame update
    void Start()
    {
        _Toggle = GetComponent<Toggle>();
        OnToggleChanged();
    }

    public void OnToggleChanged()
    {
        if(_Toggle.isOn)
        {
            _Toggle.image.sprite = _OnSprite;
            OnToggleOn.Invoke();
        }
        else
        {
            _Toggle.image.sprite = _OffSprite;
        }
    }
}
