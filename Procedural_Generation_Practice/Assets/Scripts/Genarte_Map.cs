using System.Collections.Generic;
using UnityEngine;

public class Genarte_Map : MonoBehaviour
{
    private Tiles[,] map;

    public int numTiles;

    public int maxHeight;
    public int minHeight;
    public int maxWidth;
    public int minWidth;

    public List<Tiles.Type> validGroundAdjacent = new List<Tiles.Type>();
    public List<Tiles.Type> validShoreAdjacnet = new List<Tiles.Type>();
    public List<Tiles.Type> validHazardAdjacnet = new List<Tiles.Type>();
    public List<Tiles.Type> validMerchantAdjacnet = new List<Tiles.Type>();
    public List<Tiles.Type> validTownAdjacnet = new List<Tiles.Type>();
    public List<Tiles.Type> validDoungeonAdjacnet = new List<Tiles.Type>();
    public List<Tiles.Type> validWaterAdjacnet = new List<Tiles.Type>();
    public List<Tiles.Type> validEmptyAdjacnet = new List<Tiles.Type>();

    private void GenerateMap()
    {
        map = new Tiles[maxWidth, maxHeight];

        for (int x = maxWidth; x < maxWidth; x++)
        {
            for (int y = minHeight; y < maxHeight; y++)
            {
                Tiles tile = new Tiles();
                tile.position = new Vector3Int(x, y, 0);
                tile.type = Tiles.Type.Empty;
                map[x,y] = tile;
            }
        }

        map[0,0].type = Tiles.Type.Ground;

        CheckAdjacentTiles(map[0,0]);
    }

    private void CheckAdjacentTiles(Tiles tile)
    {
        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacnetY = -1; adjacnetY <= 1; adjacnetY++)
            {
                Tiles adTile = map[adjacentX, adjacnetY];

                if (adjacentX != 0 && adjacnetY != 0)
                {
                    if (tile.type != Tiles.Type.Empty)
                    {
                        SetEntropy(tile, map[adjacentX, adjacnetY]);
                    }
                }
            }
        }
    }

    private void SetEntropy(Tiles adjacentTile, Tiles tile)
    {
        switch (tile.type)
        {
            case Tiles.Type.Ground:
                adjacentTile.entropy = validGroundAdjacent.Count;
                break;

            case Tiles.Type.Shore:
                adjacentTile.entropy = validShoreAdjacnet.Count;
                break;

            case Tiles.Type.Hazard:
                break;

            case Tiles.Type.Merchant:
                adjacentTile.entropy = validMerchantAdjacnet.Count;
                break;

            case Tiles.Type.Town:
                adjacentTile.entropy = validTownAdjacnet.Count;
                break;

            case Tiles.Type.Doungeon:
                adjacentTile.entropy = validDoungeonAdjacnet.Count;
                break;

            case Tiles.Type.Water:
                adjacentTile.entropy = validWaterAdjacnet.Count;
                break;

            case Tiles.Type.Empty:
                adjacentTile.entropy = validEmptyAdjacnet.Count;
                break;
        }
    }
}
