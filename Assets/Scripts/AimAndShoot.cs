using UnityEngine;

public class AimAndShoot : MonoBehaviour
{
    [SerializeField] private string shootAt;
    private GameObject _closestEnemy;

    void Update()
    {
        FindClosestEnemy();
        ShootClosestEnemy();
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
        transform.LookAt(_closestEnemy.transform);
    }
}
