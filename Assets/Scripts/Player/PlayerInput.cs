using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 GetMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Ensure only one direction at a time
        if (Mathf.Abs(moveX) > 0)
        {
            moveY = 0;
        }

        return new Vector2(moveX, moveY);
    }

    public bool GetInteractionInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
}