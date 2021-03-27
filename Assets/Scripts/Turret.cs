using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public bool placed;
    [SerializeField] private int health = 100;
    [SerializeField] private int damagePerShot = 20;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private float deathDelay = 1f;

    private GridTile _gridOccupied;
    private List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();

    public GridTile GridOccupied
    {
        get => _gridOccupied;
        set => _gridOccupied = value;
    }

    public int DamagePerShot
    {
        get => damagePerShot;
    }

    void Start()
    {
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
        explosionVFX.SetActive(true);
        healthBar.parent.parent.gameObject.SetActive(false);
        _gridOccupied.Occupied = false;
        placed = false;
        foreach (MeshRenderer meshRenderer in _meshRenderers) meshRenderer.enabled = false;
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject.transform.root.gameObject);
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
        }
    }
}
