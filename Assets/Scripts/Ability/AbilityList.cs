using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ability;

public class AbilityList
{
    public enum ABILITIES
    {
        MONO_REPRODUCE = 0,
        FILTER_FEED,
        NUM_OF_ABILITY
    }

    public static AbilityList INSTANCE = new AbilityList();
    public List<Ability> Abilities;

    private AbilityList()
    {
        Abilities = new List<Ability>((int)ABILITIES.NUM_OF_ABILITY);

        //Reproduction
        Abilities.Add(new MonoReproduction());

        //Digestion
        Abilities.Add(new FilterFeed());
    }
}
