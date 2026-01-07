using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Ability.AbilityType;
using static AbilityList.ABILITIES;

public class CreatureGroup : MonoBehaviour
{
    private const float MIN_SCALE = 0.5f;
    private const float SCALE_RANGE = 1.0f;

    public static uint COUNT = 0;

    public uint Index;
    public Vector3Int MapPosition;
    public CreatureData LeaderCreature;
    public Dictionary<CreatureData, int> Creatures;

    public SpriteRenderer Sprite;

    public Canvas Canvas;
    public Text CodeLabel;

    public float Energy;
    public Dictionary<CreatureData, int> CreatureLifes;
    public Dictionary<Resource, int> ResourcesCarried;

    public void OnConsumeResources()
    {
        LeaderCreature.GetDigestionAbility().Execute(this, null, null);
    }

    public void OnActivity()
    {
        //movement
        Vector3 direction = MoveIn();
        float energyUsed = Mathf.Clamp(Mathf.Pow(LeaderCreature.Speed * Mathf.Pow(Creatures.Sum(kvp => kvp.Value), 1.05f), 0.8f), 0, Energy);
        if (direction.sqrMagnitude > 0.1f && energyUsed > 0.0001f)
        {
            transform.DOMove(transform.position + (direction * LeaderCreature.Speed * Time.deltaTime * Mathf.Clamp(Energy / energyUsed, 0, 1)), SystemManager.INSTANCE.TimerCount).OnComplete(() => { MapPosition = WorldMap.INSTANCE.Base.WorldToCell(transform.position) / 2; });
            Energy -= energyUsed;
        }


        //combat

        //combine

        //life span and death
        KeyValuePair<CreatureData, int>[] kvps = Creatures.ToArray();
        foreach (KeyValuePair<CreatureData, int> kvp in kvps)
        {
            int lifeDecrease = (int)(kvp.Value * LifespanMultipler(kvp.Key));

            CreatureLifes[kvp.Key] -= lifeDecrease;
            
            int deathNum = CreatureLifes[kvp.Key] / -kvp.Key.Lifespan;

            Creatures[kvp.Key] -= deathNum;
            CreatureLifes[kvp.Key] += kvp.Key.Lifespan * deathNum;

            if(Creatures[kvp.Key] <= 0)
            {
                RemoveCreature(kvp.Key);
            }
        }

        //if this creature group all die
        if(Creatures.Count == 0)
        {
            CreatureManager.INSTANCE.RemoveCreatureGroup(Index);
        }
    }

    public void OnReproduction()
    {
        Debug.Log(Energy);
        float totalWeight = 0;
        float[] weights = new float[Creatures.Count];
        CreatureData[] creatures = Creatures.Keys.ToArray();

        //calculate the weight to share energy
        int index = 0;
        foreach (var creature in creatures)
        {
            weights[index] = creature.ResourceCarryNum * creature.GetDigestionAbility().EnergyWeight * Mathf.Pow(Creatures[creature], 0.6f);
            totalWeight += weights[index];

            ++index;
        }

        index = 0;
        float[] energyShared = new float[1] { 0 };
        float energyLefted = 0;
        foreach(var creature in creatures)
        {
            energyShared[0] = Energy * (weights[index] / totalWeight);
            energyShared[0] += energyLefted;

            creature.GetReproductionAbitity().Execute(this, creature, energyShared);
            energyLefted = energyShared[0];
            ++index;
        }

        Energy = energyLefted;
        Debug.Log(Energy);
    }



    public void OnPopulationAdjustment()
    {
        //Debug.Log($"=== 开始种群调整 ===");
        //Debug.Log($"生物数量: {Creatures.Count}");

        //// 检查最大值、最小值、总和
        //if (Creatures.Any())
        //{
        //    int max = Creatures.Max(c => c.Value);
        //    int min = Creatures.Min(c => c.Value);
        //    long longSum = Creatures.Sum(c => (long)c.Value);

        //    Debug.Log($"属性最大值: {max}");
        //    Debug.Log($"属性最小值: {min}");
        //    Debug.Log($"使用long计算的总和: {longSum}");
        //    Debug.Log($"int.MaxValue: {int.MaxValue}");
        //    Debug.Log($"int.MinValue: {int.MinValue}");

        //    // 检查是否溢出
        //    if (longSum > int.MaxValue)
        //        Debug.LogError($"总和 {longSum} > int.MaxValue");
        //    if (longSum < int.MinValue)
        //        Debug.LogError($"总和 {longSum} < int.MinValue");
        //}

        //try
        //{
        //    // 你的原始代码
        //    int population = Creatures.Sum(kvp => kvp.Value);
        //    while (population > LeaderCreature.GroupMax)
        //    {
        //        CreatureGroup newGroup = SplitGroup(ref population);
        //        newGroup.MapPosition = MapPosition;
        //        newGroup.gameObject.transform.position = WorldMap.INSTANCE.Base.CellToLocal(newGroup.MapPosition * 2 + new Vector3Int(1, 1, 0));

        //        newGroup.UpdateLeader();
        //        newGroup.UpdateSpriteSize();
        //    }

        //    UpdateLeader();
        //    UpdateSpriteSize();
        //}
        //catch (OverflowException ex)
        //{
        //    Debug.LogError($"溢出异常详情: {ex}");

        //    // 检查每个值
        //    foreach (var creature in Creatures)
        //    {
        //        Debug.Log($"生物: ID={creature.Key}, 属性值={creature.Value}");
        //    }
        //}

        int population = Creatures.Sum(kvp => kvp.Value);
        while (population > LeaderCreature.GroupMax)
        {
            CreatureGroup newGroup = SplitGroup(ref population);
            newGroup.MapPosition = MapPosition;
            newGroup.gameObject.transform.position = transform.position;

            newGroup.UpdateLeader();
            newGroup.UpdateSpriteSize();
        }

        UpdateLeader();
        UpdateSpriteSize();
    }

