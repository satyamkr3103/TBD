using UnityEngine;

public class SnapToGround : MonoBehaviour
{
    public float rayDistance = 5f;
    public LayerMask groundLayer;

    public bool Snap()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, groundLayer);

        if (hit.collider != null && hit.distance < 2f)
        {
            float objectHeight = GetComponent<Collider2D>().bounds.extents.y;

            transform.position = new Vector3(
                transform.position.x,
                hit.point.y + objectHeight,
                transform.position.z
            );

            return true; // snapped
        }

        return false; // no ground
    }
}