using UnityEngine;
using TMPro;

/// <summary>
/// Controls behaviour of user HUD
/// </summary>
public class UIUpdater : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text goldText;

    void Start()
    {
        UpdateScore();
        UpdateGold();
    }

    public void AddPoints(int points)
    {
        if (points > 0) GameData.Score += points;
        UpdateScore();
    }

    public void DropPoints(int points)
    {
        if (points > 0) GameData.Score -= points;
        UpdateScore();
    }

    public void AddGold(int amount)
    {
        if (amount > 0) GameData.Gold += amount;
        UpdateGold();
    }

    public void DropGold(int amount)
    {
        if (amount > 0) GameData.Gold -= amount;
        UpdateGold();
    }

    private void UpdateScore()
    {
        scoreText.text = "Score: " + GameData.Score;
    }

    private void UpdateGold()
    {
        goldText.text = "Gold: " + GameData.Gold;
    }
}
