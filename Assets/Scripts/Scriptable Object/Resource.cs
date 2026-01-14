using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Reource", menuName = "Scriptable/Reource")]
public class Resource : ScriptableObject
{
    public enum Categories : byte
    {
        MINERAL = 0,
        NUM_OF_CATEGORIES
    }
    
    public enum ResourceLevel : byte
    {
        LEVEL_1 = 0,
        LEVEL_2,
        LEVEL_3,
        LEVEL_4,
        LEVEL_5,
        NUM_OF_LEVEL
    }

    public Categories Category;
    public ResourceLevel Level;

    public int EnergyProvide;
}
