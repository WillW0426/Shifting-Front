using UnityEngine;

public class Fortress : MonoBehaviour
{
    [Header("Unit Prefabs")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private GameObject swordShieldPrefab;
    [SerializeField] private GameObject spearPrefab;
    [SerializeField] private GameObject spearShieldPrefab;
    [SerializeField] private GameObject cavalryPrefab;

    [Header("Spawning")]
    [SerializeField] private Transform spawnPoint;

    public void SpawnSword()
    {
        SpawnUnit(swordPrefab);
    }

    public void SpawnSwordShield()
    {
        SpawnUnit(swordShieldPrefab);
    }

    public void SpawnSpear()
    {
        SpawnUnit(spearPrefab);
    }

    public void SpawnSpearShield()
    {
        SpawnUnit(spearShieldPrefab);
    }

    public void SpawnCavalry()
    {
        SpawnUnit(cavalryPrefab);
    }

    private void SpawnUnit(GameObject prefab)
    {
        GameObject unit = Instantiate(
            prefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        UnitStats stats = unit.GetComponentInChildren<UnitStats>();
        stats.SetHealth(1f);
    }
}
