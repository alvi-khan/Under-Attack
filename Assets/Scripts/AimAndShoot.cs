using UnityEngine;

/// <summary>
/// Class to control aiming and shooting of turrets.
/// Requires turret component.
/// </summary>
[RequireComponent(typeof(Turret))]
public class AimAndShoot : MonoBehaviour
{
    [Tooltip("Tag of object to shoot at.")]
    [SerializeField] private string shootAt;
    [SerializeField] private AudioClip shotSFX;
    [SerializeField] private float maxEnemyDistance = 35f;
    [Tooltip("Layer on which obstacles are placed.")]
    [SerializeField] private int obstacleLayer = 6;

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

    /// <summary>
    /// Events that occur when turret gets shot.
    /// </summary>
    /// <param name="other">is the game object that shot the turret.</param>
    private void OnParticleCollision(GameObject other)
    {
        // todo check possible bug that lets unplaced turrets lose points if brought in front of firing turret
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
        _closestEnemy = null;   // lose enemy currently being tracked.
        Vector3 here = transform.position;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(shootAt);
        float maxDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            Vector3 enemyPosition = enemy.transform.position;
            Turret enemyTurret = enemy.GetComponent<Turret>();

            // don't look at unplaced enemies while shooting at something
            if (_laser.enabled && enemyTurret != null && !enemyTurret.Placed) continue;

            float enemyDist = Vector3.Distance(here, enemyPosition);
            bool hidden = Physics.Linecast(here, enemyPosition, 1<<obstacleLayer);

            // if the enemy is close enough and is within line of sight
            if (enemyDist < maxDist && !hidden)
            {
                maxDist = enemyDist;
                _closestEnemy = enemy;
            }
        }
    }

    void ShootClosestEnemy()
    {
        if (!_turret.Placed) return;    // don't start shooting before being placed
        if (_closestEnemy == null)
        {
            _laser.enabled = false;
            return;
        }

        transform.LookAt(_closestEnemy.transform);  // rotate towards closest enemy
        if (Vector3.Distance(transform.position, _closestEnemy.transform.position) < maxEnemyDistance)
        {
            _laser.enabled = false; // just in case
            Turret enemyTurret = _closestEnemy.transform.GetComponent<Turret>();

            // if the enemy is a turret, is placed and this turret is also placed, start shooting
            if (enemyTurret != null && _turret.Placed && enemyTurret.Placed) _laser.enabled = true;
            // if the enemy is a base camp, start shooting
            else if (enemyTurret == null) _laser.enabled = true;
        }
        else _laser.enabled = false;    // look, but don't shoot
    }
}
