using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    public void Up(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            transform.position = transform.position + new Vector3(0, 1, 0);
        }
    }

    public void Down(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            transform.position = transform.position + new Vector3(0, -1, 0);
        }

    }

    public void Left(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            transform.position = transform.position + new Vector3(-1, 0, 0);
        }
    }

    public void Right(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            transform.position = transform.position + new Vector3(1, 0, 0);
        }
    }
}
