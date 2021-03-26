using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] float gridSize = 10f;
    [SerializeField] float yShift = 2.5f;
    private RectTransform _inventory;
    private Image _itemImage;
    private Vector3 _defaultPos;
    private GameObject _tempItem;
    private GameObject _cell;

    private void Start()
    {
        _inventory = GameObject.FindGameObjectWithTag("Inventory").transform as RectTransform;
        _itemImage = GetComponent<Image>();
        _tempItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
        _defaultPos = transform.position;

        SetVisibility(true, false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 currentPosition = Input.mousePosition;
        if (!RectTransformUtility.RectangleContainsScreenPoint(_inventory, currentPosition))    // outside inventory
        {
            UpdateItemPosition(FindRealPos(currentPosition));
        }
        else
        {
            UpdateImagePosition(currentPosition);
        }
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
            Vector3 realPos = _cell.transform.position;
            realPos.y = yShift;
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
        GridTile gridTile = _cell.transform.parent.GetComponent<GridTile>();
        if (!RectTransformUtility.RectangleContainsScreenPoint(_inventory, mousePosition) && !gridTile.Occupied)   // outside inventory and unoccupied
        {
            GameObject newItem = Instantiate(itemPrefab, _tempItem.transform.position, Quaternion.identity);
            newItem.SetActive(true);
            gridTile.Occupied = true;
        }
    }
}
