using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Reource Category", menuName = "Scriptable/ReourceCategory")]
public class ResourceCategory : ScriptableObject
{
    public string Name;

    public Resource[] List;
}
