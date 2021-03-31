using System;
using TMPro;
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

    private Turret _turret;
    private RepairTool _repairTool;
    private Shield _shield;
    private TMP_Text _costText;

    private void Start()
    {
        _inventory = GameObject.FindGameObjectWithTag("Inventory").transform as RectTransform;
        _itemImage = GetComponent<Image>();
        _tempItem = Instantiate(itemPrefab, Vector3.zero, itemPrefab.transform.rotation);
        _defaultPos = transform.position;
        _uiUpdater = FindObjectOfType<UIUpdater>();

        _turret = _tempItem.GetComponentInChildren<Turret>();
        _repairTool = _tempItem.GetComponent<RepairTool>();
        _shield = _tempItem.GetComponent<Shield>();
        _costText = transform.parent.parent.Find("Cost").GetComponent<TMP_Text>();

        SetVisibility(true, false);
    }

    private void Update()
    {
        UpdateColors();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!ItemAvailable()) return;
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
        UpdateColors();
    }

    /// <summary>
    /// Sets colors for item depending on if they are too expensive or not.
    /// </summary>
    void UpdateColors()
    {
        if (!ItemAvailable())
        {
            _itemImage.color = new Color(0.11f, 0.13f, 0.15f);
            _costText.color = new Color(0.94f, 0.23f, 0.28f);
        }
        else
        {
            _itemImage.color = Color.white;
            _costText.color = Color.white;
        }
    }

    /// <summary>
    /// Checks if item is too expensive.
    /// </summary>
    /// <returns></returns>
    bool ItemAvailable()
    {
        if (_turret != null && _turret.Cost > GameData.Gold) return false;
        if (_repairTool != null && _repairTool.Cost > GameData.Gold) return false;
        if (_shield != null && _shield.Cost > GameData.Gold) return false;
        return true;
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
            GameObject objectHit = hit.collider.gameObject; // grid on which mouse is
            if (objectHit == null) return _tempItem.transform.position; // not a grid

            GridTile gridTile = objectHit.transform.parent.GetComponent<GridTile>();
            if (gridTile == null) return _tempItem.transform.position;  // not a placeable grid

            _cell = objectHit;  // grid on which item will be placed
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

        if (_turret != null && _turret.Cost <= GameData.Gold) CreateTurret();
        else if (_repairTool != null && _repairTool.Cost <= GameData.Gold) RepairPlayer();
        else if (_shield != null && _shield.Cost <= GameData.Gold) IncreaseDefense();
    }

    void CreateTurret()
    {
        GridTile gridTile = _cell.transform.parent.GetComponent<GridTile>();
        if (gridTile.Occupied) return;

        GameObject newTurret = Instantiate(itemPrefab, _tempItem.transform.position, Quaternion.identity);
        newTurret.SetActive(true);
        gridTile.Occupied = true;

        Turret turret = newTurret.GetComponentInChildren<Turret>(); // this is the placed object's turret
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
