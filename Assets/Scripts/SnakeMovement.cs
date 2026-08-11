using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeMovement : MonoBehaviour
{
    public enum InputScheme { WASD, ArrowKeys }

    [Header("Player")]
    public InputScheme inputScheme = InputScheme.WASD;

    [Header("Grid")]
    public Vector2Int startCell = Vector2Int.zero;
    public Vector2Int startDirection = Vector2Int.right;
    public float cellSize = 1f;

    [Header("Speed")]
    public float movesPerSecond = 8f;

    private Vector2Int gridPosition;
    private Vector2Int previousGridPosition;

    private Vector2Int direction;
    private Vector2Int input;
    private float moveTimer;

    public Vector2Int Direction => direction;
    public Vector2Int GridPosition => gridPosition;

    void Start()
    {
        ResetState();
    }

    void Update()
    {
        HandleInput();

        float moveInterval = 1f / Mathf.Max(0.0001f, movesPerSecond);
        moveTimer += Time.deltaTime;

        if (moveTimer >= moveInterval)
        {
            moveTimer -= moveInterval;
            Move();
        }

        float rawT = Mathf.Clamp01(moveTimer / moveInterval);
        float t = Mathf.SmoothStep(0f, 1f, rawT);

        Vector3 from = GridToWorld(previousGridPosition);
        Vector3 to = GridToWorld(gridPosition);
        transform.position = Vector3.Lerp(from, to, t);
    }

    void HandleInput()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        Vector2Int? newDir = null;

        if (inputScheme == InputScheme.WASD)
        {
            if (kb.wKey.wasPressedThisFrame) newDir = Vector2Int.up;
            else if (kb.sKey.wasPressedThisFrame) newDir = Vector2Int.down;
            else if (kb.aKey.wasPressedThisFrame) newDir = Vector2Int.left;
            else if (kb.dKey.wasPressedThisFrame) newDir = Vector2Int.right;
        }
        else
        {
            if (kb.upArrowKey.wasPressedThisFrame) newDir = Vector2Int.up;
            else if (kb.downArrowKey.wasPressedThisFrame) newDir = Vector2Int.down;
            else if (kb.leftArrowKey.wasPressedThisFrame) newDir = Vector2Int.left;
            else if (kb.rightArrowKey.wasPressedThisFrame) newDir = Vector2Int.right;
        }

        if (newDir.HasValue && newDir.Value != -direction)
            input = newDir.Value;
    }

    void Move()
    {
        previousGridPosition = gridPosition;
        direction = input;
        gridPosition += direction;
    }

    public void ResetState()
    {
        gridPosition = startCell;
        previousGridPosition = startCell;
        direction = startDirection;
        input = startDirection;
        moveTimer = 0f;

        transform.position = GridToWorld(gridPosition);
    }

    Vector3 GridToWorld(Vector2Int cell)
    {
        return new Vector3(cell.x * cellSize, cell.y * cellSize, 0f);
    }
}