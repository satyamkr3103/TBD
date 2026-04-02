using UnityEngine;

public class SnapToEdge : MonoBehaviour
{
    public float snapDistance = 1.5f;
    public LayerMask groundLayer;

    public void Snap()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, snapDistance, groundLayer);

        if (hits.Length == 0) return;

        Collider2D nearest = hits[0];
        float minDist = Vector2.Distance(transform.position, nearest.transform.position);

        // 🔍 Find closest platform
        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                nearest = hit;
                minDist = dist;
            }
        }

        Bounds bounds = nearest.bounds;

        float objectWidth = GetComponent<Collider2D>().bounds.extents.x;
        float objectHeight = GetComponent<Collider2D>().bounds.extents.y;

        Vector3 pos = transform.position;

        // 🎯 Snap to LEFT edge
        float leftEdge = bounds.min.x - objectWidth;

        // 🎯 Snap to RIGHT edge
        float rightEdge = bounds.max.x + objectWidth;

        // 🎯 Snap to TOP
        float topEdge = bounds.max.y + objectHeight;

        // Decide closest snap point
        float distLeft = Mathf.Abs(pos.x - leftEdge);
        float distRight = Mathf.Abs(pos.x - rightEdge);
        float distTop = Mathf.Abs(pos.y - topEdge);

        if (distTop < distLeft && distTop < distRight)
        {
            transform.position = new Vector3(pos.x, topEdge, pos.z);
        }
        else if (distLeft < distRight)
        {
            transform.position = new Vector3(leftEdge, pos.y, pos.z);
        }
        else
        {
            transform.position = new Vector3(rightEdge, pos.y, pos.z);
        }
    }
}