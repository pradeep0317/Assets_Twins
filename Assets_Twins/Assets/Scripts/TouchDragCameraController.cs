using UnityEngine;
using UnityEngine.EventSystems;

public class TouchDragCameraController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float moveSensitivity = 0.02f;

    [Header("Rotation")]
    public float rotateSensitivity = 0.15f;
    public float maxLookAngle = 80f;

    private Vector2 startTouchPos;
    private bool isDragging = false;

    private float rotationX = 0f;

    void Update()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        // ✅ Ignore UI touches
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return;
        }

        switch (touch.phase)
        {
            case TouchPhase.Began:
                startTouchPos = touch.position;
                isDragging = true;
                break;

            case TouchPhase.Moved:
                if (!isDragging) return;

                Vector2 delta = touch.position - startTouchPos;

                // 🟢 LEFT HALF → MOVE
                if (startTouchPos.x < Screen.width / 2)
                {
                    Vector3 move =
                        transform.forward * (delta.y * moveSensitivity) +
                        transform.right * (delta.x * moveSensitivity);

                    transform.position += move * moveSpeed * Time.deltaTime;
                }
                // 🔵 RIGHT HALF → ROTATE
                else
                {
                    float yaw = delta.x * rotateSensitivity;
                    float pitch = -delta.y * rotateSensitivity;

                    rotationX += pitch;
                    rotationX = Mathf.Clamp(rotationX, -maxLookAngle, maxLookAngle);

                    transform.rotation = Quaternion.Euler(rotationX, transform.eulerAngles.y + yaw, 0f);
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                isDragging = false;
                break;
        }
    }
}
