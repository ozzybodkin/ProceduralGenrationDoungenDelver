using System.Linq;
using UnityEngine;

public class DoungeonRoomFunctionality : MonoBehaviour
{

    [System.Serializable]
    public struct Doors
    { 
        public int direction;
        public GameObject door;
        public bool horizontalDoor;
        public bool entrance;
        public GameObject room;
    }

    public Doors[] roomDoors;
    public int numExits;
}
