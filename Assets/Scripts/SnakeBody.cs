using System.Collections.Generic;
using UnityEngine;

public class SnakeBody : MonoBehaviour
{
    [Header("References")]
    public SnakeMovement snakeMovement;
    public GameObject segmentPrefab;
    public Grid tilemapGrid;

    [Header("Starting Length")]
    public int startingLength = 2;

    [Header("Head Sprites")]
    public Sprite headUp;
    public Sprite headDown;
    public Sprite headLeft;
    public Sprite headRight;

    [Header("Body Sprites")]
    public Sprite bodyHorizontal;
    public Sprite bodyVertical;

    [Header("Corner Sprites")]
    public Sprite cornerUpRight;
    public Sprite cornerUpLeft;
    public Sprite cornerDownRight;
    public Sprite cornerDownLeft;

    [Header("Tail Sprites")]
    public Sprite tailUp;
    public Sprite tailDown;
    public Sprite tailLeft;
    public Sprite tailRight;

    private List<GameObject> segments = new List<GameObject>();

    public List<GameObject> Segments => segments;

    // Position history.
    // Index 0 = head
    // Index 1 = first body segment
    // Index 2 = second body segment
    // etc.
    private List<Vector2Int> positionHistory =
        new List<Vector2Int>();

    private Vector2Int lastHeadPosition;

    // How many segments still need to be added.
    private int pendingGrowth = 0;


    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        if (snakeMovement == null)
        {
            snakeMovement = GetComponent<SnakeMovement>();
        }

