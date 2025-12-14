using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityEffect
{
    public enum StatsType
    {
        MAX_HEALTH = 0,
        LIFESPAN,

        MAX_ENERGY,
        ENERGY_USEAGE,

        HUMIDITY_REQUIREMENT,

        HIGHEST_TEMPERATURE,
        LOWEST_TEMPERATURE,

        ATTACK,
        DEFEND,
        SPEED,

        FOOD_LIST,

        REPRODUCE_NUM,
    }

    public enum ModifierType
    {
        BASE = 0,
        MULTIPLER
    }

    public StatsType Stats;
    public ModifierType Modifier;
    public object Value;
}
