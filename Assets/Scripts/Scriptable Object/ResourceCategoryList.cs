using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource Category List", menuName = "Scriptable/Resource Category List")]
public class ResourceCategoryList : ScriptableObject
{
    public List<string> RegisterName;
    public List<ResourceCategory> RegisterCategory;

    public ResourceCategory FindResourceCategory(string name)
    {
        int index = RegisterName.FindIndex(0, RegisterName.Count, (s => s.CompareTo(name) == 0));
        return RegisterCategory[index];
    }
}
