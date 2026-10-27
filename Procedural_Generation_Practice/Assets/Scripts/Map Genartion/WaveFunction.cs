using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

public struct TilePercent
{
    public int numTiles;
    public int maxNumTiles;
    public bool tileTypeFull;
}

public class WaveFunction : MonoBehaviour
{
    public int dimensions;
    public Tile[] tileObjects;
    public List<Cell> gridComponents;
    public Cell cellObj;

    public GameObject player;
    private GameObject activePlayer;
    private Player_Controller playerController;

    private int currentScene;

    private int iterations = 0;

    private TilePercent town;
    private TilePercent ground;
    private TilePercent water;
    private TilePercent market;

    private int numTown = 0;
    private int numGround = 0;
    private int numMarket = 0;
    private int numWater = 0;

    private int maxNumTown;
    private int maxNumGround;
    private int maxNumWater;
    private int maxNumMarket;

    private bool townTilesFull = false;
    private bool groundTilesFull = false;
    private bool waterTilesFull = false;
    private bool marketTilesFull = false;
    private List<TilePercent> fullTileTypes = new List<TilePercent>();
    private List<TilePercent> notFullTileTypes = new List<TilePercent>();

    private int numCells0Entropy = 0;
    private bool slowDown = false;

    private void Awake()
    {
        gridComponents = new List<Cell>();
        InitilizeGrid();
    }

    private void Start()
    {
        activePlayer = Instantiate(player, new Vector3(0, dimensions-1, 0), Quaternion.identity);
        playerController = activePlayer.GetComponent<Player_Controller>();

        float dimensionsF = (float)dimensions * dimensions;

        //maxNumTown = Mathf.RoundToInt(dimensionsF * 0.05f);

        //maxNumGround = Mathf.RoundToInt(dimensionsF * 0.8f);

        //maxNumWater = Mathf.RoundToInt(dimensionsF * 0.1f);

        //maxNumMarket = Mathf.RoundToInt(dimensionsF * 0.05f);

        town.maxNumTiles = Mathf.RoundToInt(dimensionsF * 0.05f);

        ground.maxNumTiles = Mathf.RoundToInt(dimensionsF * 0.83f);

        water.maxNumTiles = Mathf.RoundToInt(dimensionsF * 0.1f);

        market.maxNumTiles = Mathf.RoundToInt(dimensionsF * 0.02f);

        notFullTileTypes.Add(town);
        notFullTileTypes.Add(ground);
        notFullTileTypes.Add(water);
        notFullTileTypes.Add(market);

        town.numTiles = 0;
        ground.numTiles = 0;
        water.numTiles = 0;
        market.numTiles = 0;

        int numTiles = town.maxNumTiles + ground.maxNumTiles + market.maxNumTiles + water.maxNumTiles;
        //int numTiles = maxNumTown + maxNumGround + maxNumMarket + maxNumWater;
        int area = dimensions * dimensions;

        if (numTiles > area)
        {
            int differance = numTiles - area;
            maxNumGround -= differance;
        }
        else if (numTiles < area)
        {
            int differance = area - numTiles;
            maxNumGround += differance;
        }
    }

    public void ResetBoard(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            currentScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentScene);

