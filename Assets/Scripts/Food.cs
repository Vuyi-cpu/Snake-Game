using UnityEngine;

public class Food : MonoBehaviour
{
    public Vector2Int GridPosition { get; private set; }

    private FoodSpawner spawner;

    public void Setup(Vector2Int position, FoodSpawner foodSpawner)
    {
        GridPosition = position;
        spawner = foodSpawner;
    }

    void Update()
    {
        if (spawner.player1 != null &&
            spawner.player1.GridPosition == GridPosition)
        {
            spawner.FoodEaten(spawner.player1);
            return;
        }

        if (spawner.player2 != null &&
            spawner.player2.GridPosition == GridPosition)
        {
            spawner.FoodEaten(spawner.player2);
            return;
        }
    }
}