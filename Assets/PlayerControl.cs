using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5f;
    public LayerMask unwalkableLayer;

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController component is missing on this GameObject.");
        }
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        if (movement != Vector3.zero)
        {
            Vector3 newPosition = transform.position + movement * moveSpeed * Time.deltaTime;
            if (IsWalkable(newPosition))
            {
                characterController.Move(movement * moveSpeed * Time.deltaTime);
            }
        }

        // Sprawdź, czy klawisz ESC został naciśnięty
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMenu();
        }
    }

    bool IsWalkable(Vector3 position)
    {
        Ray ray = new Ray(position + Vector3.up * 0.1f, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.2f))
        {
            if (((1 << hit.collider.gameObject.layer) & unwalkableLayer) != 0)
            {
                return false;
            }
        }
        return true;
    }

    void ReturnToMenu()
    {
        Debug.Log("Returning to Menu");
        SceneManager.LoadScene("Menu");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Seeker"))
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
