using System.Collections;
using UnityEngine;

/// <summary>
/// Class to control behaviour of base camps.
/// </summary>
public class Base : MonoBehaviour
{
    [SerializeField] private int health = 1000;
    [Tooltip("Points gained (enemy base) or lost (player base).")]
    [SerializeField] private int pointsPerHit = 50;
    [Tooltip("Enemy base only.")]
    [SerializeField] private int pointsOnDeath = 1000;
    [Tooltip("Enemy base only.")]
    [SerializeField] private int goldOnDeath;
    
    [Header("Effects")]
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private AudioClip shotSFX;
    [SerializeField] private float deathDelay = 3f;

    private MeshRenderer[] _meshRenderers;
    private UIUpdater _uiUpdater;
    private int _currentHealth;
    private AudioSource _deathAudio;
    private SceneManager _sceneManager;

    void Start()
    {
        _currentHealth = health;
        _uiUpdater = FindObjectOfType<UIUpdater>();
        // only getting the structure, not the ground beneath
        _meshRenderers = transform.Find("Structure").GetComponentsInChildren<MeshRenderer>();
        _deathAudio = GetComponent<AudioSource>();
        _sceneManager = FindObjectOfType<SceneManager>();
    }

    /// <summary>
    /// Events that occur when base camp gets shot.
    /// </summary>
    /// <param name="other">is the game object that shot the base camp.</param>
    private void OnParticleCollision(GameObject other)
    {
        Turret enemyTurret = other.transform.parent.GetComponent<Turret>();
        AudioSource audioSource = other.transform.root.GetComponentInChildren<AudioSource>();
        audioSource.PlayOneShot(shotSFX);
        UpdateScore(pointsPerHit);
        TakeDamage(enemyTurret.DamagePerShot);
    }

    void UpdateScore(int points)
    {
        if (CompareTag("Player"))   _uiUpdater.DropPoints(points); // player got shot
        else _uiUpdater.AddPoints(points);                         // enemy got shot
    }

    private void TakeDamage(int damage)
    {
        if (damage < 0) return; // this...shouldn't be happening
        _currentHealth -= damage;
        UpdateHealthBar();
        if (_currentHealth <= 0) StartCoroutine(Kill());
    }

    IEnumerator Kill()
    {
        if (CompareTag("Player"))
        {
            DeathEffects();
            yield return new WaitForSeconds(deathDelay);
            _sceneManager.EndGame();
        }
        else if (CompareTag("Enemy"))
        {
            _uiUpdater.AddPoints(pointsOnDeath);
            _uiUpdater.AddGold(goldOnDeath);
            DeathEffects();
            yield return new WaitForSeconds(deathDelay);
            _sceneManager.LoadNextScene();
        }
    }

    /// <summary>
    /// Effects that are shown when base camp is destroyed.
    /// </summary>
    void DeathEffects()
    {
        tag = "Untagged"; // stops turrets from shooting
        explosionVFX.SetActive(true);
        _deathAudio.PlayOneShot(_deathAudio.clip);
        // hide everything (destroying would destroy this script too)
        healthBar.parent.parent.gameObject.SetActive(false);
        foreach (MeshRenderer meshRenderer in _meshRenderers) meshRenderer.enabled = false;
    }

    void UpdateHealthBar()
    {
        Vector2 newHealthBar = new Vector2(_currentHealth * 100 / health, healthBar.sizeDelta.y);
        if (healthBar != null) healthBar.sizeDelta = newHealthBar;
    }
}
