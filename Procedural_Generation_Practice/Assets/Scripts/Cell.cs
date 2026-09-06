using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool collapsed;
    public Tile[] possibleTiles;

    public void CreateCell(bool collapseState, Tile[] tiles)
    {
        collapsed = collapseState;
        possibleTiles = tiles;
    }

    public void RecreateCell(Tile[] tiles)
    {
        possibleTiles = tiles;
    }
}
