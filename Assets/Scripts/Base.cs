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
    [SerializeField] private AudioClip shotSFX;
    [SerializeField] private float deathDelay = 3f;

    private MeshRenderer[] _meshRenderers;
    private UIUpdater _uiUpdater;
    private int _currentHealth;
    private AudioSource _deathAudio;
    private SceneManager _sceneManager;
    
    public int PointsPerHit => pointsPerHit;

    void Start()
    {
        _currentHealth = health;
        _uiUpdater = FindObjectOfType<UIUpdater>();
        _meshRenderers = transform.Find("Structure").GetComponentsInChildren<MeshRenderer>();
        _deathAudio = GetComponent<AudioSource>();
        _sceneManager = FindObjectOfType<SceneManager>();
    }

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
        if (CompareTag("Player"))   _uiUpdater.DropPoints(pointsOnDeath); // player base destroyed
        else
        {
            _uiUpdater.AddPoints(pointsOnDeath); // enemy turret died
            _uiUpdater.AddGold(goldOnDeath);
        }
        DeathEffects();
        yield return new WaitForSeconds(deathDelay);
        _sceneManager.LoadNextScene();
    }

    void DeathEffects()
    {
        explosionVFX.SetActive(true);
        _deathAudio.PlayOneShot(_deathAudio.clip);
        healthBar.parent.parent.gameObject.SetActive(false);
        foreach (MeshRenderer meshRenderer in _meshRenderers) meshRenderer.enabled = false;
    }

    void UpdateHealthBar()
    {
        if (healthBar != null) healthBar.sizeDelta = new Vector2(_currentHealth * 100 / health, healthBar.sizeDelta.y);
    }
}
