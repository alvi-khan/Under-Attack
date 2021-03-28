using System.Collections;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private int health = 1000;
    [Tooltip("Points gained when hitting enemy or lost when hit by enemy.")]
    [SerializeField] private int pointsPerHit = 50;
    [Tooltip("Points gained when killing enemy or lost when killed by enemy.")]
    [SerializeField] private int pointsOnDeath = 1000;
    [Tooltip("Gold gained when killing enemy or lost when killed by enemy.")]
    [SerializeField] private int goldOnDeath;
    
    [Header("Effects")]
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private float deathDelay = 3f;

    private MeshRenderer[] _meshRenderers;
    private PlayerStats _playerStats;
    private int _currentHealth;
    private SceneManager _sceneManager;
    
    public int PointsPerHit => pointsPerHit;

    void Start()
    {
        _currentHealth = health;
        _playerStats = FindObjectOfType<PlayerStats>();
        _meshRenderers = transform.Find("Structure").GetComponentsInChildren<MeshRenderer>();
        _sceneManager = FindObjectOfType<SceneManager>();
    }

    private void OnParticleCollision(GameObject other)
    {
        Turret enemyTurret = other.transform.parent.GetComponent<Turret>();
        UpdateScore(pointsPerHit);
        TakeDamage(enemyTurret.DamagePerShot);
    }

    void UpdateScore(int points)
    {
        if (CompareTag("Player"))   _playerStats.DropPoints(points); // player got shot
        else _playerStats.AddPoints(points);                         // enemy got shot
    }

    public void TakeDamage(int damage)
    {
        if (damage < 0) return;
        _currentHealth -= damage;
        UpdateHealthBar();
        if (_currentHealth <= 0) StartCoroutine(Kill());
    }

    IEnumerator Kill()
    {
        tag = "Untagged";   // stops the shooting
        if (CompareTag("Player"))   _playerStats.DropPoints(pointsOnDeath); // player base destroyed
        else
        {
            _playerStats.AddPoints(pointsOnDeath); // enemy turret died
            _playerStats.AddGold(goldOnDeath);
        }
        DeathEffects();
        yield return new WaitForSeconds(deathDelay);
        _sceneManager.LoadNextScene();
    }

    void DeathEffects()
    {
        explosionVFX.SetActive(true);
        healthBar.parent.parent.gameObject.SetActive(false);
        foreach (MeshRenderer meshRenderer in _meshRenderers) meshRenderer.enabled = false;
    }

    void UpdateHealthBar()
    {
        if (healthBar != null) healthBar.sizeDelta = new Vector2(_currentHealth * 100 / health, healthBar.sizeDelta.y);
    }
}
