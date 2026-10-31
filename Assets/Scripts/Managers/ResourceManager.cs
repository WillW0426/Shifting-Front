using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private Faction owner;
    [SerializeField] private float generationInterval = 5;
    [SerializeField] private List<ResourceStructure> ownedStructures = new List<ResourceStructure>();
    [SerializeField] Dictionary<ResourceType, float> resourceAmount;
    [SerializeField] Dictionary<ResourceType, float> resourceGains;
    [SerializeField] Dictionary<ResourceType, float> resourcesUsage;
    private float nextGenerateTime;

    private void Awake()
    {
        resourceAmount = CreateResourceDictionary();
        resourceGains = CreateResourceDictionary();
        resourcesUsage = CreateResourceDictionary();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float startTime = Time.time;
        nextGenerateTime = startTime + generationInterval;

        GetOwnedStructures();
        ReCalculateEconomy();
    }

    // Update is called once per frame
    void Update()
    {
        //Check when to generate
        if(Time.time >= nextGenerateTime)
        {
            foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
            {
                resourceAmount[type] += resourceGains[type];
            }

            //Debug
            foreach (ResourceType type in resourceAmount.Keys)
            {
                Debug.Log($"Amount {owner} {type}: {resourceAmount[type]}");              
            }
            Debug.Log("=====================================================");
            //

            nextGenerateTime += generationInterval;
        }
    }
    public Faction GetOwner()
    {
        return owner;
    }

    //Checks all owned resource structures and recalculates gains and usage, this should be called whenever a structure is changed such as ownership change or level up etc
    private void ReCalculateEconomy()
    {
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            resourceGains[type] = 0;
        }

        foreach (ResourceStructure structure in ownedStructures)
        {
            ResourceType type = structure.GetType();
            float amount = structure.GetAmount();

            resourceGains[type] += amount;
        }

        //Debug
        foreach (ResourceType type in resourceGains.Keys)
        {
            Debug.Log($"Gains: {owner} {type}: {resourceGains[type]}");
        }
        Debug.Log("=====================================================");
        //
    }

    //Called at the start, gets all owned structures and puts them in a 
    private void GetOwnedStructures()
    {
        ResourceStructure[] structures = FindObjectsByType<ResourceStructure>(FindObjectsSortMode.None);

        foreach (ResourceStructure structure in structures)
        {
            if (structure.GetFaction() == owner)
            {
                ownedStructures.Add(structure);
            }
        }
    }
    public void AddStructure(ResourceStructure structure)
    {
        if (!ownedStructures.Contains(structure))
        {
            ownedStructures.Add(structure);
            ReCalculateEconomy();
        }
    }

    public void RemoveStructure(ResourceStructure structure)
    {
        ownedStructures.Remove(structure);
        ReCalculateEconomy();
    }

    private Dictionary<ResourceType, float> CreateResourceDictionary()
    {
        return new Dictionary<ResourceType, float>
    {
        { ResourceType.Wood, 0 },
        { ResourceType.Food, 0 },
        { ResourceType.Stone, 0 },
        { ResourceType.Metal, 0 },
        { ResourceType.Horses, 0 }
    };
    }
}
