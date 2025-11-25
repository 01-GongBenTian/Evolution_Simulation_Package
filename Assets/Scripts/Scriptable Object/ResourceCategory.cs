using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Reource Category", menuName = "Scriptable/ReourceCategory")]
public class ResourceCategory : ScriptableObject
{
    public string Name;

    public float Level_1;
    public float Level_2;
    public float Level_3;
    public float Level_4;
    public float Level_5;
}
