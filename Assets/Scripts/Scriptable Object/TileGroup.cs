using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Tile Group", menuName = "Scriptable/Tile Group")]
public class TileGroup : ScriptableObject
{
    public enum TileDir
    {
        SINGLE = 0,
        CENTER,
        TOP,
        INNER_TOP_RIGHT,
        OUTER_TOP_RIGHT,
        RIGHT,
        INNER_BOTTOM_RIGHT,
        OUTER_BOTTOM_RIGHT,
        BOTTOM,
        INNER_BOTTOM_LEFT,
        OUTER_BOTTOM_LEFT,
        LEFT,
        INNER_TOP_LEFT,
        OUTER_TOP_LEFT,
        NUM_OF_DIR
    }

    [SerializeField] private GameObject Tileset;
    [SerializeField] private TileCombination _Single;

    [SerializeField] private TileCombination _Center;

    [SerializeField] private TileCombination _Top;
    [SerializeField] private TileCombination _InnerTopRight;
    [SerializeField] private TileCombination _OuterTopRight;
    [SerializeField] private TileCombination _Right;
    [SerializeField] private TileCombination _InnerBottomRight;
    [SerializeField] private TileCombination _OuterBottomRight;
    [SerializeField] private TileCombination _Bottom;
    [SerializeField] private TileCombination _InnerBottomLeft;
    [SerializeField] private TileCombination _OuterBottomLeft;
    [SerializeField] private TileCombination _Left;
    [SerializeField] private TileCombination _InnerTopLeft;
    [SerializeField] private TileCombination _OuterTopLeft;

    public TileBase GetTile(TileDir dir, int index)
    {
        switch (dir)
        {
            case TileDir.CENTER:
                {
                    return Tileset.GetComponentInChildren<Tilemap>().GetTile(_Center.GetTileBase(index));
                }
            case TileDir.TOP:
                {
                    return Tileset.GetComponentInChildren<Tilemap>().GetTile(_Top.GetTileBase(index));
                }
            case TileDir.INNER_TOP_RIGHT:
                {
                    return Tileset.GetComponentInChildren<Tilemap>().GetTile(_InnerTopRight.GetTileBase(index));
                }
            case TileDir.OUTER_TOP_RIGHT:
                {
                    return Tileset.GetComponentInChildren<Tilemap>().GetTile(_OuterTopRight.GetTileBase(index));
                }
            case TileDir.RIGHT:
                {
                    return Tileset.GetComponentInChildren<Tilemap>().GetTile(_Right.GetTileBase(index));
                }
            case TileDir.INNER_BOTTOM_RIGHT:
                {
                    return Tileset.GetComponentInChildren<Tilemap>().GetTile(_InnerBottomRight.GetTileBase(index));
                }
            case TileDir.OUTER_BOTTOM_RIGHT:
                {
                    return Tileset.GetComponentInChildren<Tilemap>().GetTile(_OuterBottomRight.GetTileBase(index));
                }
            case TileDir.BOTTOM:
                {
                    return Tileset.GetComponentInChildren<Tilemap>().GetTile(_Bottom.GetTileBase(index));
                }
            case TileDir.INNER_BOTTOM_LEFT:
                {
                    return Tileset.GetComponentInChildren<Tilemap>().GetTile(_InnerBottomLeft.GetTileBase(index));
                }
            case TileDir.OUTER_BOTTOM_LEFT:
                {
                    return Tileset.GetComponentInChildren<Tilemap>().GetTile(_OuterBottomLeft.GetTileBase(index));
                }
            case TileDir.LEFT:
                {
                    return Tileset.GetComponentInChildren<Tilemap>().GetTile(_Left.GetTileBase(index));
                }
            case TileDir.INNER_TOP_LEFT:
                {
                    return Tileset.GetComponentInChildren<Tilemap>().GetTile(_InnerTopLeft.GetTileBase(index));
                }
            case TileDir.OUTER_TOP_LEFT:
                {
                    return Tileset.GetComponentInChildren<Tilemap>().GetTile(_OuterTopLeft.GetTileBase(index));
                }
            default:
                {
                    return Tileset.GetComponentInChildren<Tilemap>().GetTile(_Single.GetTileBase(index));
                }
        }
    }
}
