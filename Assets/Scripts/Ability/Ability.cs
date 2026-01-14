using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability
{
    public enum AbilityType 
    {
        REPRODUCTION = 1,
        DIGESTION = REPRODUCTION + 1,
        STATS = DIGESTION + 1,
        SPECIAL = STATS + 1,
    }

    public AbilityType Type;
    public virtual void Execute(object param0, object param1, object param2) { }
}