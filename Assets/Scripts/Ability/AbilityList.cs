using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ability;

[CreateAssetMenu(fileName = "Ability List", menuName = "Scriptable/Ability/Ability List")]
public class AbilityList : ScriptableObject
{
    public enum ABILITIES
    {
        MITOSIS = 0,
        FILTER_FEED,
        NUM_OF_ABILITY
    }

    private static AbilityList INSTANCE;
    public List<Ability> Abilities;

    public static AbilityList GetInstance()
    {
        if(!INSTANCE)
            INSTANCE = Resources.Load<AbilityList>("ScriptableObject/Ability/Ability List");
        
        return INSTANCE;
    }
}
