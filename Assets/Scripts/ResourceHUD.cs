using UnityEngine;
using TMPro;

public class ResourceHUD : MonoBehaviour
{
    [Header("Resource Text")]
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text stoneText;
    [SerializeField] private TMP_Text metalText;

    private ResourceManager resourceManager;

    private void Start()
    {
        resourceManager = ResourceManager.Instance;
        UpdateResourceText();
    }

    private void Update()
    {
        UpdateResourceText();
    }

    private void UpdateResourceText()
    {
        if (resourceManager == null)
            return;

        woodText.text = $"Wood: {resourceManager.GetResourceAmount(ResourceType.Wood):0.#} ({resourceManager.GetResourceNetChange(ResourceType.Wood):+0.#;-0.#;0})";
        foodText.text = $"Food: {resourceManager.GetResourceAmount(ResourceType.Food):0.#} ({resourceManager.GetResourceNetChange(ResourceType.Food):+0.#;-0.#;0})";
        stoneText.text = $"Stone: {resourceManager.GetResourceAmount(ResourceType.Stone):0.#} ({resourceManager.GetResourceNetChange(ResourceType.Stone):+0.#;-0.#;0})";
        metalText.text = $"Metal: {resourceManager.GetResourceAmount(ResourceType.Metal):0.#} ({resourceManager.GetResourceNetChange(ResourceType.Metal):+0.#;-0.#;0})";
    }
}