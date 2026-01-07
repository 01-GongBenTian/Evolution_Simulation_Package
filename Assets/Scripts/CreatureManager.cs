using DG.Tweening;
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

    public Dictionary<uint, CreatureGroup> CreatureGroups;

    public GameObject CreatureGroupPrefab;

    public ResourceCategoryList ResourceList;

    [Range(0, 1)]
    public float EvoluteChance;

    [Range(1, 10)]
    public float TemperatureTolerance;

    [Range(1, 200)]
    public float HumidityTolerance;

    [Range(0.5f, 1)]
    public float ReproducePower;

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

        CreatureGroups = new Dictionary<uint, CreatureGroup>();
    }

    public void OnNewTurn()
    {
        //Consume resources
        ConsumeResource();

        //Activity
        Activity();

        //Reproduce
        Reproduction();

        //Population adjust
        PopulationAdjustment();
    }

    public void ConsumeResource()
    {
        if (CreatureGroups.Count <= 0)
            return;

        uint[] groupIndexes = CreatureGroups.Keys.ToArray();
        foreach (uint index in groupIndexes)
        {
            CreatureGroups[index].OnConsumeResources();
        }
    }

    public void Activity()
    {
        if (CreatureGroups.Count <= 0)
            return;

        uint[] groupIndexs = CreatureGroups.Keys.ToArray();
        foreach(uint index in groupIndexs)
        {
            CreatureGroups[index].OnActivity();
        }
    }

    public void Reproduction()
    {
        if (CreatureGroups.Count <= 0)
            return;

        Debug.Log("==================================================================================");
        uint[] groupIndexes = CreatureGroups.Keys.ToArray<uint>();
        foreach(uint index in groupIndexes)
        {
            CreatureGroups[index].OnReproduction();
        }
    }

    public void PopulationAdjustment()
    {
        if (CreatureGroups.Count <= 0)
            return;

        uint[] groupIndexs = CreatureGroups.Keys.ToArray<uint>();

        foreach (uint index in groupIndexs)
        {
            CreatureGroups[index].OnPopulationAdjustment();
        }
    }

    public CreatureGroup CreateNewCreatureGroup()
    {
        //create game object
        GameObject newCreature = Instantiate(CreatureGroupPrefab, WorldMap.INSTANCE.transform);
        CreatureGroup group = newCreature.GetComponent<CreatureGroup>();

        //set the index of the creature group
        group.Index = CreatureGroup.COUNT++;

        //set the canvas camera
        group.Canvas.worldCamera = Camera.main;

        //initialize the creature list
        group.Creatures = new Dictionary<CreatureData, int>();
        group.ResourcesCarried = new Dictionary<Resource, int>();

        group.CreatureLifes = new Dictionary<CreatureData, int>();
        group.Energy = 0;

        //add this group to creature group list
        CreatureGroups.Add(group.Index, group);

        return group;
    }

    public void SpawnDefaultCreature()
    {
        CreatureGroup group = CreateNewCreatureGroup();

        //add in the creature into the creature list of the group
        group.AddInCreature(CreatureData.DefaultCreature, 1);

        //update the leader of the creature group
        group.UpdateLeader();
        group.UpdateSpriteSize();

        //update the position of the creature group
        group.MapPosition = InputManager.INSTANCE.TileSelectedPos;
        group.gameObject.transform.localPosition = WorldMap.INSTANCE.Base.CellToLocal((InputManager.INSTANCE.TileSelectedPos * 2) + new Vector3Int(1, 1, 0));
    }

    public void RemoveCreatureGroup(uint index)
    {
        CreatureGroup group = CreatureGroups[index];
        CreatureGroups.Remove(index);

        group.CodeLabel.text = "Dead";
        group.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => { Destroy(group.gameObject); });
    }
}
