using UnityEngine;

public class SnakeSpawnImmunity : MonoBehaviour
{
    [Header("Immunity")]
    public float immunityDuration = 2f;

    [Header("Visuals")]
    [Range(0f, 1f)]
    public float immuneAlpha = 0.5f;

    private float immunityTimer;
    private bool isImmune;

    private SpriteRenderer headRenderer;
    private SnakeBody snakeBody;

    public bool IsImmune => isImmune;


    void Start()
    {
        headRenderer = GetComponent<SpriteRenderer>();
        snakeBody = GetComponent<SnakeBody>();

        StartImmunity();
    }


    void Update()
    {
        if (!isImmune)
            return;

        immunityTimer -= Time.deltaTime;

        if (immunityTimer <= 0f)
        {
            immunityTimer = 0f;
            isImmune = false;

            SetAlpha(1f);

            Debug.Log("Spawn immunity ended.");
        }
    }


    public void StartImmunity()
    {
        immunityTimer = immunityDuration;
        isImmune = true;

        SetAlpha(immuneAlpha);

        Debug.Log(
            "Snake is immune for " +
            immunityDuration +
            " seconds."
        );
    }


    void SetAlpha(float alpha)
    {
        // Head
        if (headRenderer != null)
        {
            Color color = headRenderer.color;
            color.a = alpha;
            headRenderer.color = color;
        }

        // Body
        if (snakeBody != null)
        {
            foreach (GameObject segment in snakeBody.Segments)
            {
                if (segment == null)
                    continue;

                SpriteRenderer renderer =
                    segment.GetComponent<SpriteRenderer>();

                if (renderer == null)
                    continue;

                Color color = renderer.color;
                color.a = alpha;
                renderer.color = color;
            }
        }
    }

    public void RefreshVisuals()
    {
        if (isImmune)
        {
            SetAlpha(immuneAlpha);
        }
    }

}