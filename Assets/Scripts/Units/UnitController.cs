using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    private Rigidbody2D rb;
    private UnitStats stats;
    private Vector2 moveDestination;
    private bool hasDestination;

    private bool hasFinalRotation = false;
    private float finalRotation;
    private bool canPerformFinalRotation = false;

    private float speed;
    private float mobility;

    private Queue<Vector2> destinationQueue = new Queue<Vector2>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<UnitStats>();
        speed = stats.GetSpeed();
        mobility = stats.GetMobility();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveTo(Vector2 newDestination)
    {
        destinationQueue.Clear();

        moveDestination = newDestination;
        hasDestination = true;
    }

    // Add a destination to the end of the queue.
    public void QueueMove(Vector2 destination)
    {
        destinationQueue.Enqueue(destination);

        // If the unit is idle, start moving to the first queued point.
        if (!hasDestination)
        {
            MoveToNextDestination();
        }
    }

    private void MoveToNextDestination()
    {
        if (destinationQueue.Count > 0)
        {
            moveDestination = destinationQueue.Dequeue();
            hasDestination = true;
        }
        else
        {
            hasDestination = false;

            if (hasFinalRotation)
            {
                canPerformFinalRotation = true;
            }
        }
    }

    public void SetFinalRotation(float angle)
    {
        finalRotation = angle;
        hasFinalRotation = true;
    }

    public void ClearFinalRotation()
    {
        hasFinalRotation = false;
        canPerformFinalRotation = false;
    }

    private void FixedUpdate()
    {
        // If there is no movement left, perform the final rotation.
        if (!hasDestination)
        {
            if (canPerformFinalRotation && hasFinalRotation)
            {
                float newAngle = Mathf.MoveTowardsAngle(
                    rb.rotation,
                    finalRotation,
                    mobility * 30f * Time.fixedDeltaTime
                );

                rb.MoveRotation(newAngle);

                // Check whether we have reached the final rotation.
                if (Mathf.Abs(Mathf.DeltaAngle(newAngle, finalRotation)) < 1f)
                {
                    rb.MoveRotation(finalRotation);

                    hasFinalRotation = false;
                    canPerformFinalRotation = false;
                }
            }

            return;
        }

        Vector2 currentPosition = rb.position;
        Vector2 direction = moveDestination - currentPosition;

        if (direction.magnitude < 0.1f)
        {
            rb.MovePosition(moveDestination);
            MoveToNextDestination();
            return;
        }

        // Calculate the direction the unit needs to face.
        float targetAngle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // Rotate gradually toward the movement direction.
        float newMovementAngle = Mathf.MoveTowardsAngle(
            rb.rotation,
            targetAngle,
            mobility * 30f * Time.fixedDeltaTime
        );

        rb.MoveRotation(newMovementAngle);

        // Don't move until the unit is facing its destination.
        if (Mathf.Abs(Mathf.DeltaAngle(newMovementAngle, targetAngle)) > 1f)
            return;

        Vector2 nextPosition = currentPosition
            + direction.normalized * speed * Time.fixedDeltaTime;

        rb.MovePosition(nextPosition);
    }

    public List<Vector2> GetPlannedRoute()
    {
        List<Vector2> route = new List<Vector2>();

        // Add the current destination first.
        if (hasDestination)
        {
            route.Add(moveDestination);
        }

        // Then add the remaining queued destinations.
        route.AddRange(destinationQueue);

        return route;
    }

    public float GetPlannedFinalAngle()
    {
        // If a final rotation has been specified, use it.
        if (hasFinalRotation)
            return finalRotation;

        List<Vector2> route = GetPlannedRoute();

        if (route.Count == 0)
            return rb.rotation;

        // The final destination.
        Vector2 finalPosition = route[route.Count - 1];

        // The position immediately before the final destination.
        Vector2 previousPosition;

        if (route.Count >= 2)
        {
            previousPosition = route[route.Count - 2];
        }
        else
        {
            // If there is only one destination, use the unit's
            // current position as the starting point.
            previousPosition = rb.position;
        }

        Vector2 direction = finalPosition - previousPosition;

        if (direction.sqrMagnitude < 0.0001f)
            return rb.rotation;

        // The unit's forward direction is positive Y.
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
    }
}