    public CreatureGroup SplitGroup(ref int population)
    {
        //rearrange in asc by population
        List<KeyValuePair<CreatureData, int>> creatures = Creatures.OrderBy(kvp => kvp.Value).ToList();

        CreatureGroup newGroup = CreatureManager.INSTANCE.CreateNewCreatureGroup();
        int newGroupPopulation = LeaderCreature.GroupMin;
        population -= newGroupPopulation;

        while (newGroupPopulation > 0)
        {
            //get the smallest population creature in the group
            KeyValuePair<CreatureData, int> smallestPopulation = creatures.First();

            //if the smallest population is smaller than the new group population should have
            if(smallestPopulation.Value < newGroupPopulation)
            {
                newGroupPopulation -= smallestPopulation.Value;
                
                //add the creature to the new group
                newGroup.AddInCreature(smallestPopulation.Key, smallestPopulation.Value);

                //remove the creature in the old group
                RemoveCreature(smallestPopulation.Key);

                //remove the creature sorted kvps
                creatures.RemoveAt(0);
            }
            else
            {
                Creatures[smallestPopulation.Key] -= newGroupPopulation;

                newGroup.AddInCreature(smallestPopulation.Key, newGroupPopulation);
                
                if (Creatures[smallestPopulation.Key] == 0)
                {
                    RemoveCreature(smallestPopulation.Key);
                }

                newGroupPopulation = 0;
            }
        }

        return newGroup;
    }

    public void UpdateSpriteSize()
    {
        //calculate the size according to population size
        int population = Creatures.Sum(kvp => kvp.Value);
        float scale = Mathf.Clamp((float)(population - LeaderCreature.GroupMin) / (float)(LeaderCreature.GroupMax - LeaderCreature.GroupMin), 0.0f, 1.0f);

        //apply do animation on scale changes
        Sprite.transform.DOScale(
            new Vector3(MIN_SCALE + (SCALE_RANGE * scale), MIN_SCALE + (SCALE_RANGE * scale), MIN_SCALE + (SCALE_RANGE * scale)), 
            SystemManager.INSTANCE.TimerCount);
    }


    public void UpdateLeader()
    {
        KeyValuePair<CreatureData, int>[] kvps = Creatures.OrderBy(kvp => kvp.Value).ToArray();
        KeyValuePair<CreatureData, int> largestPopulation = Creatures.OrderByDescending(kvp => kvp.Value).First();
        LeaderCreature = largestPopulation.Key;

        Sprite.color = LeaderCreature.Code.GetCodeColor();
        CodeLabel.text = LeaderCreature.Code.GetCode();
    }

    public void AddInCreature(CreatureData creature, int num)
    {
        Creatures.Add(creature, num);
        CreatureLifes.Add(creature, 0);
    }

    public void RemoveCreature(CreatureData creature)
    {
        Creatures.Remove(creature);
        CreatureLifes.Remove(creature);
    }

    public float LifespanMultipler(CreatureData creature)
    {
        float multipler = 1.0f;
        float temperature = WorldMap.INSTANCE.MapTiles[MapPosition.x][MapPosition.y].Temperature;
        float humidity = WorldMap.INSTANCE.MapTiles[MapPosition.x][MapPosition.y].Humidity;

        if (temperature > creature.HighestTemperatureAccept)
        {
            multipler *= Mathf.Clamp(((temperature - creature.HighestTemperatureAccept) / CreatureManager.INSTANCE.TemperatureTolerance), 1.0f, 3.0f);
        }
        else if (temperature < creature.LowestTemperatureAccept)
        {
            multipler *= Mathf.Clamp(((creature.LowestTemperatureAccept - temperature) / CreatureManager.INSTANCE.TemperatureTolerance), 1.0f, 3.0f);
        }

        if(humidity > 0 && humidity < creature.HumidityRequired)
        {
            multipler *= Mathf.Clamp(((creature.HumidityRequired - humidity) / CreatureManager.INSTANCE.HumidityTolerance), 1.0f, 3.0f);
        }

        return multipler;
    }

