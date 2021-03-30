using UnityEngine;

/// <summary>
/// Class to spawn enemies on a regular basis.
/// </summary>
public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnGap = 5f;

    private GridTile[] _gridTiles;  // every grid tile on the ground
    private GridTile _selectedTile;

    void Start()
    {
        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        _gridTiles = ground.GetComponentsInChildren<GridTile>();
        InvokeRepeating(nameof(Spawn), 0f, spawnGap);
    }

    /// <summary>
    /// Selects a random location on which to place the spawned enemy.
    /// </summary>
    /// <returns>the location where the enemy will be spawned</returns>
    Vector3 SelectRandomLocation()
    {
        do
        {
            _selectedTile = _gridTiles[Random.Range(0, _gridTiles.Length)];
        }
        while (_selectedTile.Occupied);
        // keeping going until unoccupied grid found

        _selectedTile.Occupied = true;
        return _selectedTile.transform.position;
    }

    void Spawn()
    {
        Vector3 location = SelectRandomLocation();
        location.y = 0f;    // set enemy on ground; just in case

        Turret turret = Instantiate(enemyPrefab, location, Quaternion.identity).GetComponentInChildren<Turret>();
        turret.Placed = true;
        turret.GridOccupied = _selectedTile;
        _selectedTile.Turret = turret;
    }
}
