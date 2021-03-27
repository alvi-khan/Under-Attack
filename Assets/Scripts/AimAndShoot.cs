using System;
using UnityEngine;

[RequireComponent(typeof(Turret))]
public class AimAndShoot : MonoBehaviour
{
    [SerializeField] private string shootAt;
    [SerializeField] private float maxEnemyDistance = 35f;

    private GameObject _closestEnemy;
    private ParticleSystem.EmissionModule _laser;
    private Turret _turret;
    private PlayerStats _playerStats;

    private void Start()
    {
        _laser = GetComponentInChildren<ParticleSystem>().emission;
        _turret = transform.GetComponent<Turret>();
        _playerStats = FindObjectOfType<PlayerStats>();
    }

    void Update()
    {
        FindClosestEnemy();
        ShootClosestEnemy();
    }

    private void OnParticleCollision(GameObject other)
    {
        Turret enemyTurret = other.transform.parent.GetComponent<Turret>();
        UpdateScore(_turret.PointsPerHit);
        if (_turret.Placed) _turret.TakeDamage(enemyTurret.DamagePerShot);
    }

    void UpdateScore(int points)
    {
        if (CompareTag("Player"))   _playerStats.DropPoints(points);    // player got shot
        else
        {
            _playerStats.AddPoints(points);                             // enemy got shot
        }
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(shootAt);
        float maxDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            Turret enemyTurret = enemy.GetComponent<Turret>();
            if (_laser.enabled && !enemyTurret.Placed) continue;
            float enemyDist = Vector3.Distance(transform.position, enemy.transform.position);
            if (enemyDist < maxDist)
            {
                maxDist = enemyDist;
                _closestEnemy = enemy;
            }
        }
    }

    void ShootClosestEnemy()
    {
        if (_closestEnemy == null)
        {
            _laser.enabled = false;
            return;
        }

        transform.LookAt(_closestEnemy.transform);
        if (Vector3.Distance(transform.position, _closestEnemy.transform.position) < maxEnemyDistance)
        {
            _laser.enabled = false;
            Turret enemyTurret = _closestEnemy.transform.GetComponent<Turret>();
            if (_turret.Placed && enemyTurret.Placed) _laser.enabled = true;
        }
        else
        {
            _laser.enabled = false;
        }
    }
}
