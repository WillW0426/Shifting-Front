using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;


public class InputManager : MonoBehaviour
{
    private RTSControls controls;
    private Camera mainCamera;

    [SerializeField]  private UnitController selectedUnit;
    [SerializeField]  private bool isSequencing;

    //For showing movement previews
    [SerializeField] private bool isRevealing;
    public static event Action<bool> RevealChanged;
    //For placement preview
    [SerializeField] private bool isInteractHeld;
    [SerializeField] private bool isPlacementPreviewActive;
    private Vector2 interactDestination;
    private float previewAngle;

    private void Awake()
    {
        controls = new RTSControls();
        mainCamera = Camera.main;

        // Subscribe to selection input.
        controls.Player.Select.performed += OnSelect;

        // Subscribe to interaction input.
        controls.Player.Interact.performed += OnInteract;
        controls.Player.Interact.canceled += OnInteractCanceled;

        // Track Shift being pressed and released.
        controls.Player.Sequence.started += OnSequenceStarted;
        controls.Player.Sequence.canceled += OnSequenceCanceled;

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
        //For placement preview
        if (!isInteractHeld || !isPlacementPreviewActive || selectedUnit == null)
            return;

        Vector2 mousePosition = GetMouseWorldPosition();
        Vector2 direction = mousePosition - interactDestination;

        if (direction.sqrMagnitude > 0.0001f)
        {
            previewAngle =
                Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        }

        selectedUnit.GetComponent<UnitVisuals>()
            .UpdatePlacementPreview(interactDestination, previewAngle);
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
    private void OnSelect(InputAction.CallbackContext context)
    {
        //check for colliders on in this world position
        Vector2 worldPosition = GetMouseWorldPosition();
        Collider2D hit = Physics2D.OverlapPoint(worldPosition);

        if (hit != null)
        {
            //if the collider is a unit
            UnitController unit = hit.GetComponentInParent<UnitController>();

            if (unit != null)
            {
                SetSelectedUnit(unit);
                Debug.Log("Selected: " + unit.name);
                return;
            }
        }

        SetSelectedUnit(null);
        Debug.Log("Unit deselected");
    }

    //setting a selected unit
    private void SetSelectedUnit(UnitController newUnit)
    {
        //hide the selection outline for old selected unit
        if (selectedUnit != null)
        {
            UnitVisuals oldVisuals =
                selectedUnit.GetComponent<UnitVisuals>();

            oldVisuals.HideOutline();
        }

        //setting new selected unit and revealing selection visual
        selectedUnit = newUnit;

        if (selectedUnit != null)
        {
            UnitVisuals newVisuals =
                selectedUnit.GetComponent<UnitVisuals>();

            newVisuals.ShowOutline();
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (selectedUnit == null)
            return;

        //check for if its a hold or tap action
        //hold
        if (context.interaction is UnityEngine.InputSystem.Interactions.HoldInteraction)
        {
            // Begin placement preview.
            // The rest happens in update
            interactDestination = GetMouseWorldPosition();

            previewAngle = selectedUnit.transform.eulerAngles.z;

            isInteractHeld = true;
            isPlacementPreviewActive = true;

            selectedUnit.GetComponent<UnitVisuals>()
                .ShowPlacementPreview(interactDestination, previewAngle);
        }
        //tap
        else if (context.interaction is UnityEngine.InputSystem.Interactions.TapInteraction)
        {
            // Normal quick-click movement for unit.
            Vector2 destination = GetMouseWorldPosition();

            // Cancel any previously requested final rotation.
            selectedUnit.ClearFinalRotation();

            //If its a sequence movement queue else just move
            if (isSequencing)
                selectedUnit.QueueMove(destination);
            else
                selectedUnit.MoveTo(destination);
        }
    }

    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        if (!isInteractHeld || selectedUnit == null)
            return;

        selectedUnit.GetComponent<UnitVisuals>().HidePlacementPreview();

        // Give the unit its movement order.
        if (isSequencing)
            selectedUnit.QueueMove(interactDestination);
        else
            selectedUnit.MoveTo(interactDestination);

        // Rotate to the preview angle after finishing the route.
        selectedUnit.SetFinalRotation(previewAngle);

        isInteractHeld = false;
        isPlacementPreviewActive = false;
    }


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
}
