using TMPro;
using UnityEngine;

//Manages the Wave time part of the player HUD
public class WaveHUD : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text timerText;

    private void Update()
    {
        float timeRemaining = enemySpawner.GetTimeUntilNextWave();

        int totalSeconds = Mathf.CeilToInt(timeRemaining);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        waveText.text = $"Wave {enemySpawner.GetNextWave()}";
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
