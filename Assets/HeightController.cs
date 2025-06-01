using UnityEngine;


public class HeightLimiter : MonoBehaviour
{
    public float maxHeight = 1.0f;  // Maksymalna wysokość dla gracza

    void Update()
    {
        if (transform.position.y > maxHeight)
        {
            transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z);
        }
    }
}
