using UnityEngine;

public class ResourceStructure : MonoBehaviour
{
    [SerializeField] private float generationAmount = 1;
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private Faction faction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ResourceType GetType()
    {
        return resourceType;
    }

    public float GetAmount()
    {
        return generationAmount;
    }

    public Faction GetFaction()
    {
        return faction;
    }

    public void SetFaction(Faction newFaction)
    {
        faction = newFaction;
    }
}
