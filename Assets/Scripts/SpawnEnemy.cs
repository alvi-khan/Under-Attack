using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnGap = 5f;

    private GameObject _ground;
    private int _rowCount;
    private int _cellCount;
    private GridTile _gridTile;

    void Start()
    {
        _ground = GameObject.FindGameObjectWithTag("Ground");
        GetGridDimensions();
        InvokeRepeating(nameof(Spawn), 0f, spawnGap);
    }

    void GetGridDimensions()
    {
        _rowCount = _ground.transform.childCount;
        _cellCount = _ground.transform.GetChild(0).childCount;
    }

    Vector3 SelectRandomLocation()
    {
        do
        {
            int selectedRow = Random.Range(0, _rowCount);
            int selectedCell = Random.Range(0, _cellCount);
            _gridTile = _ground.transform.GetChild(selectedRow).GetChild(selectedCell).GetComponent<GridTile>();
        }
        while (_gridTile.Occupied);

        _gridTile.Occupied = true;
        return _gridTile.transform.position;
    }

    void Spawn()
    {
        Vector3 location = SelectRandomLocation();
        location.y = 0f;
        Turret turret = Instantiate(enemyPrefab, location, Quaternion.identity).GetComponentInChildren<Turret>();
        turret.placed = true;
        turret.GridOccupied = _gridTile;
    }
}
