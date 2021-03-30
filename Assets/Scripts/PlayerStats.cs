using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{

    [SerializeField] private int initialGold = 100;

    private int _score;
    private int _gold;
    private TMP_Text _scoreText;
    private TMP_Text _goldText;

    public int Gold => _gold;

    void Awake()
    {
        int scriptCount = FindObjectsOfType<PlayerStats>().Length;

        if (scriptCount > 1)  Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        _gold += initialGold;
        UpdateScore();
        UpdateGold();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        GameObject score = null, gold = null;
        while (true)
        {
            score = GameObject.FindGameObjectWithTag("Score");
            gold = GameObject.FindGameObjectWithTag("Gold");
            if (score == null || gold == null) yield return new WaitForEndOfFrame();
            else
            {
                _scoreText = score.GetComponent<TMP_Text>();
                _goldText = gold.GetComponent<TMP_Text>();
                UpdateScore();
                UpdateGold();
                break;
            }
        }
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
        _scoreText.text = "Score: " + _score;
    }

    private void UpdateGold()
    {
        _goldText.text = "Gold: " + _gold;
    }
}
