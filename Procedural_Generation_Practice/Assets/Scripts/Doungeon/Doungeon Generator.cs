using UnityEngine;
using System.Linq;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class DoungeonGenerator : MonoBehaviour
{
    private Player_Controller pC;
    private GameObject activePlayer;
    private bool playerMoveState;

    public GameObject background;

    public GameObject[] horizontalStartingRooms;
    public GameObject[] horizontalCombatRooms;
    public GameObject[] horizontalKeyRooms;
    public GameObject[] horizontalExitRooms;
    public GameObject horizontalHallway;

    private DoungeonRoomFunctionality currentRoom;

    int entrance = 0;
    int exit = 0;
    public void CreateDoungeon()
    {
        activePlayer = GameObject.FindWithTag("player");
        pC = activePlayer.GetComponent<Player_Controller>();

        playerMoveState = pC.canMove;
        pC.canMove = false;

        Instantiate(background, new Vector3(0,0,0), Quaternion.identity);

        int doungeonVarient = Random.Range(0, 2);

        if (doungeonVarient == 1)
        {
            DoungeonVariationOne();
        }
        else
        {
            DoungeonVariationOne();
        }

        Debug.Log(doungeonVarient);
    }

    private void DoungeonVariationOne()
    {
        Vector3 targetLocation = new Vector3(0,0,0);
        GameObject spawnRoom = Instantiate(horizontalStartingRooms[Random.Range(0, horizontalStartingRooms.Length)], new Vector3(0,0,0), Quaternion.identity);
        GameObject combatRoom = Instantiate(horizontalCombatRooms[Random.Range(0, horizontalCombatRooms.Length)], new Vector3(0, 0, 0), Quaternion.identity);
        GameObject keyRoom = Instantiate(horizontalKeyRooms[Random.Range(0, horizontalKeyRooms.Length)], new Vector3(0, 0, 0), Quaternion.identity);
        GameObject exitRoom = Instantiate(horizontalExitRooms[Random.Range(0, horizontalExitRooms.Length)], new Vector3(0, 0, 0), Quaternion.identity);

        List<GameObject> doungeonRooms = new List<GameObject>();
        doungeonRooms.Add(spawnRoom);
        doungeonRooms.Add(combatRoom);
        doungeonRooms.Add(keyRoom);
        doungeonRooms.Add(exitRoom);

        for (int i = 0; i < doungeonRooms.Count-1; i++)
        {
            currentRoom = doungeonRooms[i].GetComponent<DoungeonRoomFunctionality>();

            while (currentRoom.roomDoors[exit].entrance != false)
            {
                if (exit++ > currentRoom.roomDoors.Length)
                {
                    
                }
                else
                {
                    exit++;
                }        
            }

            targetLocation = currentRoom.roomDoors[exit].door.transform.position;
            if (currentRoom.roomDoors[exit].horizontalDoor == true)
            {
                targetLocation = targetLocation + new Vector3(15 * currentRoom.roomDoors[exit].direction, 0, 0);
            }
            else
            {
                targetLocation = targetLocation + new Vector3(0, 15 * currentRoom.roomDoors[exit].direction, 0);
            }

            Instantiate(horizontalHallway, targetLocation - new Vector3(7.5f,0,0), Quaternion.identity);

            currentRoom = doungeonRooms[i+1].GetComponent<DoungeonRoomFunctionality>();

            while (currentRoom.roomDoors[entrance].entrance != true)
            {
                entrance++;
            }

            float differnace = currentRoom.roomDoors[entrance].room.transform.position.x - currentRoom.roomDoors[entrance].door.transform.position.x;
            currentRoom.transform.position = targetLocation + new Vector3(differnace, 0, 0);
            Debug.Log("test1");
        }
    }

    private void DoungeonVariationTwo() 
    { 

    }
}
