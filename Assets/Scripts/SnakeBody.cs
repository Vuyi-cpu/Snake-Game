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
    private List<Vector2Int> positionHistory = new List<Vector2Int>();

    private Vector2Int lastHeadPosition;

    // How many new segments should be added.
    private int pendingGrowth = 0;


    // ==================================================
    // START
    // ==================================================

    void Start()
    {
        if (snakeMovement == null)
        {
            snakeMovement = GetComponent<SnakeMovement>();
        }

        ResetBody();
    }


    // ==================================================
    // UPDATE
    // ==================================================

    void Update()
    {
        if (snakeMovement == null)
            return;

        UpdateHeadSprite();

        Vector2Int currentHeadPosition = snakeMovement.GridPosition;

        // The snake has moved to a new grid cell.
        if (currentHeadPosition != lastHeadPosition)
        {
            // Add the new head position to the history.
            positionHistory.Insert(0, currentHeadPosition);

            lastHeadPosition = currentHeadPosition;

            // If the snake has eaten food, create the
            // new segment now that the snake has moved.
            if (pendingGrowth > 0)
            {
                CreateGrowthSegment();

                pendingGrowth--;
            }

            UpdateBody();
        }
    }


    // ==================================================
    // GROW
    // ==================================================

    public void Grow()
    {
        // Don't create the segment immediately.
        // Instead, tell the snake that it needs to grow
        // on its next movement.
        pendingGrowth++;
    }


    // ==================================================
    // CREATE NEW SEGMENT
    // ==================================================

    void CreateGrowthSegment()
    {
        GameObject newSegment = Instantiate(
            segmentPrefab,
            transform.position,
            Quaternion.identity
        );

        // Set the owner of this segment.
        SnakeBodySegment segment =
            newSegment.GetComponent<SnakeBodySegment>();

        if (segment != null)
        {
            segment.owner = this;
        }

        segments.Add(newSegment);
    }


    // ==================================================
    // RESET BODY
    // ==================================================

    public void ResetBody()
    {
        // Destroy existing segments.
        foreach (GameObject segment in segments)
        {
            if (segment != null)
            {
                Destroy(segment);
            }
        }

        segments.Clear();

        // Cancel any pending growth.
        pendingGrowth = 0;

        // Clear movement history.
        positionHistory.Clear();

        Vector2Int headPosition =
            snakeMovement.GridPosition;

        lastHeadPosition = headPosition;

        // Add head position.
        positionHistory.Add(headPosition);

        // Build starting body positions behind the head.
        Vector2Int backwards =
            -snakeMovement.Direction;

        for (int i = 1; i <= startingLength; i++)
        {
            positionHistory.Add(
                headPosition + backwards * i
            );
        }

        // Create starting body segments.
        for (int i = 0; i < startingLength; i++)
        {
            CreateGrowthSegment();
        }

        UpdateHeadSprite();
        UpdateBody();
    }


    // ==================================================
    // HEAD SPRITE
    // ==================================================

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


    // ==================================================
    // UPDATE BODY
    // ==================================================

    void UpdateBody()
    {
        if (positionHistory.Count < 2)
            return;

        for (int i = 0; i < segments.Count; i++)
        {
            int historyIndex = i + 1;

            if (historyIndex >= positionHistory.Count)
            {
                historyIndex =
                    positionHistory.Count - 1;
            }

            Vector2Int currentPosition =
                positionHistory[historyIndex];

            segments[i].transform.position =
                GridToWorld(currentPosition);

            SpriteRenderer renderer =
                segments[i].GetComponent<SpriteRenderer>();

            if (renderer == null)
                continue;

            renderer.sprite =
                GetSegmentSprite(
                    i,
                    historyIndex
                );
        }
    }


    // ==================================================
    // GET BODY SPRITE
    // ==================================================

    Sprite GetSegmentSprite(
        int segmentIndex,
        int historyIndex)
    {
        // Last segment is the tail.
        if (segmentIndex == segments.Count - 1)
        {
            return GetTailSprite(historyIndex);
        }

        return GetBodySprite(historyIndex);
    }


    // ==================================================
    // BODY / CORNER SPRITES
    // ==================================================

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


    // ==================================================
    // TAIL SPRITE
    // ==================================================

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


    // ==================================================
    // GRID TO WORLD
    // ==================================================

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
