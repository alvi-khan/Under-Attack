using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnGap = 5f;

    private GameObject _ground;
    private GridTile[] _gridTiles;
    private GridTile _selectedTile;

    void Start()
    {
        _ground = GameObject.FindGameObjectWithTag("Ground");
        _gridTiles = _ground.GetComponentsInChildren<GridTile>();
        InvokeRepeating(nameof(Spawn), 0f, spawnGap);
    }
    Vector3 SelectRandomLocation()
    {
        do
        {
            _selectedTile = _gridTiles[Random.Range(0, _gridTiles.Length)];
        }
        while (_selectedTile.Occupied);

        _selectedTile.Occupied = true;
        return _selectedTile.transform.position;
    }

    void Spawn()
    {
        Vector3 location = SelectRandomLocation();
        location.y = 0f;
        Turret turret = Instantiate(enemyPrefab, location, Quaternion.identity).GetComponentInChildren<Turret>();
        turret.Placed = true;
        turret.GridOccupied = _selectedTile;
        _selectedTile.Turret = turret;
    }
}
