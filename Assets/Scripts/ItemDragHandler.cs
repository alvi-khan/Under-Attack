using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] float gridSize = 10f;

    private RectTransform _inventory;
    private Image _itemImage;
    private Vector3 _defaultPos;
    private GameObject _tempItem;
    private GameObject _cell;
    private UIUpdater _uiUpdater;

    private void Start()
    {
        _inventory = GameObject.FindGameObjectWithTag("Inventory").transform as RectTransform;
        _itemImage = GetComponent<Image>();
        _tempItem = Instantiate(itemPrefab, Vector3.zero, itemPrefab.transform.rotation);
        _defaultPos = transform.position;
        _uiUpdater = FindObjectOfType<UIUpdater>();

        SetVisibility(true, false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 currentPosition = Input.mousePosition;
        bool insideInventory = RectTransformUtility.RectangleContainsScreenPoint(_inventory, currentPosition);
        if (!insideInventory)   UpdateItemPosition(FindRealPos(currentPosition));
        else UpdateImagePosition(currentPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Reset();
        CreateItem(Input.mousePosition);
    }

    void SetVisibility(bool imageVisibility, bool objectVisibility)
    {
        _itemImage.enabled = imageVisibility;
        _tempItem.SetActive(objectVisibility);
    }

    Vector3 FindRealPos(Vector3 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            _cell = hit.collider.gameObject;
            if (_cell == null) return _tempItem.transform.position;
            Vector3 realPos = _cell.transform.position;
            GridTile gridTile = _cell.transform.parent.GetComponent<GridTile>();
            if (gridTile == null) return _tempItem.transform.position;
            if (gridTile.Occupied)
            {
                Transform turret = gridTile.Turret.transform;
                realPos.y = turret.position.y;
                realPos.y *= turret.root.localScale.y;
            }
            return realPos;
        }
        return _tempItem.transform.position;
    }

    void UpdateItemPosition(Vector3 currentPos)
    {
        SetVisibility(false, true);

        Vector3 snappedPos;
        snappedPos.x = Mathf.RoundToInt(currentPos.x / gridSize) * gridSize;
        snappedPos.y = Mathf.RoundToInt(currentPos.y / gridSize) * gridSize;
        snappedPos.z = Mathf.RoundToInt(currentPos.z / gridSize) * gridSize;

        _tempItem.transform.position = snappedPos;
    }

    void UpdateImagePosition(Vector3 currentPosition)
    {
        SetVisibility(true, false);
        transform.position = currentPosition;
    }

    void Reset()
    {
        transform.position = _defaultPos;
        SetVisibility(true, false);
    }

    void CreateItem(Vector3 mousePosition)
    {
        bool insideInventory = RectTransformUtility.RectangleContainsScreenPoint(_inventory, mousePosition);
        if (insideInventory) return;
        Turret turret = _tempItem.GetComponentInChildren<Turret>();
        if (turret != null && turret.Cost <= GameData.Gold) CreateTurret();
        else if (_tempItem.CompareTag("Repair Tool")) RepairPlayer();
        else if (_tempItem.CompareTag("Shield")) IncreaseDefense();
    }

    void CreateTurret()
    {
        GridTile gridTile = _cell.transform.parent.GetComponent<GridTile>();
        if (gridTile.Occupied) return;

        GameObject newTurret = Instantiate(itemPrefab, _tempItem.transform.position, Quaternion.identity);
        newTurret.SetActive(true);

        gridTile.Occupied = true;

        Turret turret = newTurret.GetComponentInChildren<Turret>();
        turret.Placed = true;
        turret.GridOccupied = gridTile;
        gridTile.Turret = turret;
        _uiUpdater.DropGold(turret.Cost);
    }

    void RepairPlayer()
    {
        GridTile gridTile = _cell.transform.parent.GetComponent<GridTile>();
        if (gridTile == null) return;
        Turret turret = gridTile.Turret;
        RepairTool repairTool = itemPrefab.GetComponent<RepairTool>();
        if (turret != null && turret.CompareTag("Player") && repairTool != null)
            turret.RepairTurret(repairTool.HealthIncrease, repairTool.Cost);
    }

    void IncreaseDefense()
    {
        GridTile gridTile = _cell.transform.parent.GetComponent<GridTile>();
        if (gridTile == null) return;
        Turret turret = gridTile.Turret;
        Shield shield = itemPrefab.GetComponent<Shield>();
        if (turret != null && turret.CompareTag("Player") && shield != null)
            turret.ShieldTurret(shield.ShieldAmount, shield.Cost);
    }
}
