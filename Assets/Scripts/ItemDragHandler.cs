using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] float gridSize = 10f;
    private RectTransform _inventory;
    private Image _itemImage;
    private GameObject _tempItem;
    private float _yShift = 2.5f;

    private void Start()
    {
        _inventory = GameObject.FindGameObjectWithTag("Inventory").transform as RectTransform;
        _itemImage = GetComponent<Image>();
        _tempItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);

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
            Vector3 realPos = hit.collider.gameObject.transform.position;
            realPos.y = _yShift;
            return realPos;
        }
        return mousePos;
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
        transform.localPosition = Vector3.zero;
        SetVisibility(true, false);
    }

    void CreateItem(Vector3 mousePosition)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(_inventory, mousePosition))   // outside inventory
        {
            GameObject newItem = Instantiate(itemPrefab, _tempItem.transform.position, Quaternion.identity);
            newItem.SetActive(true);
        }
    }
}
