using UnityEngine;

public class UnitStats : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private float mobility = 5;
    [SerializeField] private float health = 100;
    [SerializeField] private float morale = 100;
    [SerializeField] private float hunger = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetMobility()
    {
        return mobility;
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMorale()
    {
        return morale;
    }

    public float GetHunger()
    {
        return hunger;
    }
}
