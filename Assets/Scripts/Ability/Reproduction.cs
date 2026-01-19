using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Reproduction : Ability
{
    public override AbilityType GetAbilityType()
    {
        return AbilityType.REPRODUCTION;
    }
}
