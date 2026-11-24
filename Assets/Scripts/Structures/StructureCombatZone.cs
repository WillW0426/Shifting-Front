using UnityEngine;

//Handles detecting enemies attacking the structure
public class StructureCombatZone : MonoBehaviour
{
    private StructureStats structure;

    private void Awake()
    {
        structure = GetComponentInParent<StructureStats>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CombatController enemy = other.GetComponentInParent<CombatController>();

        if (enemy != null && enemy.GetComponent<EnemyAI>() != null)
        {
            enemy.EngageStructure(structure);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        CombatController enemy = other.GetComponentInParent<CombatController>();

        if (enemy != null)
        {
            enemy.DisengageStructure(structure);
        }
    }
}
