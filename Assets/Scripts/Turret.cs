using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to control behaviour of turrets.
/// </summary>
public class Turret : MonoBehaviour
{
    [Header("Turret Stats")]
    [SerializeField] private int cost = 50;
    [SerializeField] private int health = 100;
    [SerializeField] private int maxShield = 100;
    [SerializeField] private int startingShield;
    [SerializeField] private int damagePerShot = 20;
    [Tooltip("Points gained (enemy turret) or lost (player turret).")]
    [SerializeField] private int pointsPerHit = 50;
    [Tooltip("Points gained (enemy turret) or lost (player turret).")]
    [SerializeField] private int pointsOnDeath = 100;
    [Tooltip("Gold gained (enemy turret) or lost (player turret).")]
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
        if (damage < 0) return; // this...shouldn't be happening
        if (_currentShield > 0)
        {
            _currentShield -= damage;
            if (_currentShield < 0)
            {
                _currentHealth += _currentShield;
                _currentShield = 0;
            }
        }
        else _currentHealth -= damage;

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

        _gridOccupied.Occupied = false;
        _gridOccupied.Turret = null;
        Placed = false;

        DeathEffects();
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject.transform.root.gameObject);
    }

    /// <summary>
    /// Effects that are shown when turret is destroyed.
    /// </summary>
    void DeathEffects()
    {
        explosionVFX.SetActive(true);
        _deathAudio.PlayOneShot(explosionSFX);
        healthBar.parent.parent.gameObject.SetActive(false);
        foreach (MeshRenderer meshRenderer in _meshRenderers) meshRenderer.enabled = false;
    }

    void UpdateHealthBar()
    {
        Vector2 newHealthBar = new Vector2(_currentHealth * 100 / health, healthBar.sizeDelta.y);
        Vector2 newShieldBar = new Vector2(_currentShield * 100 / maxShield, shieldBar.sizeDelta.y);
        if (healthBar != null) healthBar.sizeDelta = newHealthBar;
        if (shieldBar != null) shieldBar.sizeDelta = newShieldBar;
    }

    public void RepairTurret(int healthIncrease, int repairCost)
    {
        if (_currentShield != 0 || _currentHealth == health) return;    // nothing to repair

        _currentHealth += healthIncrease;
        if (_currentHealth > health) _currentHealth = health;

        _uiUpdater.DropGold(repairCost);
        UpdateHealthBar();
    }

    public void ShieldTurret(int shieldAmount, int shieldCost)
    {
        if (_currentShield == maxShield) return;    // nothing to shield

        _currentShield += shieldAmount;
        if (_currentShield > maxShield) _currentShield = maxShield;
        _currentHealth = health;    // shield restores health (todo does thing make sense?)

        _uiUpdater.DropGold(shieldCost);
        UpdateHealthBar();
    }
}
