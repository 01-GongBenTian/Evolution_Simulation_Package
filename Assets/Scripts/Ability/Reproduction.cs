using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reproduction : Ability
{

}

public class MonoReproduction : Reproduction
{
    public MonoReproduction()
    {
        Type = AbilityType.REPRODUCTION;
    }

    /// <param name="param0">Creature List in group</param>
    /// <param name="param1">Key Value Pair of the creature reproduce</param>
    public override void Execute(object param0, object param1, object param2)
    {
        CreatureGroup group = (CreatureGroup)param0;
        CreatureData creature = (CreatureData)param1;
        float[] energyShared = (float[])param2;

        int reproduceNum = (int)(energyShared[0] / creature.ReproduceEnergyRequired);
        if(reproduceNum == 0)
        {
            return;
        }

        energyShared[0] -= creature.ReproduceEnergyRequired * reproduceNum;
        reproduceNum = (int)Mathf.Pow(reproduceNum, CreatureManager.INSTANCE.ReproducePower);

        float evolute = Random.Range(0.0f, 1.0f);
        if(evolute > CreatureManager.INSTANCE.EvoluteChance)
        {
            //no evolute happen
            group.Creatures[creature] += reproduceNum;
        }
        else
        {
            //evolute happen
            //evoluted reproduce
            CreatureData newCreature = creature.Evolute();
            int newCreaturePopulation = (int)Mathf.Floor(reproduceNum * 0.1f);

            //add to group
            group.AddInCreature(newCreature, newCreaturePopulation);

            //unevoluted reproduce
            group.Creatures[creature] += (reproduceNum - newCreaturePopulation);
        }
    }
}
