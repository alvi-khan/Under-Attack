using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Turret Stats")]
    [SerializeField] private int cost = 50;
    [SerializeField] private int health = 100;
    [SerializeField] private int damagePerShot = 20;
    [Tooltip("Points gained when hitting enemy or lost when hit by enemy.")]
    [SerializeField] private int pointsPerHit = 50;
    [Tooltip("Points gained when killing enemy or lost when killed by enemy.")]
    [SerializeField] private int pointsOnDeath = 100;
    [Tooltip("Gold gained when killing enemy or lost when killed by enemy.")]
    [SerializeField] private int goldOnDeath;

    [Header("Effects")]
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private float deathDelay = 1f;

    public bool Placed { get; set; }

    private GridTile _gridOccupied;
    private List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();
    private PlayerStats _playerStats;

    public GridTile GridOccupied
    {
        get => _gridOccupied;
        set => _gridOccupied = value;
    }

    public int DamagePerShot => damagePerShot;

    public int PointsPerHit => pointsPerHit;

    public int PointsOnDeath => pointsOnDeath;

    public int Cost => cost;

    void Start()
    {
        _playerStats = FindObjectOfType<PlayerStats>();
        foreach (Transform child in transform.parent)
        {
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
            if (meshRenderer != null)   _meshRenderers.Add(meshRenderer);
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage < 0) return;
        health -= damage;
        UpdateHealthBar();
        if (health <= 0) StartCoroutine(Kill());
    }

    IEnumerator Kill()
    {
        if (CompareTag("Player"))   _playerStats.DropPoints(pointsOnDeath); // player turret died
        else
        {
            _playerStats.AddPoints(pointsOnDeath);                          // enemy turret died
            _playerStats.AddGold(goldOnDeath);
        }

        DeathEffects();
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject.transform.root.gameObject);
    }

    void DeathEffects()
    {
        explosionVFX.SetActive(true);
        healthBar.parent.parent.gameObject.SetActive(false);
        _gridOccupied.Occupied = false;
        Placed = false;
        foreach (MeshRenderer meshRenderer in _meshRenderers) meshRenderer.enabled = false;
    }

    void UpdateHealthBar()
    {
        if (healthBar != null) healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
    }
}
