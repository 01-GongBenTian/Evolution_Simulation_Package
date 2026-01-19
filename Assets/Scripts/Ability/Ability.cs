using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public enum AbilityType 
    {
        NONE = 0,
        REPRODUCTION = NONE + 1,
        DIGESTION = REPRODUCTION + 1,
        STATS = DIGESTION + 1,
        SPECIAL = STATS + 1,
    }

    public string NameDisplay;
    public Sprite Icon;

    public abstract void Execute(object param0, object param1, object param2);
    public abstract AbilityType GetAbilityType();
}