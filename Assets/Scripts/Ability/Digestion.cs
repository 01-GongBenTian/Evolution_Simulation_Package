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

public class MonoblastCountDown
{
    public static List<MonoblastCountDown> List = new List<MonoblastCountDown>();

    public CreatureGroup Group;
    public byte CountDown;

    public MonoblastCountDown(CreatureGroup group)
    {
        Group = group;
        CountDown = 1;
    }
}


public class DiblasticCountDown
{
    public static List<DiblasticCountDown> List = new List<DiblasticCountDown>();

    public CreatureGroup Group;
    public bool NextConsume;

    public DiblasticCountDown(CreatureGroup group)
    {
        Group = group;
        NextConsume = false;
    }
}

