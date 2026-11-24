using UnityEngine;

public class UnitStats : MonoBehaviour
{
    [SerializeField] private float attack = 5;
    [SerializeField] private float defense = 5;
    [SerializeField] private float speed = 5;
    [SerializeField] private float mobility = 5;
    [SerializeField] private float health = 100;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float morale = 100;
    [SerializeField] private float hunger = 100;

    [SerializeField] private float hungerMultiplier = 0.75f;
    [SerializeField] private bool isHungry = false;

    private void OnEnable()
    {
        ResourceManager.OnHungerChanged += SetHungry;

        if (ResourceManager.Instance != null)
        {
            SetHungry(ResourceManager.Instance.IsHungry());
        }
    }

    private void OnDisable()
    {
        ResourceManager.OnHungerChanged -= SetHungry;
    }

    public float GetSpeed()
    {
        return speed * GetHungerMultiplier(); ;
    }

    public float GetMobility()
    {
        return mobility * GetHungerMultiplier(); ;
    }

    public float GetHealth()
    {
        return health;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetMorale()
    {
        return morale;
    }

    public float GetHunger()
    {
        return hunger;
    }

    public float GetAttack()
    {
        return attack * GetHungerMultiplier(); ;
    }

    public float GetDefense()
    {
        return defense * GetHungerMultiplier(); ;
    }

    public float GetHealthPercentage()
    {
        return Mathf.Clamp01(health / maxHealth);
    }

    public void TakeDamage(float damage)
    {
        health = Mathf.Max(0, health - damage);

        if (health <= 0f)
        {
            Die();
            return;
        }

        GetComponent<UnitHealing>()?.ResetHealingTimer();
    }

    private void Die()
    {
        Destroy(transform.parent.gameObject);
    }
    public void Heal(float amount)
    {
        if (amount <= 0f)
            return;

        health = Mathf.Min(maxHealth, health + amount);
    }

    public void SetHealth(float amount)
    {
        health = Mathf.Clamp(amount, 0f, maxHealth);
    }

    private void SetHungry(bool hungry)
    {
        isHungry = hungry;
    }

    private float GetHungerMultiplier()
    {
        return isHungry ? hungerMultiplier : 1f;
    }
}
