using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public struct Tiles
{
    public enum Type
    {
        Ground,
        Shore,
        Hazard,
        Merchant,
        Town,
        Doungeon,
        Water,
        Empty,
    }

    public Vector3Int position;
    public Type type;
    public bool reaveled;
    public int entropy;
}