        ResetBody();
    }


    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        if (snakeMovement == null)
            return;

        UpdateHeadSprite();

        Vector2Int currentHeadPosition =
            snakeMovement.GridPosition;

        // The head entered a new grid cell.
        if (currentHeadPosition != lastHeadPosition)
        {
            positionHistory.Insert(0, currentHeadPosition);

            lastHeadPosition = currentHeadPosition;

            // If food was eaten, add the new segment now.
            if (pendingGrowth > 0)
            {
                CreateGrowthSegment();
                pendingGrowth--;
            }

            // NEW:
            // The snake has now actually moved.
            // Enable collisions on all body segments.
            EnableSegmentCollisions();
        }

        UpdateBodyPositions();
    }


    // =========================================================
    // GROWTH
    // =========================================================

    public void Grow()
    {
        // Don't create a segment immediately.
        //
        // This prevents a new segment from appearing directly
        // on top of the head when food is eaten.
        pendingGrowth++;
        SnakeSpawnImmunity immunity =
        GetComponent<SnakeSpawnImmunity>();

        if (immunity != null)
        {
            immunity.RefreshVisuals();
        }
    }


    // =========================================================
    // CREATE SEGMENT
    // =========================================================

    void CreateGrowthSegment()
    {
        // Spawn the segment at the current tail position.
        Vector3 spawnPosition = transform.position;

        if (positionHistory.Count > 1)
        {
            Vector2Int tailPosition =
                positionHistory[positionHistory.Count - 1];

            spawnPosition = GridToWorld(tailPosition);
        }

        GameObject newSegment = Instantiate(
            segmentPrefab,
            spawnPosition,
            Quaternion.identity
        );

        SnakeBodySegment segment =
            newSegment.GetComponent<SnakeBodySegment>();

        if (segment != null)
        {
            segment.owner = this;

            // NEW:
            // The new segment should not immediately count
            // its initial overlap as a collision.
            segment.DisableCollision();
        }

        segments.Add(newSegment);
    }


    // =========================================================
    // RESET
    // =========================================================

    public void ResetBody()
    {
        // Remove existing body segments.
        foreach (GameObject segment in segments)
        {
            if (segment != null)
            {
                Destroy(segment);
            }
        }

        segments.Clear();

        pendingGrowth = 0;

        positionHistory.Clear();

        Vector2Int headPosition =
            snakeMovement.GridPosition;

        lastHeadPosition = headPosition;

        // Add the head.
        positionHistory.Add(headPosition);

        // Build the starting body behind the head.
        Vector2Int backwards =
            -snakeMovement.Direction;

        for (int i = 1; i <= startingLength; i++)
        {
            positionHistory.Add(
                headPosition + backwards * i
            );
        }

        // Create the starting body.
        for (int i = 0; i < startingLength; i++)
        {
            CreateStartingSegment(i);
        }

        UpdateHeadSprite();
        UpdateBodyPositions();

        SnakeSpawnImmunity immunity =
        GetComponent<SnakeSpawnImmunity>();

        if (immunity != null)
        {
            immunity.RefreshVisuals();
        }
    }


    // =========================================================
    // CREATE STARTING SEGMENT
    // =========================================================

    void CreateStartingSegment(int segmentIndex)
    {
        Vector2Int position =
            positionHistory[segmentIndex + 1];

        GameObject newSegment = Instantiate(
            segmentPrefab,
            GridToWorld(position),
            Quaternion.identity
        );

        SnakeBodySegment segment =
            newSegment.GetComponent<SnakeBodySegment>();

        if (segment != null)
        {
            segment.owner = this;

            // NEW:
            // Starting segments cannot cause a collision
            // just because they spawned next to each other.
            segment.DisableCollision();
        }

        segments.Add(newSegment);
    }


    // =========================================================
    // NEW - ENABLE BODY COLLISIONS
    // =========================================================

    void EnableSegmentCollisions()
    {
        foreach (GameObject segment in segments)
        {
            if (segment == null)
                continue;

            SnakeBodySegment bodySegment =
                segment.GetComponent<SnakeBodySegment>();

            if (bodySegment != null)
            {
                bodySegment.EnableCollision();
            }
        }
    }


    // =========================================================
    // BODY MOVEMENT
    // =========================================================

    void UpdateBodyPositions()
    {
        if (positionHistory.Count < 2)
            return;

        float moveInterval =
            1f / Mathf.Max(
                0.0001f,
                snakeMovement.movesPerSecond
            );

        float rawT =
            Mathf.Clamp01(
                snakeMovement.MoveTimer / moveInterval
            );

        // NEW:
        // Linear interpolation keeps the head and body
        // moving at exactly the same rate.
        float t = rawT;


        for (int i = 0; i < segments.Count; i++)
        {
            int currentIndex = i + 1;
            int previousIndex = i + 2;

            // If there isn't enough history yet, keep the
            // segment at the last available position.
            if (currentIndex >= positionHistory.Count)
                continue;

            Vector2Int currentPosition =
                positionHistory[currentIndex];

            Vector2Int previousPosition;

            if (previousIndex < positionHistory.Count)
            {
                previousPosition =
                    positionHistory[previousIndex];
            }
            else
            {
                previousPosition =
                    currentPosition;
            }

            Vector3 from =
                GridToWorld(previousPosition);

            Vector3 to =
                GridToWorld(currentPosition);

            segments[i].transform.position =
                Vector3.Lerp(from, to, t);

            UpdateSegmentSprite(
                i,
                currentIndex
            );
        }
    }


    // =========================================================
    // HEAD SPRITE
    // =========================================================

    void UpdateHeadSprite()
    {
        SpriteRenderer renderer =
            snakeMovement.GetComponent<SpriteRenderer>();

        if (renderer == null)
            return;

        Vector2Int direction =
            snakeMovement.Direction;

        if (direction == Vector2Int.up)
        {
            renderer.sprite = headUp;
        }
        else if (direction == Vector2Int.down)
        {
            renderer.sprite = headDown;
        }
        else if (direction == Vector2Int.left)
        {
            renderer.sprite = headLeft;
        }
        else if (direction == Vector2Int.right)
        {
            renderer.sprite = headRight;
        }
    }


    // =========================================================
    // SEGMENT SPRITE
    // =========================================================

    void UpdateSegmentSprite(
        int segmentIndex,
        int historyIndex)
    {
        SpriteRenderer renderer =
            segments[segmentIndex].GetComponent<SpriteRenderer>();

        if (renderer == null)
            return;

        // Last segment = tail.
        if (segmentIndex == segments.Count - 1)
        {
            renderer.sprite =
                GetTailSprite(historyIndex);

            return;
        }

        renderer.sprite =
            GetBodySprite(historyIndex);
    }


    // =========================================================
    // BODY / CORNER SPRITES
    // =========================================================

    Sprite GetBodySprite(int index)
    {
        if (index <= 0 ||
            index >= positionHistory.Count - 1)
        {
            return bodyHorizontal;
        }

        Vector2Int previous =
            positionHistory[index - 1];

        Vector2Int current =
            positionHistory[index];

        Vector2Int next =
            positionHistory[index + 1];

        Vector2Int directionToPrevious =
            previous - current;

        Vector2Int directionToNext =
            next - current;


        // Vertical
        if ((directionToPrevious == Vector2Int.up &&
             directionToNext == Vector2Int.down) ||
            (directionToPrevious == Vector2Int.down &&
             directionToNext == Vector2Int.up))
        {
            return bodyVertical;
        }


        // Horizontal
        if ((directionToPrevious == Vector2Int.left &&
             directionToNext == Vector2Int.right) ||
            (directionToPrevious == Vector2Int.right &&
             directionToNext == Vector2Int.left))
        {
            return bodyHorizontal;
        }


        // Up + Right
        if ((directionToPrevious == Vector2Int.up &&
             directionToNext == Vector2Int.right) ||
            (directionToPrevious == Vector2Int.right &&
             directionToNext == Vector2Int.up))
        {
            return cornerUpRight;
        }


        // Up + Left
        if ((directionToPrevious == Vector2Int.up &&
             directionToNext == Vector2Int.left) ||
            (directionToPrevious == Vector2Int.left &&
             directionToNext == Vector2Int.up))
        {
            return cornerUpLeft;
        }


        // Down + Right
        if ((directionToPrevious == Vector2Int.down &&
             directionToNext == Vector2Int.right) ||
            (directionToPrevious == Vector2Int.right &&
             directionToNext == Vector2Int.down))
        {
            return cornerDownRight;
        }


        // Down + Left
        if ((directionToPrevious == Vector2Int.down &&
             directionToNext == Vector2Int.left) ||
            (directionToPrevious == Vector2Int.left &&
             directionToNext == Vector2Int.down))
        {
            return cornerDownLeft;
        }


        return bodyHorizontal;
    }


    // =========================================================
    // TAIL SPRITE
    // =========================================================

    Sprite GetTailSprite(int index)
    {
        if (index <= 0 ||
            index >= positionHistory.Count)
        {
            return tailRight;
        }

        Vector2Int tailPosition =
            positionHistory[index];

        Vector2Int positionBeforeTail =
            positionHistory[index - 1];

        Vector2Int direction =
            positionBeforeTail - tailPosition;


        if (direction == Vector2Int.up)
            return tailUp;

        if (direction == Vector2Int.down)
            return tailDown;

        if (direction == Vector2Int.left)
            return tailLeft;

        if (direction == Vector2Int.right)
            return tailRight;

        return tailRight;
    }


    // =========================================================
    // GRID TO WORLD
    // =========================================================

    Vector3 GridToWorld(Vector2Int cell)
    {
        if (tilemapGrid != null)
        {
            return tilemapGrid.GetCellCenterWorld(
                new Vector3Int(
                    cell.x,
                    cell.y,
                    0
                )
            );
        }

        return new Vector3(
            cell.x + 0.5f,
            cell.y + 0.5f,
            0f
        );
    }
}