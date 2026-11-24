using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class InputManager : MonoBehaviour
{
    private RTSControls controls;
    private Camera mainCamera;

    [SerializeField] private LayerMask selectableLayers;
    [SerializeField] private List<UnitController> selectedUnits = new List<UnitController>();
    [SerializeField] private StructureSelection selectedStructure;
    [SerializeField] private bool isSequencing;
    [SerializeField] private bool isMultiSelecting;

    [Header("Box Selection")]
    [SerializeField] private RectTransform selectionBox;
    [SerializeField] private bool isSelectPressed;
    [SerializeField] private bool isBoxSelecting;

    private Vector2 selectStartScreenPosition;

    [SerializeField] private bool isInteractPressed;

    [SerializeField] private Vector2 interactStartScreenPosition;

    [SerializeField] private float dragThreshold = 5f;

    [SerializeField] private float formationSpacing;
    [SerializeField] private float minimumFormationSpacing = 1;

    //For showing movement previews
    [SerializeField] private bool isRevealing;
    public static event Action<bool> RevealChanged;
    //For placement preview
    [SerializeField] private bool isPlacementPreviewActive;
    private Vector2 interactDestination;
    private float previewAngle;

    private void Awake()
    {
        controls = new RTSControls();
        mainCamera = Camera.main;

        // Subscribe to selection input.
        controls.Player.Select.started += OnSelect;
        controls.Player.Select.canceled += OnSelectCanceled;

        // Subscribe to interaction input.
        controls.Player.Interact.started += OnInteract;
        controls.Player.Interact.canceled += OnInteractCanceled;

        // Track Shift being pressed and released.
        controls.Player.Sequence.started += OnSequenceStarted;
        controls.Player.Sequence.canceled += OnSequenceCanceled;

        // Track ctrl
        controls.Player.Multi.started += OnMultiStarted;
        controls.Player.Multi.canceled += OnMultiCanceled;

        //Visuals
        controls.Player.Reveal.started += OnRevealStarted;
        controls.Player.Reveal.canceled += OnRevealCanceled;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        isSelectPressed = false;
        isBoxSelecting = false;

        if (selectionBox != null)
            selectionBox.gameObject.SetActive(false);

        controls.Player.Disable();

        isSequencing = false;
        isRevealing = false;
        RevealChanged?.Invoke(false);
    }

    private void OnDestroy()
    {
        controls.Dispose();
    }

    private void Update()
    {
        selectedUnits.RemoveAll(unit => unit == null);
        UpdateSelection();

        if (!isInteractPressed || selectedUnits.Count == 0)
            return;

        // Detect whether the player has started dragging
        if (!isPlacementPreviewActive)
        {
            Vector2 mousePosition =
                controls.Player.Point.ReadValue<Vector2>();

            float dragDistance = Vector2.Distance(
                mousePosition, interactStartScreenPosition
            );

            if (dragDistance < dragThreshold)
                return;

            isPlacementPreviewActive = true;

            foreach (UnitController unit in selectedUnits)
            {
                unit.GetComponent<UnitVisuals>()
                    .ShowPlacementPreview(interactDestination, previewAngle);
            }
        }

        Vector2 drag = GetMouseWorldPosition() - interactDestination;

        if (selectedUnits.Count > 1)
        {
            Vector2 right = drag.normalized;

            if (right == Vector2.zero)
                right = selectedUnits[0].transform.right;

            Vector2 forward = new Vector2(-right.y, right.x);

            previewAngle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg - 90f;

            float spacing = Mathf.Max(
                minimumFormationSpacing,
                drag.magnitude / (selectedUnits.Count - 1)
            );

            List<UnitController> sortedUnits =
                new List<UnitController>(selectedUnits);

            sortedUnits.Sort((a, b) =>
                Vector2.Dot((Vector2)a.transform.position, right)
                .CompareTo(
                    Vector2.Dot((Vector2)b.transform.position, right)
                )
            );

            for (int i = 0; i < sortedUnits.Count; i++)
            {
                Vector2 previewPosition =
                    interactDestination + right * i * spacing;

                sortedUnits[i].GetComponent<UnitVisuals>()
                    .UpdatePlacementPreview(previewPosition, previewAngle);
            }
        }
        else
        {
            // 
            if (drag.sqrMagnitude > 0.0001f)
            {
                previewAngle = Mathf.Atan2(drag.y, drag.x) * Mathf.Rad2Deg;
            }

            selectedUnits[0].GetComponent<UnitVisuals>()
                .UpdatePlacementPreview(interactDestination, previewAngle);
        }
    }

    private Vector2 GetMouseWorldPosition()
    {
        Vector2 mousePosition = controls.Player.Point.ReadValue<Vector2>();

        Vector3 screenPosition = new Vector3(
            mousePosition.x,
            mousePosition.y,
            -mainCamera.transform.position.z
        );

        return mainCamera.ScreenToWorldPoint(screenPosition);
    }

    //Selecting
    private void SelectSingle()
    {
        if (IsPointerOverButton())
            return;

        Vector2 worldPosition = GetMouseWorldPosition();
        Collider2D hit = Physics2D.OverlapPoint(worldPosition, selectableLayers);

        if (hit != null){
            // Check for a structure 
            StructureSelection structure = hit.GetComponentInParent<StructureSelection>();

            if (structure != null)
            {
                ClearSelectedUnits();
                SelectStructure(structure);
                return;
            }

            UnitController unit = hit.GetComponentInParent<UnitController>();

            if (unit != null)
            {
                DeselectStructure();

                if (isMultiSelecting)
                {
                    // Toggle the unit's selection
                    if (selectedUnits.Contains(unit))
                        RemoveSelectedUnit(unit);
                    else
                        AddSelectedUnit(unit);
                }
                else
                {
                    // Replace the current selection
                    ClearSelectedUnits();
                    AddSelectedUnit(unit);
                }

                return;
            }
        }
        // Clicking empty space
        DeselectStructure();

        if (!isMultiSelecting)
            ClearSelectedUnits();
    }

    private void OnSelect(InputAction.CallbackContext context)
    {
        if (IsPointerOverButton())
            return;

        if (isInteractPressed)
            return;

        isSelectPressed = true;
        isBoxSelecting = false;

        selectStartScreenPosition =
            controls.Player.Point.ReadValue<Vector2>();
    }

    private void UpdateSelection()
    {
        if (!isSelectPressed)
            return;

        Vector2 mousePosition =
            controls.Player.Point.ReadValue<Vector2>();

        float dragDistance = Vector2.Distance(
            mousePosition, selectStartScreenPosition
        );

        if (!isBoxSelecting)
        {
            if (dragDistance < dragThreshold)
                return;

            isBoxSelecting = true;
            selectionBox.gameObject.SetActive(true);
        }

        UpdateSelectionBox(mousePosition);
    }

    private void UpdateSelectionBox(Vector2 mousePosition)
    {
        RectTransform parent = selectionBox.parent as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent,
            selectStartScreenPosition,
            null,
            out Vector2 start
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent,
            mousePosition,
            null,
            out Vector2 end
        );

        Vector2 center = (start + end) / 2f;

        Vector2 size = new Vector2(
            Mathf.Abs(end.x - start.x),
            Mathf.Abs(end.y - start.y)
        );

        selectionBox.localPosition = center;
        selectionBox.sizeDelta = size;
    }

    private void OnSelectCanceled(InputAction.CallbackContext context)
    {
        if (!isSelectPressed)
            return;

        isSelectPressed = false;

        Vector2 mousePosition =
            controls.Player.Point.ReadValue<Vector2>();

        float dragDistance = Vector2.Distance(
            mousePosition, selectStartScreenPosition
        );

        if (isBoxSelecting || dragDistance >= dragThreshold)
        {
            SelectUnitsInBox();
        }
        else
        {
            SelectSingle();
        }

        isBoxSelecting = false;
        selectionBox.gameObject.SetActive(false);
    }

    private void SelectUnitsInBox()
    {
        Vector2 start = mainCamera.ScreenToWorldPoint(
            new Vector3(
                selectStartScreenPosition.x,
                selectStartScreenPosition.y,
                -mainCamera.transform.position.z
            )
        );

        Vector2 end = GetMouseWorldPosition();

        Collider2D[] hits = Physics2D.OverlapAreaAll(
            Vector2.Min(start, end),
            Vector2.Max(start, end),
            selectableLayers
        );

        DeselectStructure();

        if (!isMultiSelecting)
            ClearSelectedUnits();

        foreach (Collider2D hit in hits)
        {
            UnitController unit =
                hit.GetComponentInParent<UnitController>();

            if (unit != null)
            {
                AddSelectedUnit(unit);
            }
        }
    }


    #region Multi Select Functions
    private void AddSelectedUnit(UnitController unit)
    {
        if (!selectedUnits.Contains(unit))
        {
            selectedUnits.Add(unit);
            unit.GetComponent<UnitVisuals>().ShowOutline();
        }
    }

    private void RemoveSelectedUnit(UnitController unit)
    {
        if (selectedUnits.Remove(unit))
        {
            unit.GetComponent<UnitVisuals>().HideOutline();
        }
    }

    private void ClearSelectedUnits()
    {
        foreach (UnitController unit in selectedUnits)
        {
            unit.GetComponent<UnitVisuals>().HideOutline();
        }

        selectedUnits.Clear();
    }

    private Vector2 GetGroupCenter()
    {
        Vector2 center = Vector2.zero;

        foreach (UnitController unit in selectedUnits)
        {
            if (isSequencing)
            {
                List<Vector2> route = unit.GetPlannedRoute();

                if (route.Count > 0)
                {
                    center += route[route.Count - 1];
                    continue;
                }
            }

            center += (Vector2)unit.transform.position;
        }

        return center / selectedUnits.Count;
    }

    private void MoveGroup(Vector2 destination)
    {
        Vector2 center = GetGroupCenter();
        Vector2 forward = (destination - center).normalized;

        if (forward == Vector2.zero)
            forward = selectedUnits[0].transform.up;

        Vector2 right = new Vector2(forward.y, -forward.x);

        Vector2 startPosition = destination
            - right * formationSpacing * (selectedUnits.Count - 1) / 2f;

        MoveGroup(startPosition, forward, formationSpacing);
    }

    private void MoveGroup(Vector2 startPosition, Vector2 forward, float spacing)
    {
        Vector2 right = new Vector2(forward.y, -forward.x);

        float finalAngle =
            Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg - 90f;

        List<UnitController> sortedUnits =
            new List<UnitController>(selectedUnits);

        sortedUnits.Sort((a, b) =>
            Vector2.Dot((Vector2)a.transform.position, right)
            .CompareTo(
                Vector2.Dot((Vector2)b.transform.position, right)
            )
        );

        for (int i = 0; i < sortedUnits.Count; i++)
        {
            Vector2 unitDestination = startPosition + right * i * spacing;

            UnitController unit = sortedUnits[i];

            unit.ClearFinalRotation();

            if (isSequencing)
                unit.QueueMove(unitDestination);
            else
                unit.MoveTo(unitDestination);

            unit.SetFinalRotation(finalAngle);
        }
    }

    #endregion

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (selectedUnits.Count == 0)
            return;

        isInteractPressed = true;

        interactDestination = GetMouseWorldPosition();

        interactStartScreenPosition =
            controls.Player.Point.ReadValue<Vector2>();

        previewAngle = selectedUnits[0].transform.eulerAngles.z;
    }

    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        if (!isInteractPressed)
            return;

        isInteractPressed = false;

        // Tap movement
        if (!isPlacementPreviewActive)
        {
            if (selectedUnits.Count > 1)
            {
                MoveGroup(interactDestination);
            }
            else if (selectedUnits.Count == 1)
            {
                UnitController unit = selectedUnits[0];

                unit.ClearFinalRotation();

                if (isSequencing)
                    unit.QueueMove(interactDestination);
                else
                    unit.MoveTo(interactDestination);
            }
        }
        // Drag movement
        else
        {
            foreach (UnitController unit in selectedUnits)
            {
                unit.GetComponent<UnitVisuals>()
                    .HidePlacementPreview();
            }

            if (selectedUnits.Count > 1)
            {
                Vector2 drag =
                    GetMouseWorldPosition() - interactDestination;

                Vector2 right = drag.normalized;

                if (right == Vector2.zero)
                    right = selectedUnits[0].transform.right;

                Vector2 forward = new Vector2(-right.y, right.x);

                float spacing = Mathf.Max(
                    minimumFormationSpacing,
                    drag.magnitude / (selectedUnits.Count - 1)
                );

                MoveGroup(interactDestination, forward, spacing);
            }
            else if (selectedUnits.Count == 1)
            {
                UnitController unit = selectedUnits[0];

                Vector2 drag =
                    GetMouseWorldPosition() - interactDestination;

                if (drag.sqrMagnitude > 0.0001f)
                {
                    previewAngle = Mathf.Atan2(drag.y, drag.x) * Mathf.Rad2Deg;
                }

                unit.ClearFinalRotation();

                if (isSequencing)
                    unit.QueueMove(interactDestination);
                else
                    unit.MoveTo(interactDestination);

                unit.SetFinalRotation(previewAngle);
            }
        }

        isPlacementPreviewActive = false;
    }

    #region Structures
    private void SelectStructure(StructureSelection structure)
    {
        // Hide the previously selected structure's buttons
        if (selectedStructure != null)
        {
            selectedStructure.Deselect();
        }

        selectedStructure = structure;
        selectedStructure.Select();
    }

    private void DeselectStructure()
    {
        if (selectedStructure != null)
        {
            selectedStructure.Deselect();
            selectedStructure = null;
        }
    }
    #endregion

    //----------------------------------------------------------------------------------------
    // Detecting Input
    #region Input Manager
    private void OnSequenceStarted(InputAction.CallbackContext context)
    {
        isSequencing = true;
        Debug.Log("Sequence mode enabled");
    }

    private void OnSequenceCanceled(InputAction.CallbackContext context)
    {
        isSequencing = false;
        Debug.Log("Sequence mode disabled");
    }
    private void OnMultiStarted(InputAction.CallbackContext context)
    {
        isMultiSelecting = true;
    }

    private void OnMultiCanceled(InputAction.CallbackContext context)
    {
        isMultiSelecting = false;
    }

    private void OnRevealStarted(InputAction.CallbackContext context)
    {
        isRevealing = true;
        RevealChanged?.Invoke(true);
    }

    private void OnRevealCanceled(InputAction.CallbackContext context)
    {
        isRevealing = false;
        RevealChanged?.Invoke(false);
    }
    #endregion
    private bool IsPointerOverButton()
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData pointerData =
            new PointerEventData(EventSystem.current);

        pointerData.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponentInParent<Button>() != null)
            {
                return true;
            }
        }

        return false;
    }
}



