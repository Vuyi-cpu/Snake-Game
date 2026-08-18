using UnityEngine;

public class SnakeBodySegment : MonoBehaviour
{
    public SnakeBody owner;

    private bool canCollide = false;


    // =========================================================
    // COLLISION CONTROL
    // =========================================================

    public void EnableCollision()
    {
        canCollide = true;
    }


    public void DisableCollision()
    {
        canCollide = false;
    }


    // =========================================================
    // TRIGGER
    // =========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canCollide)
            return;

        // Your existing collision code goes here.

        Debug.Log("Snake segment triggered: " + other.name);
    }
}