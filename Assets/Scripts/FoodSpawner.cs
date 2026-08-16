using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject foodPrefab;
    public Grid tilemapGrid;

    public SnakeMovement player1;
    public SnakeMovement player2;

    [Header("Spawn Area")]
    public Vector2Int minCell;
    public Vector2Int maxCell;

    private GameObject currentFood;

    void Start()
    {
        SpawnFood();
    }

    public void SpawnFood()
    {
        if (currentFood != null)
        {
            Destroy(currentFood);
        }

        Vector2Int spawnPosition;

        do
        {
            int x = Random.Range(minCell.x, maxCell.x + 1);
            int y = Random.Range(minCell.y, maxCell.y + 1);

            spawnPosition = new Vector2Int(x, y);

        } while (IsSnakeAtPosition(spawnPosition));

        Vector3 worldPosition = GridToWorld(spawnPosition);

        currentFood = Instantiate(
            foodPrefab,
            worldPosition,
            Quaternion.identity
        );

        Food food = currentFood.GetComponent<Food>();

        if (food != null)
        {
            food.Setup(spawnPosition, this);
        }
    }

    bool IsSnakeAtPosition(Vector2Int position)
    {
        if (player1 != null && player1.GridPosition == position)
        {
            return true;
        }

        if (player2 != null && player2.GridPosition == position)
        {
            return true;
        }

        return false;
    }

    Vector3 GridToWorld(Vector2Int cell)
    {
        if (tilemapGrid != null)
        {
            return tilemapGrid.GetCellCenterWorld(
                new Vector3Int(cell.x, cell.y, 0)
            );
        }

        return new Vector3(
            cell.x + 0.5f,
            cell.y + 0.5f,
            0f
        );
    }

    public void FoodEaten(SnakeMovement snake)
    {
        // This is the snake that ate the food.
        // We'll use 'snake' to make that specific snake grow later.

        SpawnFood();
    }
}