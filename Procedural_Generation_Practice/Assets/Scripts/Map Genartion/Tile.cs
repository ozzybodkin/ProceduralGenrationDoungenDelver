using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Tile : MonoBehaviour
{

    public Tile[] upNeighbours;
    public Tile[] downNeighbours;
    public Tile[] leftNeighbours;
    public Tile[] rightNeighbours;

    public int uSocket;
    public int dSocket;
    public int lSocket;
    public int rSocket;

    private WaveFunction waveFunction;
    private GameObject mapGen;

    public void SetNeighbours()
    {
        mapGen = GameObject.FindWithTag("mapgen");
        waveFunction = mapGen.GetComponent<WaveFunction>();

        List<Tile> upNeighboursList = upNeighbours.ToList();
        List<Tile> downNeighboursList = downNeighbours.ToList();
        List<Tile> leftNeighboursList = leftNeighbours.ToList();
        List<Tile> rightNeighbourList = rightNeighbours.ToList();

        for (int i = 0; i < waveFunction.tileObjects.Length; i++)
        {
            if (uSocket == waveFunction.tileObjects[i].dSocket)
            {
                upNeighboursList.Add(waveFunction.tileObjects[i]);
            }

            if (dSocket == waveFunction.tileObjects[i].uSocket)
            {
                downNeighboursList.Add(waveFunction.tileObjects[i]);
            }

            if (lSocket == waveFunction.tileObjects[i].rSocket)
            {
                leftNeighboursList.Add(waveFunction.tileObjects[i]);
            }

            if (rSocket == waveFunction.tileObjects[i].lSocket)
            {   
                rightNeighbourList.Add(waveFunction.tileObjects[i]);
            }
        }

        upNeighbours = upNeighboursList.ToArray();
        downNeighbours = downNeighboursList.ToArray();
        leftNeighbours = leftNeighboursList.ToArray();
        rightNeighbours = rightNeighbourList.ToArray();
    }

    public void ClearArrays()
    {
        upNeighbours = Array.Empty<Tile>();
        downNeighbours= Array.Empty<Tile>();
        leftNeighbours= Array.Empty<Tile>();
        rightNeighbours= Array.Empty<Tile>();
    }
}
