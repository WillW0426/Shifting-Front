using UnityEngine;

public class UnitIcon : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform unit;
    [SerializeField] private Vector3 offset = Vector3.zero;

    [Header("Scaling")]
    [SerializeField] private float referenceZoom = 5f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 4f;

    private Camera mainCamera;
    private Vector3 baseScale;

    private void Awake()
    {
        mainCamera = Camera.main;
        baseScale = transform.localScale;
    }

    private void LateUpdate()
    {
        // Follow the unit without inheriting its rotation
        transform.position = unit.position + offset;
        transform.rotation = Quaternion.identity;

        // Scale based on camera zoom
        float scale = mainCamera.orthographicSize / referenceZoom;
        scale = Mathf.Clamp(scale, minScale, maxScale);

        transform.localScale = baseScale * scale;
    }
}
