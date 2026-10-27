using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    public bool canMove = false;
    public bool mapGenerated = false;

    public int stamina = 3;
    public int maxStamina = 3;
    public void Up(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (canMove)
            {
                Collider2D checker = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y + 1), new Vector2(0.4f, 0.4f), 0f);
                CollisionChecker(checker);
                if (checker != null && canMove == true)
                {
                    transform.position = transform.position + new Vector3(0, 1, 0);
                    stamina--;
                }
            }
        }
    }

    public void Down(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (canMove)
            {
                Collider2D checker = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - 1), new Vector2(0.4f, 0.4f), 0f);
                CollisionChecker(checker);
                if (checker != null && canMove == true)
                {
                    transform.position = transform.position + new Vector3(0, -1, 0);
                    stamina--;
                }
            }
        }
    }

    public void Left(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (canMove)
            {
                Collider2D checker = Physics2D.OverlapBox(new Vector2(transform.position.x - 1, transform.position.y), new Vector2(0.4f, 0.4f), 0f);
                CollisionChecker(checker);
                if (checker != null && canMove == true)
                {
                    transform.position = transform.position + new Vector3(-1, 0, 0);
                    stamina--;
                }
            }
        }
    }

    public void Right(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (canMove)
            {
                Collider2D checker = Physics2D.OverlapBox(new Vector2(transform.position.x + 1, transform.position.y), new Vector2(0.4f, 0.4f), 0f);
                CollisionChecker(checker);
                if (checker != null && canMove == true)
                {
                    transform.position = transform.position + new Vector3(1, 0, 0);
                    stamina--;
                }
            }
        }
    }

    public void SpaceBar(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (mapGenerated == true)
            {
                stamina = maxStamina;
                canMove = true;
            }
        }
    }

    private void CollisionChecker(Collider2D checker)
    {
        string tag = checker.gameObject.tag;
        Debug.Log(tag);
        if (tag == "water")
        {
            canMove = false;
        }
        else
        {
            canMove = true;
        }
    }

    private void Update()
    {
        if (stamina <= 0)
        {
            canMove = false;
        }
    }
}
