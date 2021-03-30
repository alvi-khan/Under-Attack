using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class to handle drag and drop of items from inventory.
/// </summary>
public class ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] float gridSize = 10f;
    // getting grid size from code will prevent script from being attached to game objects in final build

    private RectTransform _inventory;
    private Image _itemImage;
    private Vector3 _defaultPos;    // default position of image
    private GameObject _tempItem;
    private GameObject _cell;   // grid onto which the item was dragged
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
        ResetImage();
        CreateItem(Input.mousePosition);
    }

    /// <summary>
    /// Sets the visibility conditions of the image and the actual object.
    /// </summary>
    /// <param name="imageVisibility"></param>
    /// <param name="objectVisibility"></param>
    void SetVisibility(bool imageVisibility, bool objectVisibility)
    {
        _itemImage.enabled = imageVisibility;
        _tempItem.SetActive(objectVisibility);
    }

    /// <summary>
    /// Converts mouse position to world position.
    /// </summary>
    /// <param name="mousePos"></param>
    /// <returns></returns>
    Vector3 FindRealPos(Vector3 mousePos)
    {
        // raycast from camera to mouse position in world
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            _cell = hit.collider.gameObject;    // grid onto which item was dragged
            if (_cell == null) return _tempItem.transform.position; // not a grid

            GridTile gridTile = _cell.transform.parent.GetComponent<GridTile>();
            if (gridTile == null) return _tempItem.transform.position;  // not a placeable grid

            Vector3 realPos = _cell.transform.position;
            if (gridTile.Occupied)  // for occupied grids, put item above occupying turret
            {
                Transform turret = gridTile.Turret.transform;
                realPos.y = turret.position.y;
                realPos.y *= turret.root.localScale.y;
            }
            return realPos;
        }
        return _tempItem.transform.position;
    }

    /// <summary>
    /// Snaps the dragged item between valid grids.
    /// </summary>
    /// <param name="currentPos"></param>
    void UpdateItemPosition(Vector3 currentPos)
    {
        SetVisibility(false, true);

        Vector3 snappedPos;
        snappedPos.x = Mathf.RoundToInt(currentPos.x / gridSize) * gridSize;
        snappedPos.y = Mathf.RoundToInt(currentPos.y / gridSize) * gridSize;
        snappedPos.z = Mathf.RoundToInt(currentPos.z / gridSize) * gridSize;

        _tempItem.transform.position = snappedPos;
    }

    /// <summary>
    /// Makes image position match mouse position.
    /// </summary>
    /// <param name="currentPosition"></param>
    void UpdateImagePosition(Vector3 currentPosition)
    {
        SetVisibility(true, false);
        transform.position = currentPosition;
    }

    void ResetImage()
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
