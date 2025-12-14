using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CreatureManager : MonoBehaviour
{
    public static CreatureManager INSTANCE;

    public GameObject CreatureGroupPrefab;
    public Dictionary<int, CreatureGroup> CreatureGroups;

    public ResourceCategoryList ResourceList;

    [Range(0, 1)]
    public float EvoluteChance;

    public void Start()
    {
        if(!INSTANCE)
        {
            INSTANCE = this;
           
        }
        else
        {
            Destroy(this);
        }

        CreatureGroups = new Dictionary<int, CreatureGroup>();
    }

    public void OnNewTurn()
    {
        //Reproduce
        Reproduction();

        //Consume resources
        ConsumeResource();

        //Despose reources

        //Decrese lifespan

        //Population adjust
        PopulationAdjustment();
    }

    public void Reproduction()
    {
        if (CreatureGroups.Count <= 0)
            return;

        int[] groupIndexs = CreatureGroups.Keys.ToArray<int>();

        foreach(int index in groupIndexs)
        {
            CreatureGroups[index].OnReproduction();
        }
    }

    public void ConsumeResource()
    {
        if (CreatureGroups.Count <= 0)
            return;

        //CreatureGroup group;
        //int[] groupIndexs = CreatureGroups.Keys.ToArray<int>();

        //Dictionary<CreatureData, int> creatures;
        //CreatureData[] creatureTypes;

        //foreach (int index in groupIndexs)
        //{
        //    group = CreatureGroups[index];
        //    creatures = group.Creatures;
        //    creatureTypes = group.Creatures.Keys.ToArray();

        //    Vector3Int pos = WorldMap.Instance.Base.WorldToCell(group.transform.position);
        //    pos /= 2;

        //    foreach (CreatureData creatureType in creatureTypes)
        //    {
        //        //WorldMap.Instance.MapTiles[pos.x][pos.y].ResourceList[WorldMap.Instance.ResCategoryList.FindResourceCategory("Mineral")].;
        //    }
        //}
    }


    public void PopulationAdjustment()
    {
        if (CreatureGroups.Count <= 0)
            return;

        int[] groupIndexs = CreatureGroups.Keys.ToArray<int>();

        foreach (int index in groupIndexs)
        {
            CreatureGroups[index].OnPopulationAdjustment();
        }

        //CreatureGroup group;
        //int[] groupIndexs = CreatureGroups.Keys.ToArray<int>();

        //Dictionary<CreatureData, int> creatures;



        //int totalPopulation = 0;
        //List<CreatureData> creatureOrderInPopulationSize = new List<CreatureData>();

        //foreach (int index in groupIndexs)
        //{
        //    group = CreatureGroups[index];

        //    totalPopulation = group.TotalPopulation();
        //    group.UpdateLeader();



        //    creatures = group.Creatures;

        //    creatureTypes = creatures.Keys.ToArray<CreatureData>();

        //    totalPopulation = creatures[creatureTypes[0]];
        //    creatureOrderInPopulationSize.Clear();
        //    creatureOrderInPopulationSize.Add(creatureTypes[0]);

        //    //get the total population of the creature group
        //    for (int ii = 1; ii < creatureTypes.Count(); ++ii)
        //    {
        //        totalPopulation += creatures[creatureTypes[ii]];
        //        for(int iii = 0; iii < creatureOrderInPopulationSize.Count; ++iii)
        //        {
        //            if (creatures[creatureOrderInPopulationSize[iii]] > creatures[creatureTypes[ii]])
        //            {
        //                creatureOrderInPopulationSize.Insert(iii, creatureTypes[ii]);
        //                break;
        //            }
        //        }
        //    }

        //    //output creature group information
        //    string groupInformation = string.Format("Creature Index: {0}\n Total population: {1}\n", index, totalPopulation);
        //    for(int ii = 0; ii < creatureOrderInPopulationSize.Count; ++ii)
        //    {
        //        groupInformation += string.Format("{0}: {1}\n", creatureOrderInPopulationSize[ii].Code.GetCode(), creatures[creatureOrderInPopulationSize[ii]]);
        //    }
        //    Debug.Log(groupInformation);


        //    group.LeaderCreature = creatureOrderInPopulationSize[creatureOrderInPopulationSize.Count - 1];
        //    while (group.TotalPopulation() > (group.LeaderCreature.GroupLimit + group.LeaderCreature.GroupFloor))
        //    {
        //        //CreatureGroup newGroup = SpawnCreature();
        //        //int exceedNum = totalPopulation - group.LeaderCreature.GroupLimit;

        //        //for(int i = 0; i < creatureOrderInPopulationSize.Count; ++i)
        //        //{
        //        //    //population is not enough
        //        //    if(creatures[creatureOrderInPopulationSize[i]] < creatureOrderInPopulationSize[i].GroupFloor)
        //        //    {
        //        //        newGroup.Creatures.Add(creatureOrderInPopulationSize[i], creatures[creatureOrderInPopulationSize[i]]);
        //        //        creatures.Remove(creatureOrderInPopulationSize[i]);

        //        //    }
        //        //    else
        //        //    {

        //        //    }


        //        //}

        //        ////for()

        //        Debug.Log("Group " + index + " is Oversize");
        //        break;
        //    }
        //}
    }

    public CreatureGroup SpawnCreature()
    {
        GameObject newCreature = Instantiate(CreatureGroupPrefab, WorldMap.Instance.transform);
        CreatureGroup group = newCreature.GetComponent<CreatureGroup>();
        group.Index = CreatureGroup.COUNT++;
        group.Creatures = new Dictionary<CreatureData, int>();

        CreatureGroups.Add(group.Index, group);
        return group;
    }

    public void SpawnDefaultCreature()
    {
        CreatureGroup group = SpawnCreature();
        group.Creatures.Add(CreatureData.DEFAULT_CREATURE, 1);

        group.UpdateLeader();
        group.CreatureSprite.color = group.LeaderCreature.Code.GetCodeColor();
        group.MapPosition = InputManager.Instance.TileSelectedPos;

        group.gameObject.transform.localPosition = WorldMap.Instance.Base.CellToLocal((InputManager.Instance.TileSelectedPos * 2) + new Vector3Int(1, 1, 0));
    }
}
