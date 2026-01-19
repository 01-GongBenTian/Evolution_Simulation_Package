using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListDisplayData : MonoBehaviour
{
    [SerializeField] private Image _Icon;
    [SerializeField] private TextMeshProUGUI _AbilityName;

    public void SetAbility(Ability ability)
    {
        if(ability != null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        _Icon.sprite = ability.Icon;
        _AbilityName.text = ability.NameDisplay;
    }
}
