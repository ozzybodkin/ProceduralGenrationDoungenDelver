using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    public void Up(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Collider2D checker = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y + 1), new Vector2(0.4f,0.4f), 0f);
            if (checker != null)
            {
                transform.position = transform.position + new Vector3(0, 1, 0);
            }
        }
    }

    public void Down(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Collider2D checker = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - 1), new Vector2(0.4f, 0.4f), 0f);
            if (checker != null)
            {
                transform.position = transform.position + new Vector3(0, -1, 0);
            }
        }

    }

    public void Left(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Collider2D checker = Physics2D.OverlapBox(new Vector2(transform.position.x - 1, transform.position.y), new Vector2(0.4f, 0.4f), 0f);
            if (checker != null)
            {
                transform.position = transform.position + new Vector3(-1, 0, 0);
            }
        }
    }

    public void Right(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Collider2D checker = Physics2D.OverlapBox(new Vector2(transform.position.x + 1, transform.position.y), new Vector2(0.4f, 0.4f), 0f);
            if (checker != null)
            {
                transform.position = transform.position + new Vector3(1, 0, 0);
            }
        }
    }
}
