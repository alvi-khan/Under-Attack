using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Turret Stats")]
    [SerializeField] private int cost = 50;
    [SerializeField] private int health = 100;
    [SerializeField] private int maxShield = 100;
    [SerializeField] private int startingShield = 0;
    [SerializeField] private int damagePerShot = 20;
    [Tooltip("Points gained when hitting enemy or lost when hit by enemy.")]
    [SerializeField] private int pointsPerHit = 50;
    [Tooltip("Points gained when killing enemy or lost when killed by enemy.")]
    [SerializeField] private int pointsOnDeath = 100;
    [Tooltip("Gold gained when killing enemy or lost when killed by enemy.")]
    [SerializeField] private int goldOnDeath;

    [Header("Effects")]
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private RectTransform shieldBar;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private AudioClip explosionSFX;
    [SerializeField] private float deathDelay = 1f;

    private GridTile _gridOccupied;
    private List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();
    private UIUpdater _uiUpdater;
    private int _currentHealth;
    private int _currentShield;
    private AudioSource _deathAudio;

    public bool Placed { get; set; }
    public GridTile GridOccupied { set => _gridOccupied = value; }
    public int DamagePerShot => damagePerShot;
    public int PointsPerHit => pointsPerHit;
    public int Cost => cost;

    void Start()
    {
        _currentHealth = health;
        _currentShield = startingShield;
        _uiUpdater = FindObjectOfType<UIUpdater>();
        _deathAudio = GetComponent<AudioSource>();
        foreach (Transform child in transform.parent)
        {
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
            if (meshRenderer != null)   _meshRenderers.Add(meshRenderer);
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage < 0) return;
        if (_currentShield > 0)
        {
            _currentShield -= damage;
            if (_currentShield < 0)
            {
                _currentHealth += _currentShield;
                _currentShield = 0;
            }
        }
        else
        {
            _currentHealth -= damage;
        }
        UpdateHealthBar();
        if (_currentHealth <= 0) StartCoroutine(Kill());
    }

    IEnumerator Kill()
    {
        if (CompareTag("Player"))   _uiUpdater.DropPoints(pointsOnDeath); // player turret died
        else
        {
            _uiUpdater.AddPoints(pointsOnDeath);                          // enemy turret died
            _uiUpdater.AddGold(goldOnDeath);
        }

        DeathEffects();
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject.transform.root.gameObject);
    }

    void DeathEffects()
    {
        explosionVFX.SetActive(true);
        _deathAudio.PlayOneShot(explosionSFX);
        healthBar.parent.parent.gameObject.SetActive(false);
        _gridOccupied.Occupied = false;
        _gridOccupied.Turret = null;
        Placed = false;
        foreach (MeshRenderer meshRenderer in _meshRenderers) meshRenderer.enabled = false;
    }

    void UpdateHealthBar()
    {
        if (healthBar != null) healthBar.sizeDelta = new Vector2(_currentHealth * 100 / health, healthBar.sizeDelta.y);
        if (shieldBar != null) shieldBar.sizeDelta = new Vector2(_currentShield * 100 / maxShield, shieldBar.sizeDelta.y);
    }

    public void RepairTurret(int healthIncrease, int repairCost)
    {
        if (_currentShield != 0 || _currentHealth == health) return;
        _currentHealth += healthIncrease;
        if (_currentHealth > health) _currentHealth = health;
        _uiUpdater.DropGold(repairCost);
        UpdateHealthBar();
    }

    public void ShieldTurret(int shieldAmount, int shieldCost)
    {
        if (_currentShield == maxShield) return;
        _currentShield += shieldAmount;
        if (_currentShield > maxShield) _currentShield = maxShield;
        _currentHealth = health;
        _uiUpdater.DropGold(shieldCost);
        UpdateHealthBar();
    }
}
