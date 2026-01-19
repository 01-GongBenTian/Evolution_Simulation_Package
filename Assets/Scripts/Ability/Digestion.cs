using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using static Resource;

public abstract class Digestion : Ability
{
    public abstract float GetEnergyWeight();
    public abstract float ResourcesWeight(Vector3Int pos);
    public override AbilityType GetAbilityType()
    {
        return AbilityType.DIGESTION;
    }
}

public class FilterFeedCountDown
{
    public static List<FilterFeedCountDown> List = new List<FilterFeedCountDown>();

    public CreatureGroup Group;
    public byte CountDown;

    public FilterFeedCountDown(CreatureGroup group)
    {
        Group = group;
        CountDown = 1;
    }
}
