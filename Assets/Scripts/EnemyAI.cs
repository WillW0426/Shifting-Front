using UnityEngine;

public enum EnemyTarget
{
    Structure,
    Unit
}

[RequireComponent(typeof(UnitPathFinding))]
public class EnemyAI : MonoBehaviour
{  
    [SerializeField] private EnemyTarget target;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float updateInterval = 1f;

    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private LayerMask structureLayer;

    private UnitPathFinding pathfinding;
    private CombatController combatController;

    private Transform currentTarget;
    private float updateTimer;

    private void Awake()
    {
        pathfinding = GetComponent<UnitPathFinding>();
        combatController = GetComponent<CombatController>();
    }

    private void Update()
    {
        updateTimer += Time.deltaTime;

        if (updateTimer < updateInterval)
            return;

        updateTimer = 0f;

        if (combatController != null && combatController.IsInCombat())
            return;

        FindTarget();

        if (currentTarget != null)
        {
            pathfinding.MoveTo(currentTarget.position);
        }
    }

    public void SetTarget(EnemyTarget newTarget)
    {
        target = newTarget;
    }

    private void FindTarget()
    {
        currentTarget = null;

        // Unit targeting prioritize nearby enemy units
        if (target == EnemyTarget.Unit)
        {
            Collider2D[] nearbyUnits = Physics2D.OverlapCircleAll(
                transform.position,
                detectionRadius,
                unitLayer
            );

            float closestDistance = Mathf.Infinity;

            foreach (Collider2D unit in nearbyUnits)
            {
                if (unit.transform.root == transform)
                    continue;

                float distance = Vector2.Distance(
                    transform.position,
                    unit.transform.position
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    currentTarget = unit.transform;
                }
            }
        }

        // If no nearby unit was found, target a structure
        if (currentTarget == null)
        {
            Collider2D[] structures = Physics2D.OverlapCircleAll(
                transform.position,
                Mathf.Infinity,
                structureLayer
            );

            float closestDistance = Mathf.Infinity;

            foreach (Collider2D structure in structures)
            {
                float distance = Vector2.Distance(
                    transform.position,
                    structure.transform.position
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    currentTarget = structure.transform;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (target == EnemyTarget.Unit)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
