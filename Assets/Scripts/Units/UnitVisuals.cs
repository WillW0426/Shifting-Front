using System.Collections.Generic;
using UnityEngine;

public class UnitVisuals : MonoBehaviour
{
    [Header("Selection")]
    [SerializeField] private GameObject selectionOutline;

    [Header("Movement Preview")]
    [SerializeField] private LineRenderer movementLine;
    [SerializeField] private GameObject destinationOutline;
    [SerializeField] private GameObject placementPreview;

    private UnitController controller;
    private bool showMovementPreview;

    private void Awake()
    {
        controller = GetComponent<UnitController>();

        HideOutline();
        SetMovementPreview(false);
        placementPreview.SetActive(false);
    }
    private void OnEnable()
    {
        InputManager.RevealChanged += SetMovementPreview;
    }

    private void OnDisable()
    {
        InputManager.RevealChanged -= SetMovementPreview;
    }

    public void ShowOutline()
    {
        selectionOutline.SetActive(true);
    }

    public void HideOutline()
    {
        selectionOutline.SetActive(false);
    }

    public void SetMovementPreview(bool visible)
    {
        showMovementPreview = visible;

        movementLine.enabled = visible;

        if (!visible)
        {
            movementLine.positionCount = 0;
            destinationOutline.SetActive(false);
        }
    }

    private void Update()
    {
        if (!showMovementPreview)
            return;

        DrawMovementPreview();
    }

    private void DrawMovementPreview()
    {
        List<Vector2> route = controller.GetPlannedRoute();

        if (route.Count == 0)
        {
            movementLine.positionCount = 0;
            destinationOutline.SetActive(false);
            return;
        }

        // The first point is the unit's current position.
        movementLine.positionCount = route.Count + 1;
        movementLine.SetPosition(0, transform.position);

        // Add every planned destination to the line.
        for (int i = 0; i < route.Count; i++)
        {
            Vector2 point = route[i];

            movementLine.SetPosition(
                i + 1,
                new Vector3(point.x, point.y, transform.position.z)
            );
        }

        // Place the outline at the final destination.
        Vector2 finalDestination = route[route.Count - 1];

        destinationOutline.transform.position = new Vector3(
            finalDestination.x,
            finalDestination.y,
            transform.position.z
        );

        // Predict the unit's final facing direction.
        float finalAngle = controller.GetPlannedFinalAngle();

        destinationOutline.transform.rotation =
            Quaternion.Euler(0f, 0f, finalAngle);

        destinationOutline.SetActive(true);
    }

    public void ShowPlacementPreview(Vector2 position, float angle)
    {
        placementPreview.SetActive(true);
        UpdatePlacementPreview(position, angle);
    }

    public void UpdatePlacementPreview(Vector2 position, float angle)
    {
        placementPreview.transform.position =
            new Vector3(position.x, position.y, transform.position.z);

        placementPreview.transform.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }

    public void HidePlacementPreview()
    {
        placementPreview.SetActive(false);
    }
}
