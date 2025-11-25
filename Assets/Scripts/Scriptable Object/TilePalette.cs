using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TilePalette", menuName = "Scriptable/Tile Palette")]
public class TilePalette : ScriptableObject
{
    public List<string> RegisterName;
    public List<TileGroup> RegisterTile;

    public TileGroup GetTile(string Name)
    {
        int index = RegisterName.FindIndex(0, RegisterName.Count, s => (s.CompareTo(Name) == 0));
        
        return RegisterTile[index];
    }
}