            for (int i = 0; i < tileObjects.Length; i++)
            {
                tileObjects[i].ClearArrays();
            }
        }
    }

    private void InitilizeGrid()
    {
        for (int i = 0; i < tileObjects.Length; i++)
        {
            tileObjects[i].ClearArrays();
        }

        Camera.main.transform.position = new Vector3(dimensions/2, dimensions/2, -10);
        Camera.main.orthographicSize = dimensions/2 + 1;

        for (int i = 0; i < tileObjects.Length; i++)
        {
            tileObjects[i].SetNeighbours();
        }

        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                Cell newCell = Instantiate(cellObj, new Vector2(x,y), Quaternion.identity);
                newCell.CreateCell(false, tileObjects);
                gridComponents.Add(newCell);
            }
        }

        StartCoroutine(CheckEntropy());
    }

    private IEnumerator CheckEntropy()
    {
        List<Cell> tempGrid = new List<Cell>(gridComponents);

        tempGrid.RemoveAll(c => c.collapsed);

        tempGrid.Sort((a, b) => { return a.possibleTiles.Length - b.possibleTiles.Length; });

        int arrLength = tempGrid[0].possibleTiles.Length;
        int stopIndex = default;

        for (int i = 1; i < tempGrid.Count; i++)
        {
            if (tempGrid[i].possibleTiles.Length > arrLength)
            {
                stopIndex = i;
                break;
            }
        }

        if (stopIndex > 0)
        {
            tempGrid.RemoveRange(stopIndex, tempGrid.Count - stopIndex);
        }

        yield return new WaitForSeconds(0.01f);

        CollapseCell(tempGrid);
    }

    private void AdjustPercentage()
    {
        switch (fullTileTypes.Count)
        {
            case 0:
                Debug.Log("Case0");
                int randomeIndex = UnityEngine.Random.Range(0, notFullTileTypes.Count);
                Debug.Log(randomeIndex);
                TilePercent full = notFullTileTypes[randomeIndex];
                full.maxNumTiles++;
                notFullTileTypes[randomeIndex] = full;

                int randomIndexTwo = UnityEngine.Random.Range(0, notFullTileTypes.Count);
                TilePercent notFull = notFullTileTypes[randomIndexTwo];
                notFull.maxNumTiles--;
                notFullTileTypes[randomIndexTwo] = notFull;

                if (notFull.maxNumTiles == notFull.numTiles)
                {
                    notFull.tileTypeFull = true;
                    fullTileTypes.Add(notFullTileTypes[randomIndexTwo]);
                    notFullTileTypes.Remove(notFullTileTypes[randomIndexTwo]);
                }
                
                break;
                
            case 1:
                Debug.Log("Case1");
                TilePercent full1 = fullTileTypes[0];
                full1.tileTypeFull = false;
                full1.maxNumTiles++;
                fullTileTypes[0] = full1;

                int randomIndexFull1 = UnityEngine.Random.Range(0, notFullTileTypes.Count);
                Debug.Log(randomIndexFull1);
                TilePercent notFull1 = notFullTileTypes[randomIndexFull1];
                notFull1.maxNumTiles--;
                notFullTileTypes[randomIndexFull1] = notFull1;

                if (notFull1.maxNumTiles == notFull1.numTiles)
                {
                    notFull1.tileTypeFull = true;
                    fullTileTypes.Add(notFullTileTypes[randomIndexFull1]);
                }

                notFullTileTypes.Remove(notFullTileTypes[randomIndexFull1]);

                notFullTileTypes.Add(fullTileTypes[0]);
                fullTileTypes.Remove(fullTileTypes[0]);
                break;

            case 2:
                Debug.Log("Case2");
                int randomeIndex2 = UnityEngine.Random.Range(0, fullTileTypes.Count);
                TilePercent full2 = fullTileTypes[randomeIndex2];
                full2.tileTypeFull = false;
                full2.maxNumTiles++;
                fullTileTypes[randomeIndex2] = full2;

                int randomIndexFull2 = UnityEngine.Random.Range(0, notFullTileTypes.Count);
                TilePercent notFull2 = notFullTileTypes[randomIndexFull2];
                notFull2.maxNumTiles--;
                notFullTileTypes[randomIndexFull2] = notFull2;

                notFullTileTypes.Remove(notFullTileTypes[randomIndexFull2]);

                if (notFull2.maxNumTiles == notFull2.numTiles)
                {
                    notFull2.tileTypeFull = true;
                    fullTileTypes.Add(notFullTileTypes[randomIndexFull2]);
                }

                notFullTileTypes.Add(fullTileTypes[randomeIndex2]);
                fullTileTypes.Remove(fullTileTypes[randomeIndex2]);
                break;

            case 3:
                Debug.Log("Case3");
                int randomeIndex3 = UnityEngine.Random.Range(0, fullTileTypes.Count);
                TilePercent full3 = fullTileTypes[randomeIndex3];
                full3.tileTypeFull = false;
                full3.maxNumTiles++;
                fullTileTypes[randomeIndex3] = full3;
                fullTileTypes.Remove(fullTileTypes[randomeIndex3]);

                TilePercent notFull3 = notFullTileTypes[0];
                notFull3.maxNumTiles--;
                notFullTileTypes[0] = notFull3;

                notFullTileTypes.Remove(notFullTileTypes[0]);

                if (notFull3.maxNumTiles == notFull3.numTiles)
                {
                    notFull3.tileTypeFull = true;
                    fullTileTypes.Add(notFullTileTypes[0]);
                }

                notFullTileTypes.Add(fullTileTypes[randomeIndex3]);
                break;

            case 4:
                Debug.Log("Case 4");
                int randomeIndex4 = UnityEngine.Random.Range(0, fullTileTypes.Count);
                TilePercent full4 = fullTileTypes[randomeIndex4];
                full4.tileTypeFull = false;
                full4.maxNumTiles++;
                fullTileTypes[randomeIndex4] = full4;
                notFullTileTypes.Add(fullTileTypes[randomeIndex4]);
                fullTileTypes.Remove(fullTileTypes[randomeIndex4]);
                break;
        }
        slowDown = true;
        StartCoroutine(CheckEntropy());
        return;
    }

    private void SlowDown(List<Cell> tempGrid)
    {
        CollapseCell(tempGrid);
    }

    private void CollapseCell(List<Cell> tempGrid)
    {
        List<Cell> unCollapsedCells = new(tempGrid);
        unCollapsedCells.RemoveAll(c => c.collapsed);

        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        Cell cellToCollapse = tempGrid[randIndex];
        cellToCollapse.collapsed = true;

        int randSelectedTile = UnityEngine.Random.Range(0, cellToCollapse.possibleTiles.Length);

        if (randSelectedTile > cellToCollapse.possibleTiles.Length)
        {
            StartCoroutine(CheckEntropy());
            return;
        }
        else if (cellToCollapse.possibleTiles.Length == 0)
        {
            numCells0Entropy++;
            if (numCells0Entropy == unCollapsedCells.Count)
            {
                unCollapsedCells = new List<Cell>();
                AdjustPercentage();
                return;
            }
            else
            {
                StartCoroutine(CheckEntropy());
                return;
            }
        }

        Tile selectedTile = cellToCollapse.possibleTiles[randSelectedTile];

        cellToCollapse.possibleTiles = new Tile[] { selectedTile };

        Tile foundTile = cellToCollapse.possibleTiles[0];
        Instantiate(foundTile, cellToCollapse.transform.position, Quaternion.identity);

        if (foundTile.tag == "town")
        {
            //numTown++;
            town.numTiles++;
            if (town.numTiles == town.maxNumTiles)
            {
                //townTilesFull = true;
                town.tileTypeFull = true;
                fullTileTypes.Add(town);
                notFullTileTypes.Remove(town);
                Debug.Log("Town Full");
            }
        }
        else if (foundTile.tag == "ground")
        {
            //numGround++;
            ground.numTiles++;
            if (ground.numTiles == ground.maxNumTiles)
            {
                //groundTilesFull = true;
                ground.tileTypeFull = true;
                fullTileTypes.Add(ground);
                notFullTileTypes.Remove(ground);
                Debug.Log("Ground Full");
            }
        }
        else if (foundTile.tag == "market")
        {
            //numMarket++;
            market.numTiles++;
            if (market.numTiles == market.maxNumTiles)
            {
                //marketTilesFull = true;
                market.tileTypeFull = true;
                fullTileTypes.Add(market);
                notFullTileTypes.Remove(market);
                Debug.Log("Market Full");
            }
        }
        else if (foundTile.tag == "water")
        {
            numWater++;
            water.numTiles++;
            if (water.numTiles == water.maxNumTiles)
            {
                waterTilesFull = true;
                water.tileTypeFull = true;
                fullTileTypes.Add(water);
                notFullTileTypes.Remove(water);
                Debug.Log("Water Full");
            }
        }

            UpdateGeneartion();
        return;
    }

    private void UpdateGeneartion()
    {
        List<Cell> newGeneartionCell = new List<Cell>(gridComponents);

        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                var index = x + y * dimensions;
                if (gridComponents[index].collapsed)
                {
                    Debug.Log("called");
                    newGeneartionCell[index] = gridComponents[index];
                }
                else
                {
                    List<Tile> options = new List<Tile>();
                    foreach (Tile t in tileObjects)
                    {
                        options.Add(t);
                    }

                    if (y > 0)
                    {
                        Cell up = gridComponents[x + (y - 1) * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in up.possibleTiles)
                        {
                            var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[validOption].upNeighbours;
                            
                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    if (x < dimensions - 1)
                    {
                        Cell right = gridComponents[x + 1 + y * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in right.possibleTiles)
                        {
                            var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[validOption].leftNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    if (y < dimensions - 1)
                    {
                        Cell down = gridComponents[x + (y + 1) * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in down.possibleTiles)
                        {
                            var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[validOption].downNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    if (x > 0)
                    {
                        Cell left = gridComponents[x - 1 + y * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in left.possibleTiles)
                        {
                            var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[validOption].rightNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    if (town.tileTypeFull)
                    {
                        for (int i = 0; i < options.Count; i++)
                        {
                            if (options[i].tag == "town")
                            {
                                options.Remove(options[i]);
                            }
                        }
                    }

                    if (ground.tileTypeFull)
                    {
                        for (int i = 0; i < options.Count; i++)
                        {
                            if (options[i].tag == "ground")
                            {
                                options.Remove(options[i]);
                            }
                        }
                    }

                    if (water.tileTypeFull)
                    {
                        for (int i = 0; i < options.Count; i++)
                        {
                            if (options[i].tag == "water")
                            {
                                options.Remove(options[i]);
                            }
                        }
                    }

                    if (market.tileTypeFull)
                    {
                        for (int i = 0; i < options.Count; i++)
                        {
                            if (options[i].tag == "market")
                            {
                                options.Remove(options[i]);
                            }
                        }
                    }

                    Tile[] newTileList = new Tile[options.Count];

                    for (int i = 0; i < options.Count; i++)
                    {
                        newTileList[i] = options[i];
                    }

                    newGeneartionCell[index].RecreateCell(newTileList);

                }
            }
        }

        gridComponents = newGeneartionCell;
        iterations++;

        if (iterations < dimensions * dimensions)
        {
            StartCoroutine(CheckEntropy());
        }
        else
        {
            playerController.mapGenerated = true;
        }
    }

    private void CheckValidity(List<Tile> optionList, List<Tile> validOption)
    {
        for (int x = optionList.Count - 1; x >= 0; x--)
        {
            var element = optionList[x];
            if (!validOption.Contains(element))
            {
                optionList.RemoveAt(x);
            }
        }
    }
}
