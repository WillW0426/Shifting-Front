using UnityEngine;

//Handles Healing units
public class UnitHealing : MonoBehaviour
{
    [SerializeField] private float healingDelay = 5f;
    [SerializeField] private float healingAmount = 5f;
    [SerializeField] private float healingInterval = 2f;

    private float nextHealTime;

    private UnitStats stats;
    private CombatController combat;
    private UnitResourceUsage resourceUsage;

    private float lastCombatTime;
    [SerializeField] private bool isHealing;

    private void Awake()
    {
        stats = GetComponent<UnitStats>();
        combat = GetComponent<CombatController>();
        resourceUsage = GetComponent<UnitResourceUsage>();
    }

    private void Start()
    {
        lastCombatTime = Time.time;
    }

    private void Update()
    {
        if (combat.IsInCombat())
        {
            lastCombatTime = Time.time;
            StopHealing();
            return;
        }

        if (stats.GetHealth() >= stats.GetMaxHealth())
        {
            StopHealing();
            return;
        }

        if (Time.time - lastCombatTime < healingDelay)
        {
            StopHealing();
            return;
        }

        StartHealing();

        if (Time.time >= nextHealTime)
        {
            if (resourceUsage.CanHeal())
            {
                stats.Heal(healingAmount);
            }

            nextHealTime = Time.time + healingInterval;
        }

        if (stats.GetHealth() >= stats.GetMaxHealth())
        {
            StopHealing();
        }
    }

    private void StartHealing()
    {
        if (isHealing)
            return;

        isHealing = true;
        nextHealTime = Time.time + healingInterval;

        resourceUsage.SetHealing(true);
    }

    private void StopHealing()
    {
        if (!isHealing)
            return;

        isHealing = false;
        resourceUsage.SetHealing(false);
    }

    public bool IsHealing()
    {
        return isHealing;
    }

    public void ResetHealingTimer()
    {
        lastCombatTime = Time.time;
        StopHealing();
    }

    private void OnDisable()
    {
        StopHealing();
    }
}
