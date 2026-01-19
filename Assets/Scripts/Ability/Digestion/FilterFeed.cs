using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Resource;


[CreateAssetMenu(fileName = "Filter Feed", menuName = "Scriptable/Ability/Digestion/Filter Feed")]
public class FilterFeed : Digestion
{
    public override void Execute(object param0, object param1, object param2)
    {
        CreatureGroup group = (CreatureGroup)param0;
        FilterFeedCountDown countDown = FilterFeedCountDown.List.Find(i => (i.Group == group));

        //cannot find count down record
        if (countDown == null || countDown.CountDown == 2)
        {
            ConsumeResource(group);

            if (countDown == null)
            {
                FilterFeedCountDown.List.Add(new FilterFeedCountDown(group));
            }
            else
            {
                countDown.CountDown = 1;
            }
        }
        else if (countDown.CountDown == 1)//digestion
        {
            DigestResource(group);

            countDown.CountDown = 0;
        }
        else if (countDown.CountDown == 0)
        {
            DropResource(group);

            countDown.CountDown = 2;
        }
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
        foreach (KeyValuePair<Resource, int> resource in resources)
        {
            if (totalToConsume == 0)
                break;


            if (resource.Key.Level == ResourceLevel.LEVEL_1 || resource.Value == 0)
                continue;


            consumeAmount = Mathf.Clamp(resource.Value, 1, totalToConsume);
            if (!group.ResourcesCarried.ContainsKey(resource.Key))
            {
                group.ResourcesCarried.Add(resource.Key, consumeAmount);
            }
            else
            {
                group.ResourcesCarried[resource.Key] += consumeAmount;
            }


            WorldMap.INSTANCE.MapTiles[group.MapPosition.x][group.MapPosition.y].ResourceList[resource.Key] -= consumeAmount;
            totalToConsume -= consumeAmount;
        }
    }

    private void DigestResource(CreatureGroup group)
    {
        int totalToDigest = 0;
        foreach (var kvp in group.Creatures)
        {
            totalToDigest += kvp.Key.ResourceCarryNum * kvp.Value;
        }

        int digestNum = 0;
        List<KeyValuePair<Resource, int>> resources = group.ResourcesCarried.OrderBy(i => i.Key.Level).ToList();
        foreach (KeyValuePair<Resource, int> resource in resources)
        {
            if (totalToDigest == 0)
                break;

            if (resource.Value == 0)
                continue;

            switch (resource.Key.Level)
            {
                case ResourceLevel.LEVEL_2:
                    {
                        digestNum = Mathf.Clamp(resource.Value, 1, totalToDigest);

                        //calulcate the energy get from the resources
                        int energy = (int)(resource.Key.EnergyProvide * digestNum * 0.5f);
                        group.Energy += energy;


                        goto default;
                    }
                case ResourceLevel.LEVEL_3:
                    {
                        //calulcate the energy get from the resources
                        digestNum = Mathf.Clamp(resource.Value / 4, 1, totalToDigest);
                        int energy = (int)(resource.Key.EnergyProvide * digestNum * 0.15f);
                        group.Energy += energy;


                        goto default;
                    }
                case ResourceLevel.LEVEL_4:
                    {
                        //calulcate the energy get from the resources
                        digestNum = Mathf.Clamp(resource.Value / 8, 1, totalToDigest);
                        int energy = (int)(resource.Key.EnergyProvide * digestNum * 0.05f);
                        group.Energy += energy;


                        goto default;
                    }
                case ResourceLevel.LEVEL_5:
                    {
                        //calulcate the energy get from the resources
                        digestNum = Mathf.Clamp(resource.Value / 16, 1, totalToDigest);
                        int energy = (int)(resource.Key.EnergyProvide * digestNum * 0.2f);
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
    }

    private void DropResource(CreatureGroup group)
    {
        List<KeyValuePair<Resource, int>> resources = group.ResourcesCarried.OrderBy(i => i.Key.Level).ToList();
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

    public override float GetEnergyWeight()
    {
        return 1.0f;
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
}
