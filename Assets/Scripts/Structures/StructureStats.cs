using UnityEngine;

public class StructureStats : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    [SerializeField] private int level = 1;
    [SerializeField] private float healthScaling = 0.25f;
    [SerializeField] private float productionScaling = 0.25f;

    
    [SerializeField] private float baseHealth = 100f;

    [SerializeField] private float currentHealth;
    private float maxHealth;

    [SerializeField] private ResourceType repairResource;
    [SerializeField] private ResourceType upgradeResource;

    [SerializeField] private float upgradeAmount = 50f;
    [SerializeField] private float upgradeCostScaling = 0.25f;

    private void Awake()
    {
        level = Mathf.Max(1, level);

        maxHealth = GetScaledHealth();
        currentHealth = maxHealth;
    }

    public float GetScaledHealth()
    {
        return baseHealth * (1f + healthScaling * (level - 1));
    }

    public float GetProductionMultiplier()
    {
        return 1f + productionScaling * (level - 1);
    }

    public float GetHealthPercentage()
    {
        return maxHealth > 0f ? currentHealth / maxHealth : 0f;
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0f, currentHealth - damage);

        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void SetLevel(int newLevel)
    {
        float healthPercentage = GetHealthPercentage();

        level = Mathf.Max(1, newLevel);
        maxHealth = GetScaledHealth();

        currentHealth = maxHealth * healthPercentage;
        ResourceStructure.NotifyStructureChanged();
    }

    public int GetLevel()
    {
        return level;
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetRepairCost()
    {
        return Mathf.Max(0f, maxHealth - currentHealth);
    }

    public void Repair()
    {
        float cost = GetRepairCost();

        if (cost <= 0f)
            return;

        if (!ResourceManager.Instance.SpendResources(repairResource, cost))
        {
            return;
        }

        currentHealth = maxHealth;
    }

    public float GetUpgradeCost()
    {
        return upgradeAmount * (1f + upgradeCostScaling * (level - 1));
    }

    public void Upgrade()
    {
        float cost = GetUpgradeCost();

        if (!ResourceManager.Instance.SpendResources(upgradeResource, cost))
        {
            return;
        }

        SetLevel(level + 1);
    }

    public ResourceType GetRepairResource()
    {
        return repairResource;
    }

    public ResourceType GetUpgradeResource()
    {
        return upgradeResource;
    }
}
