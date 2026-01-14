using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Resource;

[CreateAssetMenu(fileName = "Resource List", menuName = "Scriptable/ResourceList")]
public class ResourceList : ScriptableObject
{
    private static ResourceList _INSTANCE;

    public static ResourceList GetInstance()
    {
        if(!_INSTANCE)
        {
            _INSTANCE = Resources.Load<ResourceList>("ScriptableObject/Resource/Resource List");
        }

        return _INSTANCE;
    }


    [SerializeField]
    private List<Resource> _List;

    public List<Resource> GetResources(Categories category)
    {
        return _List.GetRange((int)ResourceLevel.NUM_OF_LEVEL * (int)category, (int)ResourceLevel.NUM_OF_LEVEL).ToList();
    }

    public Resource GetResource(Categories category, ResourceLevel level)
    {
        return _List[((int)ResourceLevel.NUM_OF_LEVEL * (int)category) + (int)level];
    }
}
