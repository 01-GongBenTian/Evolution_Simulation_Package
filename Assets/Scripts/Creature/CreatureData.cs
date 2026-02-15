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

        AbilityCarried = new List<ABILITIES>() { ABILITIES.MITOSIS, ABILITIES.MONOBLASTIC },

        Attack = 1,
        Defenence = 1,
        Speed = 1,

        ResourceCarryNum = 1,
        ReproduceEnergyRequired = 1,

        CreatureColor = Color.black
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

    public Color CreatureColor;

    public CreatureData Evolute()
    {
        CreatureData newData = new CreatureData();
        newData.Code = this.Code.Evolute();
        CopyData(newData);

        int lifespanWeight = (newData.Lifespan / 3);
        int humidityWeight = (int)((1000 - newData.HumidityRequired) / 50) + 1;
        int highestTemperatureWeight = (int)(((newData.HighestTemperatureAccept - DefaultCreature.HighestTemperatureAccept) / 0.5f) + 1);
        int lowestTemperatureWeight = (int)(((DefaultCreature.LowestTemperatureAccept - newData.LowestTemperatureAccept) / 0.5f) + 1);
        int speedWeight = (newData.Speed);
        int resourceWeight = (newData.ResourceCarryNum);

        int totalWeight = lifespanWeight + humidityWeight + highestTemperatureWeight + lowestTemperatureWeight + speedWeight + resourceWeight;
        
        int weight = 0;
        int changeChance = Random.Range(0, totalWeight + 1);
        if (changeChance < (weight += lifespanWeight))
        {
            int change = StatsValueChange(lifespanWeight);
            newData.Lifespan += 3 * change;
            lifespanWeight += change;
            totalWeight += change;
        }
        else if (changeChance < (weight += humidityWeight))
        {
            int change = StatsValueChange(humidityWeight);
            newData.HumidityRequired -= 50 * change;
            humidityWeight += change;
            totalWeight += change;
        }
        else if (changeChance < (weight += highestTemperatureWeight))
        {
            int change = StatsValueChange(highestTemperatureWeight);
            newData.HighestTemperatureAccept += 0.75f * change;
            highestTemperatureWeight += change;
            totalWeight += change;
        }
        else if (changeChance < (weight += lowestTemperatureWeight))
        {
            int change = StatsValueChange(lowestTemperatureWeight);
            newData.LowestTemperatureAccept -= 0.75f * change;
            lowestTemperatureWeight += change;
            totalWeight += change;
        }
        else if (changeChance < (weight += speedWeight))
        {
            int change = StatsValueChange(speedWeight);
            newData.Speed += change;
            speedWeight += change;
            totalWeight += change;
        }
        else if (changeChance < (weight += resourceWeight))
        {
            int change = StatsValueChange(resourceWeight);
            newData.ResourceCarryNum += change;
            resourceWeight += change;
            totalWeight += change;
        }

        newData.ReproduceEnergyCalulcation();


        newData.CreatureColor = (Color.green * (lifespanWeight / (float)totalWeight)) +
            (new Color(1, 0.5f, 0) * (humidityWeight / (float)totalWeight)) +
            (Color.red * (highestTemperatureWeight / (float)totalWeight)) +
            (Color.blue * (lowestTemperatureWeight / (float)totalWeight)) +
            (Color.yellow * (speedWeight / (float)totalWeight)) +
            (new Color(0.5f, 0, 0.5f) * (lowestTemperatureWeight / (float)totalWeight));

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
            return (int)(Mathf.Pow(weight - valueChange, 0.76f) - weight);
        }
        else //positive
        {
            return (int)(Mathf.Pow(weight + valueChange, 0.76f) - weight);
        }
    }

    private void CopyData(CreatureData newData)
    {
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
