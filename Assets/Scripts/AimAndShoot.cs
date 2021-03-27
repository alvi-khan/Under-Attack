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

    private void Start()
    {
        _laser = GetComponentInChildren<ParticleSystem>().emission;
        _turret = transform.GetComponent<Turret>();
    }

    void Update()
    {
        FindClosestEnemy();
        ShootClosestEnemy();
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(gameObject.name + " got shot by " + other.gameObject.name);
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(shootAt);
        float maxDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
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
        if (_closestEnemy == null) return;

        transform.LookAt(_closestEnemy.transform);
        if (Vector3.Distance(transform.position, _closestEnemy.transform.position) < maxEnemyDistance)
        {
            _laser.enabled = false;
            Turret enemyTurret = _closestEnemy.transform.GetComponent<Turret>();
            if (_turret.placed && enemyTurret.placed) _laser.enabled = true;
        }
        else
        {
            _laser.enabled = false;
        }
    }
}
