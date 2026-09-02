using UnityEngine;

public class Genarte_Map : MonoBehaviour
{
    private Tiles[,] map;

    public int numTiles;

    public int maxHeight;
    public int minHeight;
    public int maxWidth;
    public int minWidth;

    private void GenerateMap()
    {
        map = new Tiles[maxWidth, maxHeight];
        map[0, 0].type = Tiles.Type.Ground;

        PickTile(Random.Range(minWidth,maxWidth),Random.Range(minHeight, maxHeight));
    }

    private void PickTile(int xPos, int yPos)
    {

    }
}
