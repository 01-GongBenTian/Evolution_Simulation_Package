using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static AbilityList;
using static GenesCode.Taxonomy;

public class CreatureData
{
    public static CreatureData DefaultCreature = new CreatureData() {
        Code = new GenesCode { Code = new uint[(int)NUM_OF_TAXONOMY] { 1, 1, 1, 1, 1, 1, 1, 1 } },
        GroupMax = 16384,
        GroupMin = 2048,

        Lifespan = 3,

        HumidityRequired = 1000,

        HighestTemperatureAccept = 30,
        LowestTemperatureAccept = 24,

        AbilityCarried = new List<ABILITIES>() { ABILITIES.MONO_REPRODUCE, ABILITIES.FILTER_FEED },

        Attack = 1,
        Defenence = 1,
        Speed = 1,

        ResourceCarryNum = 1,
        ReproduceEnergyRequired = 1
    };


    public GenesCode Code;

    public int GroupMax;
    public int GroupMin;

    public int Lifespan;

    public float HumidityRequired;

    public float HighestTemperatureAccept;
    public float LowestTemperatureAccept;

    public int Attack;
    public int Defenence;
    public int Speed;

    public int ResourceCarryNum;

    public List<ABILITIES> AbilityCarried;
    //public List<Resource> FoodList = new List<Resource>();

    //Resource Consumption
    //public List<Resource> FoodList;
    //public Dictionary<Resource, int> CarriedResources;

    public int ReproduceEnergyRequired;

    public CreatureData Evolute()
    {
        CreatureData newData = new CreatureData();
        newData.Code = this.Code.Evolute();

        newData.GroupMax = this.GroupMax;
        newData.GroupMin = this.GroupMin;

        newData.Lifespan = this.Lifespan;

        newData.Attack = this.Attack;
        newData.Defenence = this.Defenence;
        newData.Speed = this.Speed;

        newData.ResourceCarryNum = this.ResourceCarryNum;

        newData.HumidityRequired = this.HumidityRequired;

        newData.HighestTemperatureAccept = this.HighestTemperatureAccept;
        newData.LowestTemperatureAccept = this.LowestTemperatureAccept;

        newData.AbilityCarried = new List<ABILITIES>();
        newData.AbilityCarried.AddRange(this.AbilityCarried);

        newData.ReproduceEnergyCalulcation();

        return newData;
    }

    private void ReproduceEnergyCalulcation()
    {
        ReproduceEnergyRequired = 1;

        //health
        ReproduceEnergyRequired += (Lifespan - 3) / 2;

        //stats
        ReproduceEnergyRequired += (Attack - 1) / 2;
        ReproduceEnergyRequired += (Defenence - 1) / 2;
        ReproduceEnergyRequired += (Speed - 1) / 2;

        //resource carry
        ReproduceEnergyRequired += (ResourceCarryNum - 1) / 8;

        //ability
    }

    public Digestion GetDigestionAbility()
    {
        return (Digestion)AbilityList.INSTANCE.Abilities[(int)AbilityCarried.Find(i => AbilityList.INSTANCE.Abilities[(int)i].Type == Ability.AbilityType.DIGESTION)];
    }

    public Reproduction GetReproductionAbitity()
    {
        return (Reproduction)AbilityList.INSTANCE.Abilities[(int)AbilityCarried.Find(i => AbilityList.INSTANCE.Abilities[(int)i].Type == Ability.AbilityType.REPRODUCTION)];
    }
}
