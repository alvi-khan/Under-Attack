using UnityEngine;

[RequireComponent(typeof(Turret))]
public class AimAndShoot : MonoBehaviour
{
    [SerializeField] private string shootAt;
    [SerializeField] private AudioClip shotSFX;
    [SerializeField] private float maxEnemyDistance = 35f;

    private GameObject _closestEnemy;
    private ParticleSystem.EmissionModule _laser;
    private AudioSource _audioSource;
    private Turret _turret;
    private UIUpdater _uiUpdater;

    private void Start()
    {
        _laser = GetComponentInChildren<ParticleSystem>().emission;
        _turret = transform.GetComponent<Turret>();
        _uiUpdater = FindObjectOfType<UIUpdater>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        FindClosestEnemy();
        ShootClosestEnemy();
    }

    private void OnParticleCollision(GameObject other)
    {
        _audioSource.PlayOneShot(shotSFX);
        Turret enemyTurret = other.transform.parent.GetComponent<Turret>();
        UpdateScore(_turret.PointsPerHit);
        if (_turret.Placed) _turret.TakeDamage(enemyTurret.DamagePerShot);
    }

    void UpdateScore(int points)
    {
        if (CompareTag("Player"))   _uiUpdater.DropPoints(points); // player got shot
        else _uiUpdater.AddPoints(points);                         // enemy got shot
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(shootAt);
        float maxDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            Turret enemyTurret = enemy.GetComponent<Turret>();
            if (_laser.enabled && enemyTurret != null && !enemyTurret.Placed) continue;
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
            if (enemyTurret != null && _turret.Placed && enemyTurret.Placed) _laser.enabled = true;
            else if (enemyTurret == null) _laser.enabled = true;
        }
        else _laser.enabled = false;
    }
}
