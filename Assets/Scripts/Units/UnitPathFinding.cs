using Pathfinding;
using UnityEngine;

public class UnitPathFinding : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Seeker seeker;
    private UnitController unitController;

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        unitController = GetComponent<UnitController>();
    }

    public void MoveTo(Vector2 destination)
    {
        seeker.StartPath(transform.position, destination, OnPathComplete);
    }

    private void OnPathComplete(Path path)
    {
        if (path.error)
        {
            Debug.LogWarning("Pathfinding failed: " + path.errorLog);
            return;
        }

        if (path.vectorPath.Count < 2)
            return;

        unitController.MoveTo(path.vectorPath[1]);

        for (int i = 2; i < path.vectorPath.Count; i++)
        {
            unitController.QueueMove(path.vectorPath[i]);
        }
    }
}
