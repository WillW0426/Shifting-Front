using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ResourceManager playerResourceManager;
    [SerializeField] private ResourceManager aiResourceManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeOwnership(ResourceStructure structure, Faction newOwner)
    {
        // Remove from old manager
        // Change structure's owner
        // Add to new manager
        // recalculate economies for both
    }
}
