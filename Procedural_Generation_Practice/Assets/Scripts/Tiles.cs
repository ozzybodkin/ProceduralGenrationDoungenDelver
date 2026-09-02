using UnityEngine;

public struct Tiles
{
    public enum Type
    {
        Ground,
        Hazard,
        Merchant,
        Town,
        Doungeon,
        Water,
    }

    public Vector3Int position;
    public Type type;
    public bool reaveled;
}