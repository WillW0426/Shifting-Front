using UnityEngine;

//Handles how units use up resources, and toggling resource usage on units
public class UnitResourceUsage : MonoBehaviour
{
    [SerializeField] private float foodUsage = 1f;

    [SerializeField] private float woodUsage = 0f;
    [SerializeField] private float metalUsage = 0f;

    private bool isHealing = false;

    private void Start()
    {
        ResourceManager.Instance.AddResourceUsage(ResourceType.Food, foodUsage);
    }

    public void SetHealing(bool healing)
    {
        if (isHealing == healing)
            return;

        isHealing = healing;

        if (isHealing)
        {
            ResourceManager.Instance.AddResourceUsage(
                ResourceType.Wood, woodUsage
            );

            ResourceManager.Instance.AddResourceUsage(
                ResourceType.Metal, metalUsage
            );
        }
        else
        {
            ResourceManager.Instance.RemoveResourceUsage(
                ResourceType.Wood, woodUsage
            );

            ResourceManager.Instance.RemoveResourceUsage(
                ResourceType.Metal, metalUsage
            );
        }
    }

    public bool CanHeal()
    {
        return ResourceManager.Instance.IsResourceAvailable(ResourceType.Food)
        && (woodUsage <= 0 ||
            ResourceManager.Instance.IsResourceAvailable(ResourceType.Wood))
        && (metalUsage <= 0 ||
            ResourceManager.Instance.IsResourceAvailable(ResourceType.Metal));
    }

    private void OnDestroy()
    {
        if (ResourceManager.Instance == null)
            return;

        ResourceManager.Instance.RemoveResourceUsage(
            ResourceType.Food, foodUsage
        );

        if (isHealing)
        {
            ResourceManager.Instance.RemoveResourceUsage(
                ResourceType.Wood, woodUsage
            );

            ResourceManager.Instance.RemoveResourceUsage(
                ResourceType.Metal, metalUsage
            );
        }
    }
}
