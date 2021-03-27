using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private int initialGold = 100;

    private int _score;
    private int _gold;

    public int Gold => _gold;

    void Start()
    {
        _gold = initialGold;
        UpdateScore();
        UpdateGold();
    }

    public void AddPoints(int points)
    {
        if (points > 0) _score += points;
        UpdateScore();
    }

    public void DropPoints(int points)
    {
        if (points > 0) _score -= points;
        UpdateScore();
    }

    public void AddGold(int amount)
    {
        if (amount > 0) _gold += amount;
        UpdateGold();
    }

    public void DropGold(int amount)
    {
        if (amount > 0) _gold -= amount;
        UpdateGold();
    }

    private void UpdateScore()
    {
        scoreText.text = "Score: " + _score;
    }

    private void UpdateGold()
    {
        goldText.text = "Gold: " + _gold;
    }
}
