using UnityEngine;
using UnityEngine.UI;

public class UnitHealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;

    private UnitStats stats;

    private void Awake()
    {
        stats = GetComponentInParent<UnitStats>();
    }

    private void Update()
    {
        healthBarFill.fillAmount = stats.GetHealthPercentage();
    }

    
}
