using UnityEngine;
using System;

public class ResourceStructure : MonoBehaviour
{
    [SerializeField] private float generationAmount = 1;
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private Faction faction;

    private StructureStats stats;

    public static event Action OnStructureChanged;

    private void Awake()
    {
        stats = GetComponent<StructureStats>();
    }

    public ResourceType GetType()
    {
        return resourceType;
    }

    public float GetAmount()
    {
        return generationAmount * stats.GetProductionMultiplier();
    }

    public Faction GetFaction()
    {
        return faction;
    }

    public void SetFaction(Faction newFaction)
    {
        faction = newFaction;
    }

    //event
    public static void NotifyStructureChanged()
    {
        OnStructureChanged?.Invoke();
    }

    private void OnDestroy()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.RemoveStructure(this);
        }
    }
}
