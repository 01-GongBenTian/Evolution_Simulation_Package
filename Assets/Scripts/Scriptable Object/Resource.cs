using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Reource", menuName = "Scriptable/Reource")]
public class Resource : ScriptableObject
{
    public enum ResourceLevel
    {
        LEVEL_1 = 0,
        LEVEL_2,
        LEVEL_3,
        LEVEL_4,
        LEVEL_5,
        NUM_OF_LEVEL
    }

    [SerializeReference]
    public Resource UpperLevel;

    [SerializeReference]
    public Resource LowerLevel;

    public ResourceCategory Category;
    public ResourceLevel Level;

    public int EnergyProvide;
    public int EnergyToBreak;
    public float TimeToUpgrade;
}
