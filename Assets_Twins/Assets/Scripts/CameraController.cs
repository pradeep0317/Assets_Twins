using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float fastSpeed = 10f;

    [Header("Rotation (Keyboard Only)")]
    public float rotateSpeed = 60f; // arrow key rotation speed

    void Update()
    {
        // ⌨️ Movement
        float speed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : moveSpeed;

        Vector3 move =
            transform.forward * Input.GetAxis("Vertical") +
            transform.right * Input.GetAxis("Horizontal");

        // Up / Down
        if (Input.GetKey(KeyCode.E))
            move += transform.up;
        if (Input.GetKey(KeyCode.Q))
            move -= transform.up;

        transform.position += move * speed * Time.deltaTime;

        // ⌨️ Rotation using arrow keys (optional)
        float rotateY = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
            rotateY = -1f;
        if (Input.GetKey(KeyCode.RightArrow))
            rotateY = 1f;

        transform.Rotate(0f, rotateY * rotateSpeed * Time.deltaTime, 0f);
    }
}