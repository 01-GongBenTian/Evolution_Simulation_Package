using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Resource;

[CreateAssetMenu(fileName = "Diblastic", menuName = "Scriptable/Ability/Digestion/Diblastic")]
public class Diblastic : Digestion
{
    public override void Execute(object param0, object param1, object param2)
    {
        CreatureGroup group = (CreatureGroup)param0;
        DiblasticCountDown countDown = DiblasticCountDown.List.Find(i => i.Group == group);

        if (countDown == null || countDown.NextConsume)
        {
            ConsumeResource(group);

            if (countDown == null)
            {
                DiblasticCountDown.List.Add(new DiblasticCountDown(group));
            }
            else
            {
                countDown.NextConsume = false;
            }

        }
        else
        {
            Digestion(group);

            countDown.NextConsume = true;
        }
    }

    public override float GetEnergyWeight()
    {
        return 1.5f;
    }

    public override float ResourcesWeight(Vector3Int pos)
    {
        float weight = 0;
        List<KeyValuePair<Resource, int>> kvps = WorldMap.INSTANCE.MapTiles[pos.x][pos.y].ResourceList.ToList();

        foreach (var kvp in kvps)
        {
            if (kvp.Value == 0)
            {
                continue;
            }

            switch (kvp.Key.Level)
            {
                case ResourceLevel.LEVEL_2:
                    {
                        weight += kvp.Value;
                        break;
                    }
                case ResourceLevel.LEVEL_3:
                    {
                        weight += kvp.Value / 5.7f;
                        break;
                    }
                case ResourceLevel.LEVEL_4:
                    {
                        weight += kvp.Value / 13.0f;
                        break;
                    }
                case ResourceLevel.LEVEL_5:
                    {
                        weight += kvp.Value / 30.0f;
                        break;
                    }
            }
        }

        return weight;
    }

    private void ConsumeResource(CreatureGroup group)
    {
        int totalToConsume = 0;
        foreach (var kvp in group.Creatures)
        {
            totalToConsume += kvp.Key.ResourceCarryNum * kvp.Value;
        }

        int consumeAmount = 0;
        List<KeyValuePair<Resource, int>> resources = WorldMap.INSTANCE.MapTiles[group.MapPosition.x][group.MapPosition.y].ResourceList.OrderBy(i => i.Key.Level).ToList();
        foreach(KeyValuePair<Resource, int> resource in resources)
        {
            if (totalToConsume == 0)
                break;

            if (resource.Value == 0)
                continue;

            //get the possible amount of this resource can consume
            consumeAmount = Mathf.Clamp(totalToConsume, 0, resource.Value);


            //if in the group don't have resources
            if(group.ResourcesCarried.ContainsKey(resource.Key))
                group.ResourcesCarried.Add(resource.Key, 0);
            
            //add the resource to creature group and deduct the resource from map
            group.ResourcesCarried[resource.Key] += consumeAmount;
            WorldMap.INSTANCE.MapTiles[group.MapPosition.x][group.MapPosition.y].ResourceList[resource.Key] -= consumeAmount;
        }

    }

    private void Digestion(CreatureGroup group)
    {
        int totalToDigest = 0;
        foreach (var kvp in group.Creatures)
        {
            totalToDigest += kvp.Key.ResourceCarryNum * kvp.Value;
        }

        int digestNum = 0;
        List<KeyValuePair<Resource, int>> resources = group.ResourcesCarried.OrderBy(i => i.Key.Level).ToList();
        foreach(KeyValuePair<Resource, int> resource in resources)
        {
            if (totalToDigest == 0)
                break;

            if (resource.Value == 0)
                continue;

            switch (resource.Key.Level)
            {
                case Resource.ResourceLevel.LEVEL_2:
                    {
                        digestNum = Mathf.Clamp(resource.Value, 1, totalToDigest);

                        //calulcate the energy get from the resources
                        float energy = resource.Key.EnergyProvide * digestNum * 0.5f;
                        group.Energy += energy;

                        goto default;
                    }
                case Resource.ResourceLevel.LEVEL_3:
                    {
                        //calulcate the energy get from the resources
                        digestNum = Mathf.Clamp(resource.Value / 4, 1, totalToDigest);
                        float energy = resource.Key.EnergyProvide * digestNum * 0.15f;
                        group.Energy += energy;

                        goto default;
                    }
                case Resource.ResourceLevel.LEVEL_4:
                    {
                        //calulcate the energy get from the resources
                        digestNum = Mathf.Clamp(resource.Value / 8, 1, totalToDigest);
                        float energy = resource.Key.EnergyProvide * digestNum * 0.05f;
                        group.Energy += energy;

                        goto default;
                    }
                case Resource.ResourceLevel.LEVEL_5:
                    {
                        //calulcate the energy get from the resources
                        digestNum = Mathf.Clamp(resource.Value / 16, 1, totalToDigest);
                        float energy = resource.Key.EnergyProvide * digestNum * 0.02f;
                        group.Energy += energy;

                        goto default;
                    }
                default:
                    {
                        Resource lowerLevel = ResourceList.GetInstance().GetResource(resource.Key.Category, resource.Key.Level - 1);
                        if (!group.ResourcesCarried.ContainsKey(lowerLevel))
                        {
                            group.ResourcesCarried.Add(lowerLevel, 0);
                        }

                        //return the digested product
                        group.ResourcesCarried[lowerLevel] += digestNum * 2;
                        group.ResourcesCarried[resource.Key] -= digestNum;

                        break;
                    }
            }
        }

        //return the resource to map
        resources = group.ResourcesCarried.OrderBy(i => i.Key.Level).ToList();
        foreach (var resource in resources)
        {
            if (!WorldMap.INSTANCE.MapTiles[group.MapPosition.x][group.MapPosition.y].ResourceList.ContainsKey(resource.Key))
            {
                WorldMap.INSTANCE.MapTiles[group.MapPosition.x][group.MapPosition.y].ResourceList.Add(resource.Key, 0);
            }

            WorldMap.INSTANCE.MapTiles[group.MapPosition.x][group.MapPosition.y].ResourceList[resource.Key] += resource.Value;
            group.ResourcesCarried[resource.Key] = 0;
        }
    }

    public override AbilityList.ABILITIES GetABILITIES()
    {
        return AbilityList.ABILITIES.DIBLASTIC;
    }
}
