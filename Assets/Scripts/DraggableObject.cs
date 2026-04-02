using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private bool isDragging = true;
    private float dragTime = 5f;
    private float timer;
    private Rigidbody2D rb;

    void Start()
    {
        timer = dragTime;
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = mousePos;

            // 🔥 LEFT CLICK to place
            if (Input.GetMouseButtonDown(0))
            {
                PlaceObject();
            }
        }
    }
    void PlaceObject()
    {
        isDragging = false;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }

        // Snap
        SnapToGround snap = GetComponent<SnapToGround>();
        if (snap != null)
        {
            snap.Snap();
        }

        // Start lifetime
        GetComponent<BlockLifetime>().StartLifetime();
    }
    void CreateSupport()
    {
        Vector3 pos = transform.position;

        for (int i = 0; i < 5; i++)
        {
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);

            block.transform.position = new Vector3(pos.x, pos.y - i - 1, 0);

            block.AddComponent<BoxCollider2D>();
        }
    }
}