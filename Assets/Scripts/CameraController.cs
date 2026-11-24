using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;

    
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private float zoomSmoothness = 10f;

    private Camera cam;
    private float targetZoom;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    //Reminder to incorporate this into the inputsystem later
    private void HandleMovement()
    {
        Vector2 movement = Vector2.zero;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.wKey.isPressed)
            movement.y += 1;

        if (Keyboard.current.sKey.isPressed)
            movement.y -= 1;

        if (Keyboard.current.aKey.isPressed)
            movement.x -= 1;

        if (Keyboard.current.dKey.isPressed)
            movement.x += 1;

        movement.Normalize();

        float adjustedSpeed = moveSpeed * (cam.orthographicSize / 10f);

        transform.position +=
            (Vector3)(movement * adjustedSpeed * Time.deltaTime);
    }

    private void HandleZoom()
    {
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;

            if (scroll != 0)
            {
                targetZoom -= Mathf.Sign(scroll) * zoomSpeed;
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            }
        }

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetZoom,
            zoomSmoothness * Time.deltaTime
        );
    }
}
