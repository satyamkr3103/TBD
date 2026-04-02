using UnityEngine;

public class BlockLifetime : MonoBehaviour
{
    public float lifetime = 10f;

    public void StartLifetime()
    {
        Invoke(nameof(DestroyBlock), lifetime);
    }

    void DestroyBlock()
    {
        Destroy(gameObject);
    }
}