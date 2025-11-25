using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileCombination", menuName = "Scriptable/Tile Combination")]
public class TileCombination : ScriptableObject
{
    [SerializeField] private Vector3Int BottomLeft;
    [SerializeField] private Vector3Int BottomRight;
    [SerializeField] private Vector3Int TopLeft;
    [SerializeField] private Vector3Int TopRight;

    public Vector3Int GetTileBase(int index)
    {
        switch (index)
        {
            case 0:
                {
                    return BottomLeft;
                }
            case 1:
                {
                    return BottomRight;
                }
            case 2:
                {
                    return TopLeft;
                }
            default:
                {
                    return TopRight;
                }
        }
    }
}
