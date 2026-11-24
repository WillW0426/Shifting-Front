using TMPro;
using UnityEngine;

//Selecting stuctures and the ui elements that appear with it
public class StructureSelection : MonoBehaviour
{
    [SerializeField] private GameObject actionsPanel;
    [SerializeField] private TMP_Text repairText;
    [SerializeField] private TMP_Text upgradeText;

    private StructureStats stats;

    private void Awake()
    {
        stats = GetComponent<StructureStats>();
        actionsPanel.SetActive(false);
    }

    private void Update()
    {
        if (actionsPanel.activeSelf)
        {
            UpdateButtonText();
        }
    }
    public void Select()
    {
        actionsPanel.SetActive(true);
        UpdateButtonText();
    }

    public void Deselect()
    {
        actionsPanel.SetActive(false);
    }

    public void Repair()
    {
        if (stats != null)
        {
            stats.Repair();
        }
    }

    public void Upgrade()
    {
        if (stats != null)
        {
            stats.Upgrade();
        }
    }

    private void UpdateButtonText()
    {
        repairText.text =
            $"Repair\n{stats.GetRepairCost():0.#} {stats.GetRepairResource()}";

        upgradeText.text =
            $"Upgrade\n{stats.GetUpgradeCost():0.#} {stats.GetUpgradeResource()}";
    }
}
