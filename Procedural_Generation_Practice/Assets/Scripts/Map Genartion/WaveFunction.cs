using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

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

        maxNumTown = Mathf.RoundToInt(dimensionsF * 0.05f);

        maxNumGround = Mathf.RoundToInt(dimensionsF * 0.8f);

        maxNumWater = Mathf.RoundToInt(dimensionsF * 0.1f);

        maxNumMarket = Mathf.RoundToInt(dimensionsF * 0.05f);

        int numTiles = maxNumTown + maxNumGround + maxNumMarket + maxNumWater;
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

    private void CollapseCell(List<Cell> tempGrid)
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        Cell cellToCollapse = tempGrid[randIndex];
        cellToCollapse.collapsed = true;

        int randSelectedTile = UnityEngine.Random.Range(0, cellToCollapse.possibleTiles.Length);

        if (randSelectedTile > cellToCollapse.possibleTiles.Length)
        {
            CollapseCell(tempGrid);
        }

        Tile selectedTile = cellToCollapse.possibleTiles[UnityEngine.Random.Range(0, randSelectedTile)];

        cellToCollapse.possibleTiles = new Tile[] { selectedTile };

        Tile foundTile = cellToCollapse.possibleTiles[0];
        Instantiate(foundTile, cellToCollapse.transform.position, Quaternion.identity);

        if (foundTile.tag == "town")
        {
            numTown++;
            if (numTown == maxNumTown)
            {
                townTilesFull = true;
                Debug.Log("Town Full");
            }
        }
        else if (foundTile.tag == "ground")
        {
            numGround++;
            if (numGround == maxNumGround)
            {
                groundTilesFull = true;
                Debug.Log("Ground Full");
            }
        }
        else if (foundTile.tag == "market")
        {
            numMarket++;
            if (numMarket == maxNumMarket)
            {
                marketTilesFull = true;
                Debug.Log("Market Full");
            }
        }
        else if (foundTile.tag == "water")
        {
            numWater++;
            if (numWater == maxNumWater)
            {
                waterTilesFull = true;
                Debug.Log("Water Full");
            }
        }

            UpdateGeneartion();
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

                    if (townTilesFull)
                    {
                        for (int i = 0; i < options.Count; i++)
                        {
                            if (options[i].tag == "town")
                            {
                                options.Remove(options[i]);
                            }
                        }
                    }

                    if (groundTilesFull)
                    {
                        for (int i = 0; i < options.Count; i++)
                        {
                            if (options[i].tag == "ground")
                            {
                                options.Remove(options[i]);
                            }
                        }
                    }

                    if (waterTilesFull)
                    {
                        for (int i = 0; i < options.Count; i++)
                        {
                            if (options[i].tag == "water")
                            {
                                options.Remove(options[i]);
                            }
                        }
                    }

                    if (marketTilesFull)
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
