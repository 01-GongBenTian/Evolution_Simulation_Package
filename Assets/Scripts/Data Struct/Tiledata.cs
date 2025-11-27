using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tiledata
{
    public TileGroup BaseTile;
    public TileGroup DetailTile;

    public float Altitude;

    public float Temperature;
    public float Humidity;

    public Dictionary<Resource, float> ResourceList;
}