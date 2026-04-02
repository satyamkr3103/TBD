using UnityEngine;
using System.Collections.Generic;

public class PromptGuardrails : MonoBehaviour
{
    [Tooltip("Maximum allowed characters in the prompt.")]
    [SerializeField] private int maxLength = 200;
    
    [Tooltip("List of banned words that will trigger a rejection.")]
    [SerializeField] private List<string> bannedWords = new List<string> { "kill", "die", "sex", "hack", "bypass", "cheat", "godmode" };
    
    [Tooltip("Cooldown in seconds between AI requests to prevent spam.")]
    [SerializeField] private float requestCooldown = 5f;
    
    [Header("Spatial Boundaries")]
    [Tooltip("Allowed play area minimum bounds (X, Y).")]
    public Vector2 minBounds = new Vector2(-10f, -5f);
    
    [Tooltip("Allowed play area maximum bounds (X, Y).")]
    public Vector2 maxBounds = new Vector2(10f, 5f);

    [Header("Energy Costs")]
    [Tooltip("Base energy cost just for making a request.")]
    public float baseRequestCost = 5f;
    [Tooltip("Cost per standard word.")]
    public float costPerWord = 1f;

    // Dictionary of high-cost complex words. Could also be made visible in editor via custom struct.
    private Dictionary<string, float> expensiveWords = new Dictionary<string, float>()
    {
        { "huge", 20f }, { "giant", 20f }, { "massive", 20f },
        { "bouncy", 15f }, { "rubber", 15f },
        { "teleport", 30f }, { "fly", 25f },
        { "metal", 10f }, { "steel", 10f },
        { "tiny", 10f }, { "small", 10f },
        { "red", 5f }, { "blue", 5f }, { "green", 5f }
    };

    private float lastRequestTime = -10f;

    /// <summary>
    /// Validates the prompt against length restrictions, cooldowns, and banned words.
    /// </summary>
    public bool ValidatePrompt(string prompt, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            errorMessage = "Prompt cannot be empty.";
            return false;
        }

        if (prompt.Length > maxLength)
        {
            errorMessage = $"Prompt is too long. Max length is {maxLength} characters.";
            return false;
        }

        if (Time.time - lastRequestTime < requestCooldown)
        {
            errorMessage = $"Please wait {requestCooldown - (Time.time - lastRequestTime):F1} seconds before trying again.";
            return false;
        }

        string lowerPrompt = prompt.ToLower();
        foreach (var word in bannedWords)
        {
            if (lowerPrompt.Contains(word.ToLower()))
            {
                errorMessage = "Prompt contains inappropriate or banned words.";
                return false;
            }
        }

        lastRequestTime = Time.time;
        errorMessage = "";
        return true;
    }

    /// <summary>
    /// Forces the AI modification request coordinates to stay inside the allowed play area.
    /// </summary>
    public void ClampRequestPosition(SingleModificationRequest requestData)
    {
        if (requestData.position == null) 
            requestData.position = new PositionData { x = 0, y = 0 };

        requestData.position.x = Mathf.Clamp(requestData.position.x, minBounds.x, maxBounds.x);
        requestData.position.y = Mathf.Clamp(requestData.position.y, minBounds.y, maxBounds.y);
    }

    /// <summary>
    /// Validates a candidate world-space spawn position. Returns null if the position is safe,
    /// or a friendly error string explaining why it is rejected.
    /// </summary>
    public string ValidateSpawnPosition(Vector2 spawnPos, Transform playerTransform = null)
    {
        // 1. Bounds check
        if (spawnPos.x < minBounds.x || spawnPos.x > maxBounds.x ||
            spawnPos.y < minBounds.y || spawnPos.y > maxBounds.y)
        {
            return $"That location is out of bounds! Target must be between X({minBounds.x},{maxBounds.x}) and Y({minBounds.y},{maxBounds.y}).";
        }

        // 2. Overlap check — is something already occupying this position?
        Collider2D overlap = Physics2D.OverlapCircle(spawnPos, 0.45f);
        if (overlap != null)
        {
            bool isPlayer = playerTransform != null && overlap.transform == playerTransform;
            if (!isPlayer)
            {
                return $"Something is already at that position ({spawnPos.x:F1}, {spawnPos.y:F1}). Try a different spot!";
            }
        }

        // 3. Wall occlusion check (horizontal ray from player toward spawn)
        if (playerTransform != null)
        {
            Vector2 fromPlayer = new Vector2(playerTransform.position.x, playerTransform.position.y);
            Vector2 toSpawn = spawnPos - fromPlayer;
            float dist = toSpawn.magnitude;
            if (dist > 0.1f)
            {
                RaycastHit2D wallHit = Physics2D.Raycast(fromPlayer, toSpawn.normalized, dist - 0.3f);
                if (wallHit.collider != null && wallHit.collider.transform != playerTransform)
                {
                    return $"A wall is blocking the path to ({spawnPos.x:F1}, {spawnPos.y:F1})! Try placing it closer or to the side.";
                }
            }
        }

        return null; // all checks passed!
    }

    /// <summary>
    /// Calculates the dynamic energy cost of a prompt based on specific expensive words and overall length.
    /// </summary>
    public float CalculateEnergyCost(string prompt)
    {
        float totalCost = baseRequestCost;

        if (string.IsNullOrWhiteSpace(prompt))
            return totalCost;

        string[] words = prompt.ToLower().Split(new char[] { ' ', '.', ',', '!', '?' }, System.StringSplitOptions.RemoveEmptyEntries);
        
        // Add flat cost per word
        totalCost += (words.Length * costPerWord);

        // Add premium cost for specific powerful adjectives or verbs
        foreach (string w in words)
        {
            if (expensiveWords.ContainsKey(w))
            {
                totalCost += expensiveWords[w];
            }
        }

        return totalCost;
    }
}
