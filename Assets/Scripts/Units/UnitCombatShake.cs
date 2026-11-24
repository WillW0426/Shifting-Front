using UnityEngine;

public class UnitCombatShake : MonoBehaviour
{
    [SerializeField] private float shakeAmount = 0.05f;
    [SerializeField] private float shakeSpeed = 20f;

    private CombatController combatController;
    private Vector3 originalPosition;

    private void Awake()
    {
        combatController = GetComponentInParent<CombatController>();
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (combatController.IsInCombat())
        {
            float x = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            float y = Mathf.Cos(Time.time * shakeSpeed * 1.3f) * shakeAmount;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);
        }
        else
        {
            transform.localPosition = originalPosition;
        }
    }
}
