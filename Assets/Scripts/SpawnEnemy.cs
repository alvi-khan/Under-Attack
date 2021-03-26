using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnGap = 5f;

    private GameObject _ground;
    private int _rowCount;
    private int _cellCount;

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
        GridTile gridTile;
        do
        {
            int selectedRow = Random.Range(0, _rowCount);
            int selectedCell = Random.Range(0, _cellCount);
            gridTile = _ground.transform.GetChild(selectedRow).GetChild(selectedCell).GetComponent<GridTile>();
        }
        while (gridTile.Occupied);

        gridTile.Occupied = true;
        return gridTile.transform.position;
    }

    void Spawn()
    {
        Vector3 location = SelectRandomLocation();
        location.y = 0f;
        Instantiate(enemyPrefab, location, Quaternion.identity);
    }
}
