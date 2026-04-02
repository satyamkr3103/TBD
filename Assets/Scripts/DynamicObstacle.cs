using UnityEngine;

public class DynamicObstacle : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private MeshRenderer meshRenderer;
    private Collider2D objCollider; 
    private PhysicsMaterial2D physicsMaterial; 

    private Vector3 originalScale;
    private bool isBouncyObj = false;
    private int jumpsRemaining = 3;

    [Header("Interactive Types")]
    public bool isHazardBlock = false;
    public bool isLadderBlock = false;
    public bool isConveyorBlock = false;
    public bool isWaterBlock = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
        objCollider = GetComponent<Collider2D>();
        originalScale = transform.localScale;

        if (objCollider != null)
        {
            physicsMaterial = new PhysicsMaterial2D("DynamicMaterial");
            physicsMaterial.friction = 0f; // Frictionless per user request
            // Ensure the collider has the instance of the material so we don't modify shared assets
            objCollider.sharedMaterial = physicsMaterial;
        }

        // Apply fallback material if this is a 3D procedural shape without one
        if (meshRenderer != null && meshRenderer.sharedMaterial == null && BlockManager.Instance != null)
        {
            meshRenderer.material = BlockManager.Instance.defaultProceduralMaterial;
        }
    }

    public void ApplyModifications(string json)
    {
        try
        {
            SingleModificationRequest mod = JsonUtility.FromJson<SingleModificationRequest>(json);

            // Apply Color
            if (!string.IsNullOrEmpty(mod.color) && ColorUtility.TryParseHtmlString(mod.color, out Color newColor))
            {
                if (spriteRenderer != null) spriteRenderer.color = newColor;
                if (meshRenderer != null) meshRenderer.material.color = newColor;
            }

            // Interactive Flags
            int groundLayer = LayerMask.NameToLayer("Ground");
            if (groundLayer != -1) gameObject.layer = groundLayer;

            try 
            {
                if (mod.isHazard) gameObject.tag = "Hazard";
                if (mod.isLadder) gameObject.tag = "Ladder";
                if (mod.isConveyor) gameObject.tag = "Conveyor";
                if (mod.isWater) gameObject.tag = "Water";
            }
            catch (UnityException)
            {
                Debug.LogWarning("Unity Tag not found! Please manually add 'Hazard', 'Ladder', 'Conveyor', and 'Water' to Edit -> Project Settings -> Tags and Layers to utilize them.");
            }
            
            if (mod.isHazard) 
            {
                isHazardBlock = true;
            }
            if (mod.isLadder)
            {
                isLadderBlock = true;
                SetupLadderColliders();
                try { gameObject.tag = "Ladder"; } catch (UnityException) {}
            }

            if (mod.isConveyor)
            {
                isConveyorBlock = true;
                Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
                foreach (Collider2D col in allColliders)
                {
                    col.usedByEffector = true;
                }
                SurfaceEffector2D effector = gameObject.AddComponent<SurfaceEffector2D>();
                effector.speed = 5f; // Pushes player right
                effector.forceScale = 1f;
            }

            if (mod.isWater)
            {
                isWaterBlock = true;
                Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
                foreach (Collider2D col in allColliders)
                {
                    col.isTrigger = true; // Player must be able to sink into it
                    col.usedByEffector = true;
                }
                BuoyancyEffector2D effector = gameObject.AddComponent<BuoyancyEffector2D>();
                effector.density = 1.5f; 

                // Attempt to make water semi-transparent 
                if (spriteRenderer != null) spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
                if (meshRenderer != null && meshRenderer.material != null) 
                {
                    Color c = meshRenderer.material.color;
                    meshRenderer.material.color = new Color(c.r, c.g, c.b, 0.5f);
                }
            }

            // Apply Bounciness (Only applies to 2D colliders currently)
            if (physicsMaterial != null && objCollider != null)
            {
                if (mod.isBouncy)
                {
                    isBouncyObj = true;
                    jumpsRemaining = 3;
                    physicsMaterial.bounciness = 1.3f; // elastic
                }
                else
                {
                    physicsMaterial.bounciness = Mathf.Clamp(mod.bounciness, 0f, 1f);
                }
                
                // Re-assign material to force an update in Unity's physics engine
                objCollider.sharedMaterial = null; 
                objCollider.sharedMaterial = physicsMaterial;
            }

            // Apply Scale
            float scaleMult = Mathf.Clamp(mod.scaleMultiplier, 0.5f, 3.0f);
            if(scaleMult > 0.01f) // Ensure it doesn't disappear completely
            {
                transform.localScale = originalScale * scaleMult;
            }

            // Apply Position Shift
            if (mod.position != null)
            {
                // We could move it absolutely or relatively. Moving absolutely based on AI response:
                transform.position = new Vector3(mod.position.x, mod.position.y, transform.position.z);
            }

            Debug.Log($"Applied material '{mod.material}' modifications successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to parse AI modifications: " + e.Message + "\nJSON: " + json);
        }
    }

    /// <summary>
    /// Properly configures dual colliders for ladder behaviour:
    /// Largest collider = SOLID (player can stand on top)
    /// Smallest collider = TRIGGER (climbing detection zone)
    /// If only one collider, keeps it solid and adds a new inset trigger.
    /// </summary>
    private void SetupLadderColliders()
    {
        // Ladders are purely trigger zones — player walks through them and climbs.
        // No solid collider is needed or wanted.
        BoxCollider2D[] boxes = GetComponents<BoxCollider2D>();
        CircleCollider2D[] circles = GetComponents<CircleCollider2D>();

        if (boxes.Length == 0 && circles.Length == 0)
        {
            // No collider at all? Add a single trigger box
            BoxCollider2D trigger = gameObject.AddComponent<BoxCollider2D>();
            trigger.isTrigger = true;
            return;
        }

        // Make every existing collider a trigger
        foreach (var b in boxes) b.isTrigger = true;
        foreach (var c in circles) c.isTrigger = true;
    }

    /// <summary>
    /// Publicly callable method so UIManager can convert any existing block into a ladder in-place.
    /// </summary>
    public void ConvertToLadder()
    {
        isLadderBlock = true;
        SetupLadderColliders();
        try { gameObject.tag = "Ladder"; } catch (UnityException) {}
    }

    void OnMouseDown()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleTargetObstacle(this);
            // Optional: Provide a visual pop when clicked
            transform.localScale = originalScale * 1.1f;
            Invoke(nameof(ResetVisualPop), 0.1f);
        }
    }

    void ResetVisualPop()
    {
        // Reset scale back to its current target scale, assuming no other modifications happening simultaneously
        transform.localScale = originalScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // For trampolines: count jumps and destroy after 3
        // Removed the strict "Player" tag check so any physical impact counts towards degradation!
        if (isBouncyObj)
        {
            jumpsRemaining--;
            if (jumpsRemaining <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