    private float LifespanMultipler(CreatureData creature, Vector3Int pos)
    {
        float multipler = 1.0f;
        float temperature = WorldMap.INSTANCE.MapTiles[pos.x][pos.y].Temperature;
        float humidity = WorldMap.INSTANCE.MapTiles[pos.x][pos.y].Humidity;

        if (temperature > creature.HighestTemperatureAccept)
        {
            multipler *= Mathf.Clamp(((temperature - creature.HighestTemperatureAccept) / CreatureManager.INSTANCE.TemperatureTolerance), 1.0f, 3.0f);
        }
        else if (temperature < creature.LowestTemperatureAccept)
        {
            multipler *= Mathf.Clamp(((creature.LowestTemperatureAccept - temperature) / CreatureManager.INSTANCE.TemperatureTolerance), 1.0f, 3.0f);
        }

        if (humidity > 0 && humidity < creature.HumidityRequired)
        {
            multipler *= Mathf.Clamp(((creature.HumidityRequired - humidity) / CreatureManager.INSTANCE.HumidityTolerance), 1.0f, 3.0f);
        }

        return multipler;
    }

    private float ResourceWeight(Vector3Int pos)
    {
        int index = (int)LeaderCreature.AbilityCarried.Find(i => AbilityList.INSTANCE.Abilities[(int)i].Type == Ability.AbilityType.DIGESTION);
        Digestion dig = (Digestion)AbilityList.INSTANCE.Abilities[index];

        return dig.ResourcesWeight(pos);
    }

    private float CreatureWeight(Vector3Int pos)
    {
        float weight = 0;
        List<KeyValuePair<uint, CreatureGroup>> kvps = CreatureManager.INSTANCE.CreatureGroups.Where(
            kvp => kvp.Value.MapPosition.x == pos.x && 
            kvp.Value.MapPosition.y == pos.y && 
            kvp.Value != this).ToList();

        float tempWeight = 0;
        foreach (var kvp in kvps)
        {
            tempWeight = kvp.Value.Creatures.Sum(kvp => kvp.Value) * kvp.Value.LeaderCreature.ResourceCarryNum;

            if (LeaderCreature.Code.IsSameSpecies(kvp.Value.LeaderCreature.Code))
            {
                tempWeight *= 1.5f;
            }

            weight += tempWeight;
        }

        return weight;
    }

    public Vector3 MoveIn()
    {
        float[][] weights = new float[3][];
        weights[0] = new float[3] { 0, 0, 0 };
        weights[1] = new float[3] { 0, 0, 0 };
        weights[2] = new float[3] { 0, 0, 0 };

        float weight = 0;
        Vector3Int pos = Vector3Int.zero;
        for (int x = -1; x < 2; ++x)
        {
            for (int y = -1; y < 2; ++y)
            {
                pos.x = MapPosition.x + x;
                pos.y = MapPosition.y + y;

                if ((pos.x >= WorldMap.INSTANCE.Width || pos.x < 0) ||
                    (pos.y >= WorldMap.INSTANCE.Height || pos.y < 0))
                {
                    weights[x + 1][y + 1] = float.MinValue;
                    continue;
                }

                //resources condition
                weight = ResourceWeight(pos);

                //environment condition
                weight /= LifespanMultipler(LeaderCreature, pos);

                //same spiece desnity
                weight -= CreatureWeight(pos);

                weights[x + 1][y + 1] = weight;
            }
        }
        weights[0][0] *= Mathf.Pow(Creatures.Sum(kvp => kvp.Value), 0.08f);

        int highestX = 0;
        int highestY = 0;
        for (int x = 0; x < 3; ++x)
        {
            for (int y = 0; y < 3; ++y)
            {
                if (weights[x][y] > weights[highestX][highestY])
                {
                    highestX = x;
                    highestY = y;
                }
            }
        }

        if (highestX == 1 && highestY == 1)
        {
            return Vector3.zero;
        }

        Vector3 direciton = WorldMap.INSTANCE.Base.CellToWorld(new Vector3Int(MapPosition.x * 2 + highestX, MapPosition.y * 2 + highestY, 0)) - transform.position;
        direciton.Normalize();


        return direciton;
    }
}
