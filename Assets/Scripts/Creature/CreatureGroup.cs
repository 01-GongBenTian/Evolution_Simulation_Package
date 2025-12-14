using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreatureGroup : MonoBehaviour
{
    private const float MIN_SCALE = 0.5f;
    private const float SCALE_RANGE = 2.0f;

    public static int COUNT = 0;

    public Vector3Int MapPosition;
    public int Index;
    public CreatureData LeaderCreature;
    public Dictionary<CreatureData, int> Creatures;
    public SpriteRenderer CreatureSprite;

    public float Health;
    public float Energy;

    public void OnReproduction()
    {
        CreatureData[] creatures = Creatures.Keys.ToArray();

        foreach (CreatureData creature in creatures)
        {
            int newBorn = creature.ReproduceNum * Creatures[creature];

            float mutationPresentage = UnityEngine.Random.Range(0.0f, 1.0f);
            if (mutationPresentage <= 0.1f)
            {
                int newSpeciesBorn = (int)(newBorn * 0.1f < 1 ? 1 : newBorn * 0.1f);
                Creatures.Add(creature.Evolute(), newSpeciesBorn);
                Creatures[creature] += (newBorn - newSpeciesBorn);
            }
            else
            {
                Creatures[creature] += newBorn;
            }
        }
    }

    public void OnConsumeResources()
    {
        //Energy = 0;
        //int population = TotalPopulation();

        //if(WorldMap.Instance.MapTiles[MapPosition.x][MapPosition.y].ResourceList.ContainsKey(WorldMap.Instance.ResCategoryList.FindResourceCategory("Mineral").List[0]))
        //{
        //    if(WorldMap.Instance.MapTiles[MapPosition.x][MapPosition.y].ResourceList[WorldMap.Instance.ResCategoryList.FindResourceCategory("Mineral").List[0]] >=  )
        //}
        //else
        //{
        //    Debug.Log("No Food");
        //}

    }

    public void OnPopulationAdjustment()
    {
        SortCreatureInPopulation();
        int population = TotalPopulation();
        if (population > LeaderCreature.GroupMax)
        {
            CreatureGroup newGroup = SplitGroup(population);
            newGroup.UpdateLeader();
            newGroup.UpdateSpriteSize();
            newGroup.MapPosition = new Vector3Int(
                Random.Range(0, WorldMap.Instance.Width),
                Random.Range(0, WorldMap.Instance.Height),
                0);
            newGroup.gameObject.transform.position = WorldMap.Instance.Base.CellToLocal(newGroup.MapPosition * 2 + new Vector3Int(1, 1, 0));
        }

        UpdateSpriteSize();
        UpdateLeader();
        UpdateSpriteSize();
    }

    public CreatureGroup SplitGroup(int population)
    {
        CreatureGroup newGroup = CreatureManager.INSTANCE.SpawnCreature();

        int newGroupPopulation = (int)Mathf.Floor(population / 2.0f);
        while (newGroupPopulation > 0)
        {
            KeyValuePair<CreatureData, int> lastKVP = Creatures.First();
            if(lastKVP.Value < newGroupPopulation)
            {
                newGroupPopulation -= lastKVP.Value;
                newGroup.Creatures.Add(lastKVP.Key, lastKVP.Value);
                Creatures.Remove(lastKVP.Key);
            }
            else
            {
                Creatures[lastKVP.Key] -= newGroupPopulation;

                newGroup.Creatures.Add(lastKVP.Key, newGroupPopulation);
                
                if (lastKVP.Value == newGroupPopulation)
                {
                    Creatures.Remove(lastKVP.Key);
                }
                newGroupPopulation = 0;
            }
        }

        return newGroup;
    }

    public void UpdateSpriteSize()
    {
        UpdateLeader();
        int population = TotalPopulation();
        float scale = Mathf.Clamp((float)(population - LeaderCreature.GroupMin) / (float)(LeaderCreature.GroupMax - LeaderCreature.GroupMin), 0.0f, 1.0f);

        gameObject.transform.localScale = new Vector3(MIN_SCALE + (SCALE_RANGE * scale), MIN_SCALE + (SCALE_RANGE * scale), MIN_SCALE + (SCALE_RANGE * scale));
    }

    public int TotalPopulation()
    {
        return Creatures.Sum(kvp => kvp.Value);
    }

    public void SortCreatureInPopulation()
    {
        Dictionary<CreatureData, int> kvps = Creatures.OrderBy(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public void UpdateLeader()
    {
        SortCreatureInPopulation();
        KeyValuePair<CreatureData, int> lastKVP = Creatures.First();
        LeaderCreature = lastKVP.Key;

        CreatureSprite.color = LeaderCreature.Code.GetCodeColor();
    }
}
