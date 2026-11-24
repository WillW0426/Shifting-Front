using UnityEngine;

public class CombatZone : MonoBehaviour
{
    //Handles detecting which side an enemy has started attacking.
    [SerializeField] private CombatSide side;

    private CombatController owner;

    private void Awake()
    {
        owner = GetComponentInParent<CombatController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CombatController enemy =
            other.GetComponentInParent<CombatController>();

        if (enemy != null && enemy != owner)
        {
            enemy.AddEngagement(owner, side);
            Debug.Log($"{enemy.name} entered {owner.name}'s {side} zone");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        CombatController enemy =
            other.GetComponentInParent<CombatController>();

        if (enemy != null && enemy != owner)
        {
            Debug.Log($"{enemy.name} exited {owner.name}'s {side} zone");
            enemy.RemoveEngagement(owner, side);
        }
    }
}
