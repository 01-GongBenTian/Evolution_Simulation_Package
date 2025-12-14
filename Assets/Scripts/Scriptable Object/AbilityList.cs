using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability List", menuName = "Scriptable/Creature/Ability List")]
public class AbilityList : ScriptableObject
{
    [SerializeField] private List<string> RegisterName;
    [SerializeField] private List<Ability> RegisterAbility;

    public Ability GetAbility(string name)
    {
        int index = RegisterName.FindIndex(s => s.CompareTo(name) == 0);

        return RegisterAbility[index];
    }
}
