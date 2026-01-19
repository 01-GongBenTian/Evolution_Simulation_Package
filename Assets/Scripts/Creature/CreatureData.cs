using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static AbilityList;
using static GenesCode.Taxonomy;

public class CreatureData
{
    public static CreatureData DefaultCreature = new CreatureData() {
        Code = new GenesCode { Code = new uint[(int)NUM_OF_TAXONOMY] { 1, 1, 1, 1, 1, 1, 1, 1 } },
        GroupMax = 2048,
        GroupMin = 256,

        Lifespan = 3,

        HumidityRequired = 1000,

        HighestTemperatureAccept = 30,
        LowestTemperatureAccept = 24,

        AbilityCarried = new List<ABILITIES>() { ABILITIES.MITOSIS, ABILITIES.FILTER_FEED },

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
        newData.Speed = this.Speed + 1;

        newData.ResourceCarryNum = this.ResourceCarryNum;

        newData.HumidityRequired = this.HumidityRequired;

        newData.HighestTemperatureAccept = this.HighestTemperatureAccept;
        newData.LowestTemperatureAccept = this.LowestTemperatureAccept;

        newData.AbilityCarried = new List<ABILITIES>();
        newData.AbilityCarried.AddRange(this.AbilityCarried);

        float lifespanWeight = (newData.Lifespan / 3);
        float humidityWeight = (int)((1000 - newData.HumidityRequired) / 50) + 1;
        float temperatureWeight = (int)((((newData.HighestTemperatureAccept - newData.LowestTemperatureAccept) - 6) / 0.25f) + 1);
        float speedWeight = (newData.Speed);
        float resourceWeight = (newData.ResourceCarryNum);

        float totalWeight = lifespanWeight + humidityWeight + temperatureWeight + speedWeight + resourceWeight;
        float weight = 0;

        float changeChance = Random.Range(0.0f, 1.0f);
        if (changeChance < (weight += lifespanWeight))
        {
            newData.Lifespan += 3 * StatsValueChange(lifespanWeight);
        }
        else if (changeChance < (weight += humidityWeight))
        {
            newData.HumidityRequired -= 50 * StatsValueChange(humidityWeight);
        }
        else if (changeChance < (weight += temperatureWeight))
        {
            int change = StatsValueChange(temperatureWeight);
            newData.HighestTemperatureAccept += 0.125f * change;
            newData.LowestTemperatureAccept -= 0.125f * change;
        }
        else if (changeChance < (weight += speedWeight))
        {
            newData.Speed += StatsValueChange(speedWeight);
        }
        else if (changeChance < (weight += resourceWeight))
        {
            newData.ResourceCarryNum += StatsValueChange(resourceWeight);
        }


        newData.ReproduceEnergyCalulcation();

        return newData;
    }

    private int StatsValueChange(float weight)
    {
        int valueChange = Random.Range(1, 10);
        float positiveChance = ((1.0f / weight) * Mathf.Pow(1.03f, weight));
        float random = Random.Range(0.0f, 1.0f);
        
        //negative
        if(random > positiveChance)
        {
            valueChange = Mathf.Clamp(valueChange, int.MinValue, (int)(weight - 1));
            return (int)(Mathf.Pow(weight - valueChange, 0.6f) - weight);
        }
        else //positive
        {
            return (int)(Mathf.Pow(weight + valueChange, 0.6f) - weight);
        }
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
        return (Digestion)AbilityList.GetInstance().Abilities[(int)AbilityCarried.Find(i => AbilityList.GetInstance().Abilities[(int)i].GetAbilityType() == Ability.AbilityType.DIGESTION)];
    }

    public Reproduction GetReproductionAbitity()
    {
        return (Reproduction)AbilityList.GetInstance().Abilities[(int)AbilityCarried.Find(i => AbilityList.GetInstance().Abilities[(int)i].GetAbilityType() == Ability.AbilityType.REPRODUCTION)];
    }
}
