using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using UnityEngine;
using static GenesCode;

public class CreatureData
{
    public static CreatureData DEFAULT_CREATURE = new CreatureData {
        Code = new GenesCode()
        {
            Code = new int[(int)Taxonomy.NUM_OF_TAXONOMY] { 1, 1, 1, 1, 1, 1, 1, 1 },
            EvoEXP = 0
        },

        GroupMax = 128,
        GroupMin = 1,

        MaxHealth = 1,
        Lifespan = 1,

        MaxEnergyStorage = 1,
        EnergyRequired = 1,

        ReproduceNum = 2
};


    public GenesCode Code;

    public int GroupMax;
    public int GroupMin;

    public int MaxHealth;
    public int Lifespan;

    public float MaxEnergyStorage;
    public float EnergyRequired;

    //public float HumidityRequired = 1;

    //public float HighestTemperatureAccept = 30;
    //public float LowestTemperatureAccept = 22;

    //public int Attack = 1;
    //public int Defenence = 1;
    //public int Speed = 1;

    public int ReproduceNum;
    //public float ReproduceNumMultipler = 1.0f;

    //public List<Ability> AbilityList = new List<Ability>();
    //public List<Resource> FoodList = new List<Resource>();

    public CreatureData Evolute()
    {
        CreatureData newData = new CreatureData();
        newData.Code = this.Code.Evolute();
        newData.ReproduceNum *= 2;



        //newData.AbilityList = new List<Ability>();
        //newData.AbilityList.CopyTo(this.AbilityList.ToArray(), 0);

        return newData;
    }
}
