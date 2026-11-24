using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    private Dictionary<CombatController, CombatSide> engagedUnits = new Dictionary<CombatController, CombatSide>();
    private StructureStats engagedStructure;
    private UnitController unitController;
    private UnitStats stats;
    [SerializeField] private bool isInCombat = false;

    [SerializeField] private float combatInterval = 1f;
    private float combatTimer = 0f;

    // Handles Unit Combat
    void Start()
    {
        
    }

    private void Awake()
    {
        unitController = GetComponent<UnitController>();
        stats = GetComponent<UnitStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInCombat)
            return;

        combatTimer += Time.deltaTime;

        if (combatTimer >= combatInterval)
        {
            combatTimer -= combatInterval;
            PerformAttacks();
        }
    }

    public void AddEngagement(CombatController enemy, CombatSide side)
    {
        if (enemy == null || engagedUnits.ContainsKey(enemy))
            return;

        engagedUnits.Add(enemy, side);

        // Only stop movement when first entering combat
        if (!isInCombat)
        {
            isInCombat = true;
            combatTimer = 0f;
            unitController.StopMovement();
        }
    }

    public void RemoveEngagement(CombatController enemy, CombatSide side)
    {
        if (enemy == null)
            return;

        if (engagedUnits.TryGetValue(enemy, out CombatSide currentSide) && currentSide == side)
        {
            engagedUnits.Remove(enemy);

            // Combat ends when no enemies remain
            if (engagedUnits.Count == 0)
            {
                isInCombat = false;
                combatTimer = 0f;
            }
        }
    }

    public bool IsInCombat()
    {
        return isInCombat;
    }
    public void Attack(CombatController enemy, CombatSide side)
    {
        float attack = stats.GetAttack();
        float healthPercentage = stats.GetHealthPercentage();

        if (healthPercentage > 0.85f)
        {
            attack *= 1.25f;
        }
        else if (healthPercentage >= 0.70f)
        {
            attack *= 1f;
        }
        else if (healthPercentage >= 0.50f)
        {
            attack *= 0.9f;
        }
        else if (healthPercentage >= 0.30f)
        {
            attack *= 0.75f;
        }
        else
        {
            attack *= 0.5f;
        }

        enemy.Defend(attack, side);
    }

    public void Defend(float attack, CombatSide side)
    {
        float defense = stats.GetDefense();
        float mobility = stats.GetMobility();

        // Defense reduces incoming damage
        float defenseMultiplier = 100f / (100f + defense);

        // Higher mobility reduces directional vulnerability
        float directionalMultiplier = 1f;

        switch (side)
        {
            case CombatSide.Front:
                directionalMultiplier = 1f;
                break;

            case CombatSide.Side:
                directionalMultiplier = 1f + Mathf.Max(0.15f, 0.5f - mobility * 0.015f);
                break;

            case CombatSide.Back:
                directionalMultiplier = 1f + Mathf.Max(0.30f, 1f - mobility * 0.03f);
                break;
        }

        float damage = attack * defenseMultiplier * directionalMultiplier;

        stats.TakeDamage(damage);
    }

    private void PerformAttacks()
    {
        foreach (var engagement in engagedUnits)
        {
            CombatController enemy = engagement.Key;
            CombatSide side = engagement.Value;

            if (enemy == null)
                continue;

            Attack(enemy, side);
        }

        if (engagedStructure != null)
        {
            float damage = stats.GetAttack();
            engagedStructure.TakeDamage(damage);
        }
    }

    #region Structures
    public void EngageStructure(StructureStats structure)
    {
        if (structure == null)
            return;

        engagedStructure = structure;
        isInCombat = true;
        combatTimer = 0f;

        unitController.StopMovement();
    }

    public void DisengageStructure(StructureStats structure)
    {
        if (engagedStructure != structure)
            return;

        engagedStructure = null;

        if (engagedUnits.Count == 0)
        {
            isInCombat = false;
            combatTimer = 0f;
        }
    }
    #endregion
}
