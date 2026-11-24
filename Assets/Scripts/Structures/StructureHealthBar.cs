using UnityEngine;
using UnityEngine.UI;

public class StructureHealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;

    private StructureStats stats;

    private void Awake()
    {
        stats = GetComponentInParent<StructureStats>();
    }

    private void Update()
    {
        healthBarFill.fillAmount = stats.GetHealthPercentage();
    }
}
