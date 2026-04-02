using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform player;

    [Header("Background Layers (Back to Front)")]
    public SpriteRenderer layer1; // Farthest (e.g., sky)
    public SpriteRenderer layer2; // Middle (e.g., mountains)
    public SpriteRenderer layer3; // Closest (e.g., trees)

    [Header("Parallax Speeds")]
    [Tooltip("Lower = slower movement = feels farther away")]
    public float layer1Speed = 0.1f;
    public float layer2Speed = 0.3f;
    public float layer3Speed = 0.6f;

    private Vector3 previousPlayerPos;

    private void Start()
    {
        if (player == null)
            player = Camera.main.transform;

        previousPlayerPos = player.position;
    }

    private void LateUpdate()
    {
        Vector3 delta = player.position - previousPlayerPos;

        layer1.transform.position += new Vector3(delta.x * layer1Speed, delta.y * layer1Speed, 0f);
        layer2.transform.position += new Vector3(delta.x * layer2Speed, delta.y * layer2Speed, 0f);
        layer3.transform.position += new Vector3(delta.x * layer3Speed, delta.y * layer3Speed, 0f);

        previousPlayerPos = player.position;
    }
}
